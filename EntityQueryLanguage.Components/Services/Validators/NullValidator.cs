using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Validators
{
    public interface INullValidator
    {
        void Validate(
            EntityType entityType,
            EntityMutation mutation,
            EqlValidation validation
        );
    }

    [EqlService(typeof(INullValidator))]
    public class NullValidator : INullValidator
    {
        private INullChecker nullChecker;

        public NullValidator(INullChecker nullChecker)
        {
            this.nullChecker = nullChecker;
        }

        public void Validate(EntityType entityType, EntityMutation mutation, EqlValidation validation)
        {
            switch (mutation.Type)
            {
                case EntityMutationType.Insert:
                    ValidateRequiredFields(entityType, mutation, validation);
                    break;
                case EntityMutationType.Update:
                    ValidateEntityId(entityType, mutation, validation);
                    ValidateRequiredFields(entityType, mutation, validation);
                    break;
                case EntityMutationType.Delete:
                    ValidateEntityId(entityType, mutation, validation);
                    break;
            }
        }

        private void ValidateRequiredFields(EntityType entityType, EntityMutation mutation, EqlValidation validation) =>
             entityType
            .RequiredFields
            .Where(field => !field.IsIdentity && nullChecker.IsNotPopulated(mutation, field))
            .ToList()
            .ForEach(field => validation.Errors.Add($"'{field.TermKey}' is required for '{entityType.EntityKey}'"));

        private void ValidateEntityId(EntityType entityType, EntityMutation mutation, EqlValidation validation)
        {
            if (!mutation.HasEntityId)
                validation.Errors.Add($"'EntityId' is required for any update or delete operation. Please make sure '{entityType.EntityKey}' has this field populated.'");
        }
    }
}
