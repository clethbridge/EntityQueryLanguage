using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Components.Services.DataAccess;
using EntityQueryLanguage.Components.Services.Deserialization;
using EntityQueryLanguage.Components.Services.Validators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.Executors
{
    public interface IEntityQueryExecutor
    {
        Task<List<ExpandoObject>> ExecuteAsync(EntityQuery entityQuery);
    }

    [EqlService(typeof(IEntityQueryExecutor))]
    public class EntityQueryExecutor: IEntityQueryExecutor
    {
        private EntitySchema entitySchema;

        private IEntityValidator entityValidator;

        private IDbContext dbContext;

        private ISelectStatementBuilder selectStatementBuilder;

        private IDataTableDeserializer dataTableDeserializer;

        public EntityQueryExecutor(
            EntitySchema entitySchema,
            IEntityValidator entityValidator,
            IDbContext dbContext,
            ISelectStatementBuilder selectStatementBuilder,
            IDataTableDeserializer dataTableDeserializer)
        {
            this.entitySchema = entitySchema;
            this.entityValidator = entityValidator;
            this.dbContext = dbContext;
            this.selectStatementBuilder = selectStatementBuilder;
            this.dataTableDeserializer = dataTableDeserializer;
        }

        public async Task<List<ExpandoObject>> ExecuteAsync(EntityQuery entityQuery)
        {
            EntityType entityType;
            EqlValidation validation = entityValidator.Validate(entityQuery.EntityKey, entityQuery.TermKeys, out entityType);

            if (validation.IsValid)
            {
                SqlStatement statement = selectStatementBuilder.Build(entityQuery);

                DataTable dataTable = await dbContext.ExecuteQueryAsync(statement.Sql, statement.Parameters);

                var records = dataTableDeserializer.Deserialize(dataTable, entityQuery);

                return records;
            }
            else
            {
                throw new Exception($"{string.Join("\r\n", validation.Errors)}");
            }
        }
    }
}
