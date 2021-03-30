using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Executors;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace EntityQueryLanguage.IntegrationTests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class BulkInsertTests
    {
        private IEntityMutationExecutor entityMutationExecutor;

        private IEntityQueryExecutor entityQueryExecutor;

        public BulkInsertTests(
            IEntityMutationExecutor entityMutationExecutor,
            IEntityQueryExecutor entityQueryExecutor)
        {
            this.entityMutationExecutor = entityMutationExecutor;
            this.entityQueryExecutor = entityQueryExecutor;
        }

        
    }
}
