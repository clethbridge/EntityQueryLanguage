using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Components.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.Executors
{
    public interface IInsertExecutor : IMutationStatementExecutor { }

    [EqlService(typeof(IInsertExecutor))]
    public class InsertExecutor : MutationStatementExecutor, IInsertExecutor
    {
        private IInsertStatementBuilder statementBuilder;

        public InsertExecutor(
            IDbContext dbContext,
            IEntityQueryExecutor queryExecutor,
            IInsertStatementBuilder statementBuilder) :
            base(dbContext, queryExecutor)
        {
            this.statementBuilder = statementBuilder;
        }

        public async Task<dynamic> ExecuteAsync(
            EntityMutation mutation, 
            Dictionary<string, dynamic> model)
        {
            try
            {
                SqlStatement statement = statementBuilder.Build(mutation, model);

                DataTable result = await dbContext.ExecuteQueryAsync(statement.Sql, statement.Parameters);

                string primaryKeyTermKey = result.Columns[0].ColumnName;

                var identity = result.Rows[0][0];

                if (mutation.Return != null)
                {
                    mutation.Return.EntityId = identity;

                    var payload = await queryExecutor.ExecuteAsync(mutation.Return);

                    return payload[0];
                }
                else
                {
                    return identity;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
