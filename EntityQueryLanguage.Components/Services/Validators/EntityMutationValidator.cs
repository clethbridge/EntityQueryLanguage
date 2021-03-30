using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Parsers;
using System.Collections.Generic;

namespace EntityQueryLanguage.Components.Services.Validators
{
    public interface IEntityMutationValidator
    {
        EqlValidation Validate(EntityMutation mutation, out Dictionary<string, dynamic> model);
    }

    [EqlService(typeof(IEntityMutationValidator))]
    public class EntityMutationValidator : IEntityMutationValidator
    {
        private IEntityValidator entityValidator;

        private INullValidator nullValidator;

        private IDataTypeParser dataTypeParser;

        private ILookupValidator lookupValidator;

        public EntityMutationValidator(
            IEntityValidator entityValidator,
            INullValidator nullValidator,
            IDataTypeParser dataTypeParser,
            ILookupValidator lookupValidator)
        {
            this.entityValidator = entityValidator;
            this.nullValidator = nullValidator;
            this.dataTypeParser = dataTypeParser;
            this.lookupValidator = lookupValidator;
        }

        public EqlValidation Validate(EntityMutation mutation, out Dictionary<string, dynamic> model)
        {
            model = new Dictionary<string, dynamic>();

            EntityType entityType;
            EqlValidation validation = entityValidator.Validate(mutation.EntityKey, mutation.Fields.Keys, out entityType);

            if (validation.IsValid)
            {
                AuthorizeOperation(mutation, validation, entityType);

                if (validation.IsValid)
                {
                    nullValidator.Validate(entityType, mutation, validation);

                    if (validation.IsValid)
                    {
                        Dictionary<string, int> encodings;
                        lookupValidator.Validate(entityType, mutation, validation, out encodings);

                        if (validation.IsValid)
                        {
                            foreach (var encoding in encodings)
                            {
                                mutation.Fields[encoding.Key] = encoding.Value;
                            }

                            model = dataTypeParser.Parse(entityType, mutation, validation);
                        }
                    }
                }
            }

            return validation;
        }

        private void AuthorizeOperation(
            EntityMutation mutation,
            EqlValidation validation,
            EntityType entityType)
        {
            switch (mutation.Type)
            {
                case EntityMutationType.Insert:
                    CheckInsert(validation, entityType);
                    break;
                case EntityMutationType.Update:
                    CheckUpdate(validation, entityType);
                    break;
                case EntityMutationType.Delete:
                    CheckDelete(validation, entityType);
                    break;
            }
        }

        private void CheckInsert(EqlValidation validation, EntityType entityType)
        {
            if (!entityType.CanInsert)
            {
                validation.Errors.Add($"'{entityType.EntityKey}' does not support inserts.");
            }
        }

        private void CheckUpdate(
            EqlValidation validation,
            EntityType entityType)
        {
            if (!entityType.CanUpdate)
                validation.Errors.Add($"'{entityType.EntityKey}' does not support updates.");
        }

        private void CheckDelete(
            EqlValidation validation,
            EntityType entityType)
        {
            if (!entityType.CanDelete)
                validation.Errors.Add($"'{entityType.EntityKey}' does not support deletes.");
        }
    }
}
