using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public interface ISelectStatementBuilder
    {
        SqlStatement Build(EntityQuery entityQuery);
    }

    [EqlService(typeof(ISelectStatementBuilder))]
    public class SelectStatementBuilder: ISelectStatementBuilder
    {
        private EntitySchema entitySchema;

        private IJoinParser relatedEntityParser;

        private IColumnParser columnParser;

        public SelectStatementBuilder(
            EntitySchema entitySchema,
            IJoinParser relatedEntityParser,
            IColumnParser columnParser)
        {
            this.entitySchema = entitySchema;
            this.relatedEntityParser = relatedEntityParser;
            this.columnParser = columnParser;
        }

        public SqlStatement Build(EntityQuery entityQuery)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($"{Constants.SELECT}\r\n\t");

            EntityType entityType = entitySchema.GetEntityType(entityQuery.EntityKey);

            var columns = columnParser.Parse(entityQuery);

            var columnSegment = string.Join($"\r\n\t,", columns);

            sql.AppendLine($" {columnSegment}");
            
            sql.AppendLine($"{Constants.FROM} {entityType.DatabaseName}");

            var joinTokens = relatedEntityParser.Parse(entityQuery);

            joinTokens.ForEach(join => sql.AppendLine($"\t{join}"));

            return new SqlStatement()
            {
                Sql = sql.ToString().Trim()
            };
        }
    }
}
