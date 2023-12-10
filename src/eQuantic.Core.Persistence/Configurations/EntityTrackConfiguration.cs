using eQuantic.Core.Application.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.Configurations;

/// <summary>
/// The entity time track configuration class
/// </summary>
/// <seealso cref="EntityOwnedConfiguration{TEntity,TUser}"/>
public class EntityTrackConfiguration<TEntity> : EntityTrackConfiguration<TEntity, int>
    where TEntity : class, IEntityOwned<int>, IEntityTrack<int>
{
}

/// <summary>
/// The entity time track configuration class
/// </summary>
/// <seealso cref="EntityOwnedConfiguration{TEntity,TUser}"/>
public class EntityTrackConfiguration<TEntity, TUserKey> : EntityTimeTrackConfiguration<TEntity>
    where TEntity : class, IEntityOwned<TUserKey>, IEntityTrack<TUserKey>
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
    }
}

/// <summary>
/// The entity track configuration class
/// </summary>
/// <seealso cref="EntityOwnedConfiguration{TEntity,TUser}"/>
public class EntityTrackConfiguration<TEntity, TUser, TUserKey> : EntityOwnedConfiguration<TEntity, TUser, TUserKey>
    where TEntity : class, IEntityTrack<TUser, TUserKey>, IEntityOwned<TUser, TUserKey>
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
        
        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.UpdatedById)
            .IsRequired(false);
        
        builder.HasOne(o => o.UpdatedBy)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }
}