using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public interface IUpdateStatementBuilder : IMutationStatementBuilder { }

    [EqlService(typeof(IUpdateStatementBuilder))]
    public class UpdateStatementBuilder : IUpdateStatementBuilder
    {
        private EntitySchema schema;

        public UpdateStatementBuilder(EntitySchema schema)
        {
            this.schema = schema;
        }

        public SqlStatement Build(EntityMutation entityMutation, Dictionary<string, dynamic> model)
        {
            EntityType entityType = schema.GetEntityType(entityMutation.EntityKey);

            var parameters = model.ToDictionary(x => x.Key, x => x.Value);

            string primaryKey = entityType.PrimaryKey.ColumnName.UnquoteName();

            parameters.Add($"@{primaryKey}", entityMutation.EntityId);

            string columns = string.Join("\r\n \t,", model.Select(field => $"\t[{field.Key}] = @{field.Key}"));

            string statement =
                 new StringBuilder($"UPDATE {entityType.DatabaseName} \r\n")
                .AppendLine("SET")
                .AppendLine(columns)
                .AppendLine($"WHERE {primaryKey} = @{primaryKey}")
                .ToString();

            return new SqlStatement()
            {
                Sql = statement,
                Parameters = parameters
            };
        }
    }
}
