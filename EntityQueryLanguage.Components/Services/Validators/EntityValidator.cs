using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityQueryLanguage.Components.Services.Validators
{
    public interface IEntityValidator
    {
        EqlValidation Validate(string entityKey, IEnumerable<string> termKeys, out EntityType entityType);
    }

    [EqlService(typeof(IEntityValidator))]
    public class EntityValidator: IEntityValidator
    {
        private EntitySchema entitySchema;

        public EntityValidator(EntitySchema entitySchema)
        {
            this.entitySchema = entitySchema;
        }

        public EqlValidation Validate(string entityKey, IEnumerable<string> termKeys, out EntityType entityType)
        {
            EqlValidation eqlValidation = new EqlValidation();

            entityType = entitySchema.GetEntityType(entityKey);

            if (entityType == null)
            {
                eqlValidation.Errors.Add($"'{entityKey}' does not exist within the schema.");
            }
            else
            {
                var definiedFields = entityType.EntityFields.Select(f => f.TermKey);

                var undefinedFields = termKeys.Where(termKey => !definiedFields.Contains(termKey));

                if (undefinedFields.Count() > 0)
                {
                    var errors = undefinedFields.Select(field => $"'{field}' is not defined for '{entityKey}'.");

                    eqlValidation.Errors.AddRange(errors);
                }
            }

            return eqlValidation;
        }
    }
}
