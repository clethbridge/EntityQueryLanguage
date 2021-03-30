using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Factories
{
    public interface IEntityMutationBuilderFactory
    {
        IEntityMutationBuilder<T> Get<T>();
    }

    [EqlService(typeof(IEntityMutationBuilderFactory))]
    public class EntityMutationBuilderFactory: IEntityMutationBuilderFactory
    {
        private EntitySchema entitySchema;

        public EntityMutationBuilderFactory(EntitySchema entitySchema)
        {
            this.entitySchema = entitySchema;
        }

        public IEntityMutationBuilder<T> Get<T>() => new EntityMutationBuilder<T>(entitySchema);
    }
}
