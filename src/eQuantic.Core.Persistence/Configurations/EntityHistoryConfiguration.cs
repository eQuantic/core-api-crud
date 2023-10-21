using eQuantic.Core.Application.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.Configurations;

/// <summary>
/// The entity history configuration class
/// </summary>
/// <seealso cref="EntityTrackConfiguration{TEntity,TUser}"/>
public class EntityHistoryConfiguration<TEntity> : EntityTrackConfiguration<TEntity> 
    where TEntity : class, IEntityTimeMark, IEntityTimeTrack, IEntityTimeEnded
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
    }
}

/// <summary>
/// The entity history configuration class
/// </summary>
/// <seealso cref="EntityTrackConfiguration{TEntity,TUser}"/>
public class EntityHistoryConfiguration<TEntity, TUser> : EntityTrackConfiguration<TEntity, TUser> 
    where TEntity : EntityHistoryDataBase<TUser> 
    where TUser : class
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
        
        builder.HasOne(o => o.DeletedBy)
            .WithMany()
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }
}
