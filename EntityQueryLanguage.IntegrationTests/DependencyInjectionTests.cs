using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Executors;
using System;
using Xunit;

namespace EntityQueryLanguage.IntegrationTests
{
    public class DependencyInjectionTests: BaseIntegrationTest  
    {
        private IEntityQueryExecutor entityQueryExecutor;

        private EntitySchema entitySchema;

        public DependencyInjectionTests(
            IEntityQueryExecutor entityQueryExecutor,
            EntitySchema entitySchema)
        {
            this.entityQueryExecutor = entityQueryExecutor;
            this.entitySchema = entitySchema;
        }

        [Fact(DisplayName = "Able to inject 'QueryExecutor' into a class")]
        public void QueryExecutor() => Assert.NotNull(entityQueryExecutor);

        [Fact(DisplayName = "Able to inject 'EntitySchema' into a class")]
        public void EntitySchema() => Assert.NotNull(entitySchema);
    }
}
