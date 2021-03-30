using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Components.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.Executors
{
    public interface IUpdateExecutor : IMutationStatementExecutor { }

    [EqlService(typeof(IUpdateExecutor))]
    public class UpdateExecutor : MutationStatementExecutor, IUpdateExecutor
    {
        private IUpdateStatementBuilder statementBuilder;

        public UpdateExecutor(
            IDbContext dbContext,
            IEntityQueryExecutor queryExecutor,
            IUpdateStatementBuilder statementBuilder) :
            base(dbContext, queryExecutor)
        {
            this.statementBuilder = statementBuilder;
        }

        public async Task<dynamic> ExecuteAsync(EntityMutation mutation, Dictionary<string, dynamic> model)
        {
            try
            {
                SqlStatement statement = statementBuilder.Build(mutation, model);

                await dbContext.ExecuteNonQueryAsync(statement.Sql, statement.Parameters);

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
