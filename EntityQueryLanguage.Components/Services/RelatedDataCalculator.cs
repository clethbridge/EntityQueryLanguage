using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services
{
    public interface IRelatedDataCalculator
    {
        List<string> Calculate(string entityKey);
    }

    [EqlService(typeof(IRelatedDataCalculator))]
    public class RelatedDataCalculator : IRelatedDataCalculator
    {
        private EntitySchema schema;

        private List<string> allPrimaryKeys;

        public RelatedDataCalculator(EntitySchema schema)
        {
            this.schema = schema;

            allPrimaryKeys =
                 schema
                .EntityTypes
                .Where(e => e.PrimaryKey != null)
                .Select(e => e.PrimaryKey.TermKey)
                .ToList();
        }

        public List<string> Calculate(string entityKey)
        {
            EntityType context = schema.GetEntityType(entityKey);

            if (context.PrimaryKey != null)
            {
                var entityTypes = schema.EntityTypes.Where(e => e.EntityKey != entityKey);

                var referenceTermKeys =
                     context
                    .EntityFields
                    .Where(f => !f.IsPrimaryKey)
                    .Where(f => allPrimaryKeys.Contains(f.TermKey))
                    .Select(f => f.TermKey);

                var referencedIn =
                      entityTypes
                     .Where(e =>
                        e.EntityFields.Any(f =>
                            string.Equals(f.TermKey, context.PrimaryKey.TermKey, StringComparison.OrdinalIgnoreCase)
                        )
                     );

                var referenceEntities =
                     entityTypes.Where(e => e.EntityFields.Any(f => referenceTermKeys.Contains(f.TermKey)));

                return
                     referencedIn
                    .Union(referenceEntities)
                    .Select(e => e.EntityKey)
                    .ToList();
            }
            else
            {
                return new List<string>();
            }
        }
    }
}
