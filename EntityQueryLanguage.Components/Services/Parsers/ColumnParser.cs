using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Parsers
{
    public interface IColumnParser
    {
        List<string> Parse(EntityQuery entityQuery);
    }

    [EqlService(typeof(IColumnParser))]
    public class ColumnParser: IColumnParser
    {
        private EntitySchema entitySchema;

        private IColumnBuilder columnBuilder;

        public ColumnParser(EntitySchema entitySchema, IColumnBuilder columnBuilder)
        {
            this.entitySchema = entitySchema;
            this.columnBuilder = columnBuilder;
        }

        public List<string> Parse(EntityQuery subjectEntityQuery)
        {
            var tokens = new List<string>();

            AddRelatedColumns(subjectEntityQuery, tokens);

            return tokens;
        }

        private void AddRelatedColumns(EntityQuery entityQuery, List<string> tokens)
        {
            EntityType entityType = entitySchema.GetEntityType(entityQuery.EntityKey);

            var relatedColumns = BuildColumns(entityQuery, entityType);

            tokens.AddRange(relatedColumns);
            
            bool noPrimary = !entityQuery.TermKeys.Contains(entityType.PrimaryKey.TermKey);

            if (noPrimary)
            {
                string primaryKeyToken = columnBuilder.Build(entityType, entityType.PrimaryKey);

                tokens.Add(primaryKeyToken);
            }

            foreach (EntitySubQuery entitySubQuery in entityQuery.SubQueries)
            {
                AddRelatedColumns(entitySubQuery.EntityQuery, tokens);
            }
        }

        private IEnumerable<string> BuildColumns(EntityQuery entityQuery, EntityType entityType) =>
             entityQuery
            .TermKeys
            .Select(termKey => entityType.GetEntityField(termKey))
            .Select(entityField => columnBuilder.Build(entityType, entityField));
    }
}
