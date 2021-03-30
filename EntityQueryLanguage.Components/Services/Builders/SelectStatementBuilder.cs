using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
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

        private IColumnBuilder columnBuilder;

        public SelectStatementBuilder(
            EntitySchema entitySchema,
            IColumnBuilder columnBuilder)
        {
            this.entitySchema = entitySchema;
            this.columnBuilder = columnBuilder;
        }

        public SqlStatement Build(EntityQuery entityQuery)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($"{Constants.SELECT}\r\n\t");

            EntityType entityType = entitySchema.GetEntityType(entityQuery.EntityKey);

            var columns = GetColumns(entityQuery, entityType);

            var columnSegment = string.Join($"\r\n\t,", columns);

            sql.AppendLine($" {columnSegment}");
            
            sql.AppendLine($"{Constants.FROM} {entityType.DatabaseName}");

            return new SqlStatement()
            {
                Sql = sql.ToString().Trim()
            };
        }

        private IEnumerable<string> GetColumns(EntityQuery entityQuery, EntityType entityType) =>
             entityQuery
            .TermKeys
            .Select(termKey => entityType.GetEntityField(termKey))
            .Select(entityField => columnBuilder.Build(entityType, entityField));
    }
}
