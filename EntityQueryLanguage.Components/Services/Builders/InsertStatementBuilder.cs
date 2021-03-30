using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public interface IInsertStatementBuilder : IMutationStatementBuilder { }

    [EqlService(typeof(IInsertStatementBuilder))]
    public class InsertStatementBuilder : IInsertStatementBuilder
    {
        private EntitySchema schema;

        public InsertStatementBuilder(EntitySchema schema)
        {
            this.schema = schema;
        }

        public SqlStatement Build(EntityMutation entityMutation, Dictionary<string, dynamic> model)
        {
            EntityType entityType = schema.GetEntityType(entityMutation.EntityKey);

            return new SqlStatement()
            {
                Sql = BuildStatement(model, entityType),
                Parameters = model.ToDictionary(x => $"@{x.Key}", x => x.Value)
            };
        }

        private string BuildStatement(Dictionary<string, dynamic> model, EntityType entityType)
        {
            string columns = string.Join("\r\n \t,", model.Keys.Select(k => $"[{k}]"));

            string values = string.Join("\r\n \t,", model.Keys.Select(k => $"@{k}"));

            return
                 new StringBuilder($"INSERT INTO {entityType.DatabaseName} (\r\n")
                .AppendLine(columns)
                .AppendLine(") VALUES (\r\n")
                .AppendLine(values)
                .AppendLine(")")
                .AppendLine($"SELECT [{entityType.PrimaryKey.TermKey}] = SCOPE_IDENTITY()")
                .ToString();
        }
    }
}
