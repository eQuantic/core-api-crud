using eQuantic.Core.Application.Entities.Data;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace eQuantic.Core.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void UseCamelCase(this ModelBuilder modelBuilder, string entitySuffix = "Data")
    {
        foreach(var entity in modelBuilder.Model.GetEntityTypes())
        {
            var entityName = entity.Name.TrimEnd(entitySuffix.ToCharArray());
            
            // Replace table names
            entity.SetTableName(entityName.Pluralize().Camelize());

            // Replace column names            
            foreach(var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.Camelize());
            }

            foreach(var key in entity.GetKeys())
            {
                var name = key.GetName();
                if (name == nameof(EntityDataBase.Id))
                {
                    name = entityName + name;
                }
                key.SetName(name.Camelize());
            }
        }
    }
}
