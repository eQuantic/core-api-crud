using eQuantic.Core.Application.Entities.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.Configurations;

/// <summary>
/// The entity time track configuration class
/// </summary>
/// <seealso cref="EntityTimeMarkConfiguration{TEntity}"/>
public class EntityTimeEndedConfiguration<TEntity> : EntityTimeTrackConfiguration<TEntity>
    where TEntity : class, IEntityTimeMark, IEntityTimeTrack, IEntityTimeEnded
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);
        
        builder.Property(x => x.DeletedAt)
            .IsRequired(false);
    }
}