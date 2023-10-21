using eQuantic.Core.Application.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.Configurations;

/// <summary>
/// The entity owned configuration class
/// </summary>
/// <seealso cref="EntityTimeMarkConfiguration{TEntity}"/>
public class EntityOwnedConfiguration<TEntity, TUser> : EntityTimeMarkConfiguration<TEntity> 
    where TEntity : class, IEntityOwned<TUser> 
    where TUser : class
{
    /// <summary>
    /// Configures the builder
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.HasOne(o => o.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
