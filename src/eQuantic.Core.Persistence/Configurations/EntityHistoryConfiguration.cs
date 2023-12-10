using eQuantic.Core.Application.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.Configurations;

public class EntityHistoryConfiguration<TEntity> : EntityHistoryConfiguration<TEntity, int>
    where TEntity : class, IEntityOwned<int>, IEntityTrack<int>, IEntityHistory<int>
{
}

/// <summary>
/// The entity history configuration class
/// </summary>
/// <seealso cref="EntityTimeEndedConfiguration{TEntity}"/>
public class EntityHistoryConfiguration<TEntity, TUserKey> : EntityTimeEndedConfiguration<TEntity> 
    where TEntity : class, IEntityOwned<TUserKey>, IEntityTrack<TUserKey>, IEntityHistory<TUserKey>
    where TUserKey : struct
{
    /// <summary>
    /// Configures the builder
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(o => o.CreatedById)
            .IsRequired();
        
        builder.Property(x => x.UpdatedById)
            .IsRequired(false);
        
        builder.Property(x => x.DeletedById)
            .IsRequired(false);
    }
}

/// <summary>
/// The entity history configuration class
/// </summary>
/// <seealso cref="EntityTrackConfiguration{TEntity,TUser}"/>
public class EntityHistoryConfiguration<TEntity, TUser, TUserKey> : EntityTrackConfiguration<TEntity, TUser, TUserKey> 
    where TEntity : class, IEntityOwned<TUser, TUserKey>, IEntityTrack<TUser, TUserKey>, IEntityHistory<TUser, TUserKey>
    where TUser : class
    where TUserKey : struct
{
    /// <summary>
    /// Configures the builder
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);
        
        builder.Property(x => x.DeletedById)
            .IsRequired(false);
        
        builder.HasOne(o => o.DeletedBy)
            .WithMany()
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }
}
