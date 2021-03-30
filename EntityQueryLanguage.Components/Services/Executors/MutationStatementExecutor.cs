using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.Executors
{
    public interface IMutationStatementExecutor
    {
        Task<dynamic> ExecuteAsync(EntityMutation mutation, Dictionary<string, dynamic> model);
    }

    public class MutationStatementExecutor
    {
        protected IDbContext dbContext;

        protected IEntityQueryExecutor queryExecutor;

        public MutationStatementExecutor(
            IDbContext dbContext,
            IEntityQueryExecutor queryExecutor)
        {
            this.dbContext = dbContext;
            this.queryExecutor = queryExecutor;
        }
    }
}
