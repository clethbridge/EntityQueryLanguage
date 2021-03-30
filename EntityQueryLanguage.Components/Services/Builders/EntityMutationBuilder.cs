using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public interface IEntityMutationBuilder<T> : IDisposable
    {
        EntityMutationBuilder<T> Insert(T instance);

        EntityMutationBuilder<T> Return(EntityQuery returnQuery);

        EntityMutation ToMutation();
    }

    public class EntityMutationBuilder<T> : BaseEntityBuilder, IEntityMutationBuilder<T>
    {
        private Type modelType;

        private EntityMutation entityMutation;

        private EntitySchema entitySchema;

        public EntityMutationBuilder(EntitySchema entitySchema)
        {
            this.entitySchema = entitySchema;
            
            modelType = typeof(T);

            entityMutation = new EntityMutation() 
            { 
                EntityKey = GetEntityKey(modelType)
            };
        }

        public EntityMutationBuilder<T> Insert(T instance)
        {
            string entityKey = GetEntityKey(modelType);

            EntityType entityType = entitySchema.GetEntityType(entityKey);

            if (entityType != null)
            {
                string primaryKeyTermKey = entityType.PrimaryKey?.TermKey;

                var fields = GetMutationFields(instance, primaryKeyTermKey);

                entityMutation = new EntityMutation()
                {
                    EntityKey = entityKey,
                    Type = EntityMutationType.Insert,
                    Fields = fields
                };

                return this;
            }
            else
            {
                throw new ArgumentException($"'{modelType.Name}' must have an entity key that exists within the schema.");
            }
        }

        public EntityMutationBuilder<T> Return(EntityQuery returnQuery)
        {
            entityMutation.Return = returnQuery;

            return this;
        }

        public EntityMutation ToMutation() => entityMutation;

        public void Dispose()
        {
            modelType = null;
            entityMutation = null;
        }

        private Dictionary<string, dynamic> GetAllFields(T instance) =>
             modelType
            .GetProperties()
            .Where(property => property.GetCustomAttribute<DbDefault>() is null)
            .Where(property => property.GetCustomAttribute<TermKey>() != null)
            .ToDictionary(
                 property => property.GetCustomAttribute<TermKey>().Value,
                 property => (dynamic)property.GetValue(instance)
            );

        private Dictionary<string, dynamic> GetMutationFields(T instance, string primaryKeyTermKey) =>
             GetAllFields(instance)
            .Where(field => field.Key != primaryKeyTermKey)
            .ToDictionary(f => f.Key, f => f.Value);
    }
}
