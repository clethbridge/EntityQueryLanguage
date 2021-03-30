using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Executors
{
    public interface IMutationStatementExecutorFactory
    {
        IMutationStatementExecutor Get(EntityMutationType mutationType);
    }

    [EqlService(typeof(IMutationStatementExecutorFactory))]
    public class MutationStatementExecutorFactory : IMutationStatementExecutorFactory
    {
        private IInsertExecutor insertExecutor;
        private IUpdateExecutor updateExecutor;
        private IDeleteExecutor deleteExecutor;

        public MutationStatementExecutorFactory(
            IInsertExecutor insertExecutor,
            IUpdateExecutor updateExecutor,
            IDeleteExecutor deleteExecutor)
        {
            this.insertExecutor = insertExecutor;
            this.updateExecutor = updateExecutor;
            this.deleteExecutor = deleteExecutor;
        }

        public IMutationStatementExecutor Get(EntityMutationType mutationType)
        {
            switch (mutationType)
            {
                case EntityMutationType.Insert: return insertExecutor;
                case EntityMutationType.Update: return updateExecutor;
                case EntityMutationType.Delete: return deleteExecutor;
                default: throw new NotImplementedException();
            }
        }
    }
}
