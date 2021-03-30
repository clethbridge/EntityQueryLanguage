using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Components.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.Executors
{
    public interface IDeleteExecutor : IMutationStatementExecutor { }

    [EqlService(typeof(IDeleteExecutor))]
    public class DeleteExecutor : MutationStatementExecutor, IDeleteExecutor
    {
        private IDeleteStatementBuilder statementBuilder;

        public DeleteExecutor(
            IDbContext dbContext,
            IEntityQueryExecutor queryExecutor,
            IDeleteStatementBuilder statementBuilder) :
            base(dbContext, queryExecutor)
        {
            this.statementBuilder = statementBuilder;
        }

        public async Task<dynamic> ExecuteAsync(EntityMutation mutation, Dictionary<string, dynamic> model)
        {
            try
            {
                SqlStatement statment = statementBuilder.Build(mutation, model);

                await dbContext.ExecuteNonQueryAsync(statment.Sql, statment.Parameters);

                if (mutation.Return != null)
                {
                    var payload = await queryExecutor.ExecuteAsync(mutation.Return);

                    return payload;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
