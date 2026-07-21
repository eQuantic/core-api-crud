using System.Linq.Expressions;
using System.Reflection;
using eQuantic.Core.Application.Crud.Enums;
using eQuantic.Core.Application.Crud.Options;
using eQuantic.Core.Collections;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Options;
using eQuantic.Core.DataModel;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Exceptions;
using eQuantic.Linq.Expressions;
using eQuantic.Linq.Expressions.Casting;
using eQuantic.Linq.Web;
using eQuantic.Mapper;
using Microsoft.Extensions.Logging;

namespace eQuantic.Core.Application.Crud.Services;

public abstract class ReaderServiceBase<TEntity, TDataEntity> : ReaderServiceBase<TEntity, TDataEntity, int, int>
    where TEntity : class, IDomainEntity, new()
    where TDataEntity : class, IEntity<int>, new()
{
    protected ReaderServiceBase(IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork,
        IMapperFactory mapperFactory,
        ILogger logger,
        Action<ReadOptions>? options = null) : base(applicationContext, unitOfWork, mapperFactory, logger, options)
    {
    }
}

/// <summary>
///     The read side of the CRUD service. Filtering and sorting are authored over the domain entity
///     (<typeparamref name="TEntity" />) and rewritten to the data entity (<typeparamref name="TDataEntity" />)
///     with <see cref="ExpressionCast" /> — configure custom maps by overriding <see cref="OnConfigureCast" />.
///     The rewritten predicate and sortings drive a <see cref="QueryOptions{TDataEntity}" /> against the v5
///     repository.
/// </summary>
public abstract class ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey> : IReaderService<TEntity, TKey>
    where TEntity : class, IDomainEntity, new()
    where TDataEntity : class, IEntity<TKey>, new()
{
    protected readonly ILogger Logger;
    protected IApplicationContext<TUserKey> ApplicationContext { get; }
    protected IMapperFactory MapperFactory { get; }
    protected IQueryableUnitOfWork UnitOfWork { get; }
    protected IAsyncQueryableRepository<TDataEntity, TKey> Repository { get; }
    protected ReadOptions ReadOptions { get; }

    private ExpressionCast<TEntity, TDataEntity>? _cast;

    protected ReaderServiceBase(
        IApplicationContext<TUserKey> applicationContext,
        IQueryableUnitOfWork unitOfWork,
        IMapperFactory mapperFactory,
        ILogger logger, Action<ReadOptions>? options = null)
    {
        Logger = logger;
        ApplicationContext = applicationContext;
        MapperFactory = mapperFactory;
        UnitOfWork = unitOfWork;
        Repository = unitOfWork.GetAsyncQueryableRepository<TDataEntity, TKey>();

        var readOptions = new ReadOptions();
        options?.Invoke(readOptions);
        ReadOptions = readOptions;
    }

    /// <summary>The domain → data cast, built once from <see cref="OnConfigureCast" /> (by-name auto-mapping by default).</summary>
    protected ExpressionCast<TEntity, TDataEntity> Cast => _cast ??= ExpressionCast.Create<TEntity, TDataEntity>(OnConfigureCast);

    public virtual async Task<TEntity?> GetByIdAsync(GetRequest<TKey> request,
        CancellationToken cancellationToken = default)
    {
        var options = new QueryOptions<TDataEntity>();
        Include(options, request.IncludeFields);
        OnConfigureQuery(CrudAction.Get, options);

        var item = await Repository.GetAsync(request.Id, options, cancellationToken);

        if (item == null)
        {
            var ex = new EntityNotFoundException<TKey>(request.Id);
            Logger.LogError(ex, "{ServiceName} - GetById: Entity of {EntityName} not found", GetType().Name,
                typeof(TEntity).Name);
            throw ex;
        }

        await OnCheckPermissionsAsync(CrudAction.Get, item, cancellationToken);

        ValidateReference(request, item);

        var result = await OnMapEntityAsync(request, item, cancellationToken);

        await OnAfterGetByIdAsync(item, result, cancellationToken);
        return result;
    }

    public virtual async Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request,
        CancellationToken cancellationToken = default)
    {
        var options = new QueryOptions<TDataEntity>();

        var predicate = Combine(BuildFilter(request), await OnBuildPermissionFilterAsync());
        if (predicate != null)
        {
            options.Where(predicate);
        }

        ApplySorting(options, request.GetSorts());
        Include(options, request.IncludeFields);
        OnConfigureQuery(CrudAction.GetPaged, options);

        await OnBeforeGetPagedListAsync(request, options, cancellationToken);

        var pageIndex = request.PageIndex ?? 1;
        var pageSize = request.PageSize ?? 10;
        var paged = await Repository.GetPagedAsync(new PageRequest(pageIndex, pageSize), options, cancellationToken);
        var dataList = paged.Items.ToList();

        var list = new List<TEntity>();
        foreach (var dataEntity in dataList)
        {
            var item = await OnMapEntityAsync(request, dataEntity, cancellationToken);
            if (item != null)
                list.Add(item);
        }

        await OnAfterGetPagedListAsync(dataList, list, cancellationToken);

        return new PagedList<TEntity>(list, paged.TotalCount) { PageIndex = pageIndex, PageSize = pageSize };
    }

    // ---------------------------------------------------------------- query building

    /// <summary>The data-shaped predicate: the request's domain filter cast to the data entity, plus any reference filter.</summary>
    protected Expression<Func<TDataEntity, bool>>? BuildFilter(PagedListRequest<TEntity> request)
    {
        var domainPredicate = request.GetFilterPredicate();
        var predicate = domainPredicate is null ? null : Cast.Predicate(domainPredicate);
        return Combine(predicate, GetReferenceFilter(request));
    }

    private void ApplySorting(QueryOptions<TDataEntity> options, IReadOnlyList<QuerySort<TEntity>> sortings)
    {
        if (sortings.Count == 0)
        {
            return;
        }

        // Cast each domain sort selector to the data shape and emit the v3 query-string order form
        // (e.g. "createdAt:desc,name") — the target member path survives explicit maps.
        var orderBy = string.Join(",", sortings.Select(sort =>
        {
            var path = MemberPath(Cast.Lambda(sort.KeySelector));
            return sort.Direction == SortDirection.Descending ? $"{path}:desc" : path;
        }));

        options.OrderBy(orderBy);
    }

    private static void Include(QueryOptions<TDataEntity> options, string[]? includeFields)
    {
        if (includeFields is { Length: > 0 })
        {
            options.Include(includeFields);
        }
    }

    private static Expression<Func<TDataEntity, bool>>? Combine(
        Expression<Func<TDataEntity, bool>>? left, Expression<Func<TDataEntity, bool>>? right) =>
        left is null ? right : right is null ? left : left.And(right);

    private static string MemberPath(LambdaExpression lambda)
    {
        var parts = new List<string>();
        var expression = Unwrap(lambda.Body);
        while (expression is MemberExpression member)
        {
            parts.Add(member.Member.Name);
            expression = member.Expression is null ? null! : Unwrap(member.Expression);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }

    private static Expression Unwrap(Expression expression) =>
        expression is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary
            ? Unwrap(unary.Operand)
            : expression;

    // ---------------------------------------------------------------- extensibility hooks

    /// <summary>Configures the domain → data cast (explicit maps, nested shapes). By-name auto-mapping applies by default.</summary>
    protected virtual void OnConfigureCast(CastOptions<TEntity, TDataEntity> cast)
    {
    }

    /// <summary>Shapes the query further per action (e.g. add includes or a base filter).</summary>
    protected virtual void OnConfigureQuery(CrudAction action, QueryOptions<TDataEntity> options)
    {
    }

    /// <summary>Builds the ownership predicate that scopes a listing to the current user, when the entity is owned.</summary>
    protected virtual async Task<Expression<Func<TDataEntity, bool>>?> OnBuildPermissionFilterAsync()
    {
        if (!IsOwned())
            return null;

        var isInRole = await CheckIsInRolesAsync();
        if (!ReadOptions.OnlyOwner)
        {
            if (isInRole == false)
                throw new ForbiddenAccessException();

            return null;
        }

        var userId = await ApplicationContext.GetCurrentUserIdAsync();
        if (userId == null)
            throw new ForbiddenAccessException();

        if (isInRole == true)
            return null;

        return OwnerPredicate(userId);
    }

    protected virtual async Task<TEntity?> OnMapEntityAsync(
        IGetRequest getRequest,
        TDataEntity dataEntity,
        CancellationToken cancellationToken = default)
    {
        var mapper = MapperFactory.GetAnyMapper<TDataEntity, TEntity>();
        return await mapper.MapAsync(dataEntity, cancellationToken);
    }

    protected virtual Task OnAfterGetByIdAsync(
        TDataEntity? dataEntity,
        TEntity? entity,
        CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected virtual async Task OnCheckPermissionsAsync(
        CrudAction action,
        TDataEntity? dataEntity,
        CancellationToken cancellationToken = default)
    {
        if (dataEntity is not IEntityOwned<TUserKey> ownedEntity)
            return;

        var isOwner = await CheckOwnerAsync(ownedEntity);
        var isInRole = await CheckIsInRolesAsync();
        if (!isOwner && isInRole == false)
            throw new ForbiddenAccessException();
    }

    protected virtual Task OnAfterGetPagedListAsync(
        IEnumerable<TDataEntity> dataEntityList,
        IEnumerable<TEntity> entityList,
        CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected virtual Task OnBeforeGetPagedListAsync(
        PagedListRequest<TEntity> request,
        QueryOptions<TDataEntity> options,
        CancellationToken cancellationToken = default) => Task.CompletedTask;

    // ---------------------------------------------------------------- permissions / ownership

    private async Task<bool?> CheckIsInRolesAsync()
    {
        if (!ReadOptions.Roles.Any())
            return null;

        var roles = await ApplicationContext.GetCurrentUserRolesAsync();
        return roles.Length != 0 && roles.Any(role => ReadOptions.Roles.Contains(role));
    }

    private async Task<bool> CheckOwnerAsync(IEntityOwned<TUserKey> ownedEntity)
    {
        if (!ReadOptions.OnlyOwner)
            return true;

        var userId = await ApplicationContext.GetCurrentUserIdAsync();
        return userId != null && userId.Equals(ownedEntity.CreatedById);
    }

    private static Expression<Func<TDataEntity, bool>> OwnerPredicate(TUserKey? userId)
    {
        // entity => entity.CreatedById == userId (CreatedById comes from IEntityOwned<TUserKey>)
        var parameter = Expression.Parameter(typeof(TDataEntity), "entity");
        var createdBy = Expression.Property(parameter, nameof(IEntityOwned<TUserKey>.CreatedById));
        var body = Expression.Equal(createdBy, Expression.Constant(userId, createdBy.Type));
        return Expression.Lambda<Func<TDataEntity, bool>>(body, parameter);
    }

    private static bool IsOwned() =>
        typeof(TDataEntity).GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityOwned<>));

    // ---------------------------------------------------------------- references

    private void ValidateReference(BasicRequest request, TDataEntity item)
    {
        var referenceType = request.GetReferenceType();
        if (referenceType == null)
            return;

        typeof(ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey>)
            .GetMethod(nameof(ValidateReference), BindingFlags.NonPublic | BindingFlags.Static)?
            .MakeGenericMethod(referenceType)
            .Invoke(null, [request, item]);
    }

    private static void ValidateReference<TReferenceKey>(BasicRequest request, TDataEntity item)
    {
        if (request is not IReferencedRequest<TReferenceKey> referencedRequest ||
            item is not IWithReferenceId<TDataEntity, TReferenceKey> referencedItem)
            return;

        if (referencedItem.GetReferenceId()?.Equals(referencedRequest.GetReferenceId()) == false)
        {
            throw new InvalidEntityReferenceException<TReferenceKey>(referencedRequest.GetReferenceId()!);
        }
    }

    private Expression<Func<TDataEntity, bool>>? GetReferenceFilter(BasicRequest request)
    {
        var referenceType = request.GetReferenceType();
        if (referenceType == null)
            return null;

        return (Expression<Func<TDataEntity, bool>>?)typeof(ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey>)
            .GetMethod(nameof(GetReferenceFilter), BindingFlags.NonPublic | BindingFlags.Static)?
            .MakeGenericMethod(referenceType)
            .Invoke(null, [request]);
    }

    private static Expression<Func<TDataEntity, bool>>? GetReferenceFilter<TReferenceKey>(BasicRequest request)
    {
        if (request is not IReferencedRequest<TReferenceKey> referencedRequest)
            return null;

        var dataEntity = new TDataEntity();
        if (dataEntity is not IWithReferenceId<TDataEntity, TReferenceKey> referencedDataEntity)
            return null;

        referencedDataEntity.SetReferenceId(referencedRequest.GetReferenceId()!);
        return referencedDataEntity.GetReferenceFilter();
    }
}
