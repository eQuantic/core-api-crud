using eQuantic.Core.Application.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.Configurations;

public class EntityOwnedConfiguration<TEntity> : EntityTimeMarkConfiguration<TEntity>
    where TEntity : class, IEntityOwned<int>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);
        
        builder.Property(o => o.CreatedById)
            .IsRequired();
    }
}

/// <summary>
/// The entity owned configuration class
/// </summary>
/// <seealso cref="EntityTimeMarkConfiguration{TEntity}"/>
public class EntityOwnedConfiguration<TEntity, TUserKey> : EntityTimeMarkConfiguration<TEntity>
    where TEntity : class, IEntityOwned<TUserKey>
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
    }
}

public class EntityOwnedConfiguration<TEntity, TUser, TUserKey> : EntityTimeMarkConfiguration<TEntity>
    where TEntity : class, IEntityOwned<TUser, TUserKey> 
    where TUser : class
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
        
        builder.HasOne(o => o.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
