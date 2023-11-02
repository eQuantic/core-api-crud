using eQuantic.Core.Application.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.Configurations;

/// <summary>
/// The entity time track configuration class
/// </summary>
/// <seealso cref="EntityOwnedConfiguration{TEntity,TUser}"/>
public class EntityTrackConfiguration<TEntity> : EntityTimeMarkConfiguration<TEntity>
    where TEntity : class, IEntityTimeMark, IEntityTimeTrack
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
    }
}

/// <summary>
/// The entity track configuration class
/// </summary>
/// <seealso cref="EntityOwnedConfiguration{TEntity,TUser}"/>
public class EntityTrackConfiguration<TEntity, TUser> : EntityOwnedConfiguration<TEntity, TUser>
    where TEntity : class, IEntityTrack<TUser>, IEntityOwned<TUser>
    where TUser : class
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

        builder.HasOne(o => o.UpdatedBy)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }
}