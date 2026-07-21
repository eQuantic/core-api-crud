using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Entities.Data;
using eQuantic.Core.Api.Sample.Entities.Requests;
using eQuantic.Core.Api.Sample.Services;
using eQuantic.Core.Collections;
using eQuantic.Core.Domain.Entities.Requests;

namespace eQuantic.Core.Api.Crud.Tests;

/// <summary>
/// In-memory fake of <see cref="IExampleService"/>. The integration tests swap the EF-backed
/// services for these fakes so they exercise the controller layer (routing, model binding, status
/// codes) without the data/EF infrastructure.
/// </summary>
public sealed class FakeExampleService : IExampleService
{
    private readonly Dictionary<int, Example> _store = new();
    private int _next;

    public Example Seed(string name)
    {
        var id = ++_next;
        var entity = new Example { Id = id, Name = name };
        _store[id] = entity;
        return entity;
    }

    public Task<int> CreateAsync(CreateRequest<ExampleRequest> request, CancellationToken cancellationToken = default)
    {
        var id = ++_next;
        _store[id] = new Example { Id = id, Name = request.Body?.Name ?? string.Empty };
        return Task.FromResult(id);
    }

    public Task<bool> UpdateAsync(UpdateRequest<ExampleRequest, int> request, CancellationToken cancellationToken = default)
    {
        if (!_store.TryGetValue(request.Id, out var entity))
            return Task.FromResult(false);
        entity.Name = request.Body?.Name ?? entity.Name;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(ItemRequest<int> request, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Remove(request.Id));

    public Task<Example?> GetByIdAsync(GetRequest<int> request, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.TryGetValue(request.Id, out var entity) ? entity : null);

    public Task<IPagedEnumerable<Example>?> GetPagedListAsync(PagedListRequest<Example> request, CancellationToken cancellationToken = default)
    {
        var items = _store.Values.ToList();
        return Task.FromResult<IPagedEnumerable<Example>?>(new PagedList<Example>(items, items.Count));
    }
}

/// <summary>
/// In-memory fake of <see cref="IChildExampleService"/> (referenced entity).
/// </summary>
public sealed class FakeChildExampleService : IChildExampleService
{
    private readonly Dictionary<int, ChildExample> _store = new();
    private int _next;

    public ChildExample Seed(int id, string name)
    {
        var entity = new ChildExample { Id = id, Name = name };
        _store[id] = entity;
        if (id > _next) _next = id;
        return entity;
    }

    public Task<int> CreateAsync(CreateRequest<ChildExampleRequest> request, CancellationToken cancellationToken = default)
    {
        var id = ++_next;
        _store[id] = new ChildExample { Id = id, Name = request.Body?.Name ?? string.Empty };
        return Task.FromResult(id);
    }

    public Task<bool> UpdateAsync(UpdateRequest<ChildExampleRequest, int> request, CancellationToken cancellationToken = default)
    {
        if (!_store.TryGetValue(request.Id, out var entity))
            return Task.FromResult(false);
        entity.Name = request.Body?.Name ?? entity.Name;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(ItemRequest<int> request, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Remove(request.Id));

    public Task<ChildExample?> GetByIdAsync(GetRequest<int> request, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.TryGetValue(request.Id, out var entity) ? entity : null);

    public Task<IPagedEnumerable<ChildExample>?> GetPagedListAsync(PagedListRequest<ChildExample> request, CancellationToken cancellationToken = default)
    {
        var items = _store.Values.ToList();
        return Task.FromResult<IPagedEnumerable<ChildExample>?>(new PagedList<ChildExample>(items, items.Count));
    }
}

/// <summary>
/// In-memory fake of <see cref="IExampleWithComplexKeyService"/> (composite key).
/// </summary>
public sealed class FakeExampleWithComplexKeyService : IExampleWithComplexKeyService
{
    private readonly Dictionary<ExampleWithComplexKeyData.ExampleKey, ExampleWithComplexKey> _store = new();

    public ExampleWithComplexKey Seed(string code, string location)
    {
        var key = new ExampleWithComplexKeyData.ExampleKey(code, location);
        var entity = new ExampleWithComplexKey { Code = code, Location = location };
        _store[key] = entity;
        return entity;
    }

    public Task<ExampleWithComplexKeyData.ExampleKey> CreateAsync(CreateRequest<ExampleWithComplexKeyRequest> request, CancellationToken cancellationToken = default)
    {
        var body = request.Body;
        var key = new ExampleWithComplexKeyData.ExampleKey(body?.Code ?? string.Empty, body?.Location ?? string.Empty);
        _store[key] = new ExampleWithComplexKey { Code = key.Code, Location = key.Location };
        return Task.FromResult(key);
    }

    public Task<bool> UpdateAsync(UpdateRequest<ExampleWithComplexKeyRequest, ExampleWithComplexKeyData.ExampleKey> request, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.ContainsKey(request.Id));

    public Task<bool> DeleteAsync(ItemRequest<ExampleWithComplexKeyData.ExampleKey> request, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Remove(request.Id));

    public Task<ExampleWithComplexKey?> GetByIdAsync(GetRequest<ExampleWithComplexKeyData.ExampleKey> request, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.TryGetValue(request.Id, out var entity) ? entity : null);

    public Task<IPagedEnumerable<ExampleWithComplexKey>?> GetPagedListAsync(PagedListRequest<ExampleWithComplexKey> request, CancellationToken cancellationToken = default)
    {
        var items = _store.Values.ToList();
        return Task.FromResult<IPagedEnumerable<ExampleWithComplexKey>?>(new PagedList<ExampleWithComplexKey>(items, items.Count));
    }
}
