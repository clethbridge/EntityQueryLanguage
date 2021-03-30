using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Validators
{
    public interface ILookupValidator
    {
        void Validate(
            EntityType entityType,
            EntityMutation mutation,
            EqlValidation validation,
            out Dictionary<string, int> encodings);
    }

    [EqlService(typeof(ILookupValidator))]
    public class LookupValidator : ILookupValidator
    {
        private EntitySchema schema;

        public LookupValidator(EntitySchema schema)
        {
            this.schema = schema;
        }

        public void Validate(
            EntityType entityType,
            EntityMutation mutation,
            EqlValidation validation,
            out Dictionary<string, int> encodings)
        {
            encodings = new Dictionary<string, int>();

            var lookupTerms = GetLookupTerms(entityType);

            var lookupFields = mutation.Fields.Where(f => lookupTerms.Contains(f.Key));

            foreach (var field in lookupFields)
            {
                var allowableValues = schema.Lookups.Where(l => l.TermKey == field.Key);

                string value = Convert.ToString(field.Value);

                int code;
                bool isCode = int.TryParse(value, out code);

                if (isCode)
                {
                    HandleCode(validation, encodings, field, allowableValues, code);
                }
                else
                {
                    HandleValue(validation, encodings, field, allowableValues, value);
                }
            }
        }

        private void HandleCode(
            EqlValidation validation,
            Dictionary<string, int> encodings,
            KeyValuePair<string, dynamic> field,
            IEnumerable<EntityLookup> allowableValues,
            int code)
        {
            var allowableValue = allowableValues.FirstOrDefault(av => av.Code == code);

            if (allowableValue == null)
            {
                encodings.Add(field.Key, code);
                var codes = allowableValues.Select(av => av.Code);

                string error = BuildCodeError(field, codes);

                validation.Errors.Add(error);
            }
        }

        private void HandleValue(
            EqlValidation validation,
            Dictionary<string, int> encodings,
            KeyValuePair<string, dynamic> field,
            IEnumerable<EntityLookup> allowableValues,
            string value)
        {
            var allowableValue = allowableValues.FirstOrDefault(av => av.Value == value);

            if (allowableValue != null)
            {
                int code = schema.EncodeLookup(field.Key, field.Value);

                encodings.Add(field.Key, code);
            }
            else
            {
                var values = allowableValues.Select(av => av.Value);

                string error = BuildValueError(field, values);

                validation.Errors.Add(error);
            }
        }

        private IEnumerable<string> GetLookupTerms(EntityType entityType) =>
             entityType
            .EntityFields
            .Where(f => f.IsLookup)
            .Select(f => f.TermKey);

        private string BuildValueError(KeyValuePair<string, dynamic> field, IEnumerable<string> allowableValues) =>
             new StringBuilder()
            .AppendLine($"'{field.Value}' is not a valid value for '{field.Key}'.")
            .AppendLine("The following values are allowed:")
            .AppendLine(string.Join("\r\n", allowableValues))
            .ToString();

        private string BuildCodeError(KeyValuePair<string, dynamic> field, IEnumerable<int> allowableCodes) =>
             new StringBuilder()
            .AppendLine($"'{field.Value}' is not a valid code for '{field.Key}'.")
            .AppendLine("The following values are allowed:")
            .AppendLine(string.Join("\r\n", allowableCodes))
            .ToString();
    }
}
