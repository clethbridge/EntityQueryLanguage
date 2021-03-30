using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Validators
{
    public interface IEntityQueryValidator
    {
        EqlValidation Validate(EntityQuery entityQuery);
    }

    [EqlService(typeof(IEntityQueryValidator))]
    public class EntityQueryValidator : IEntityQueryValidator
    {
        private IEntityValidator entityValidator;

        private IRelatedDataCalculator relatedDataCalculator;

        public EntityQueryValidator(
            IEntityValidator entityKeyValidator,
            IRelatedDataCalculator relatedDataCalculator)
        {
            this.entityValidator = entityKeyValidator;
            this.relatedDataCalculator = relatedDataCalculator;
        }

        public EqlValidation Validate(EntityQuery entityQuery)
        {
            EqlValidation validation = new EqlValidation();

            ValidateQuery(entityQuery, validation);

            return validation;
        }

        private void ValidateQuery(
            EntityQuery entityQuery,
            EqlValidation validation)
        {

            EntityType entityType;
            EqlValidation eqlValidation = entityValidator.Validate(entityQuery.EntityKey, entityQuery.TermKeys, out entityType);

            if (eqlValidation.IsValid)
            {
                ValidateFilters(entityQuery, validation, entityType.TermKeys);

                ValidateSubQueries(entityQuery, validation, entityType);

                entityQuery
               .SubQueries
               .ForEach(subQuery => ValidateQuery(subQuery.EntityQuery, validation));
            }
        }

        private void ValidateSubQueries(EntityQuery entityQuery, EqlValidation validation, EntityType entityType)
        {
            var missingNames = entityQuery.SubQueries.Where(c => string.IsNullOrEmpty(c.Name)).ToList();

            missingNames
           .ForEach(missingName =>
           {
               string error = BuildMissingNameError(entityQuery);

               validation.Errors.Add(error);
           });

            var relatedEntityKeys = relatedDataCalculator.Calculate(entityQuery.EntityKey);

            if (!entityQuery.SubQueries.Any(q => q.Conditions.Count() > 0))
            {
                entityQuery
               .SubQueries
               .Where(subQuery => !relatedEntityKeys.Contains(subQuery.EntityQuery.EntityKey))
               .ToList()
               .ForEach(subQuery =>
               {
                   string error = BuildSubQueryError(entityQuery, subQuery);

                   validation.Errors.Add(error);
               });
            }
        }

        private string BuildMissingNameError(EntityQuery entityQuery) =>
             $"The 'Name' field must be populated for the sub query on '{entityQuery.EntityKey}'.";

        private void ValidateFilters(
            EntityQuery entityQuery,
            EqlValidation validation,
            List<string> validTermKeys)
        {
            GetInvalidFilters(entityQuery, validTermKeys)
           .ForEach(filter =>
           {
               string error = BuildFilterError(entityQuery, filter);

               validation.Errors.Add(error);
           });

            GetInvalidListFilters(entityQuery)
           .ForEach(fieldFilter =>
                validation.Errors.Add($"The 'Values' field must be populated when filtering on '{fieldFilter.TermKey}' for '{entityQuery.EntityKey}'.")
            );
        }

        private List<EntityFieldFilter> GetInvalidListFilters(EntityQuery entityQuery) =>
             entityQuery
            .Filter
            .FieldFilters
            .Where(fieldFilter =>
                fieldFilter.Operator == OperatorType.In ||
                fieldFilter.Operator == OperatorType.NotIn
            )
            .Where(fieldFilter =>
                fieldFilter.Values == null ||
                fieldFilter.Values?.Count == 0
            )
            .ToList();

        private string BuildSubQueryError(EntityQuery entityQuery, EntitySubQuery subQuery) =>
             new StringBuilder("Invalid Sub-Query - ")
            .Append($"There is no path registered within the")
            .Append($"schema connecting '{entityQuery.EntityKey} '")
            .Append($"to '{subQuery.EntityQuery.EntityKey}.")
            .ToString();

        private string BuildFilterError(EntityQuery entityQuery, EntityFieldFilter filter) =>
             new StringBuilder($"Invalid Filter -  ")
            .Append($"Unable to select '{filter.TermKey}'")
            .Append($"since it is not defined for {entityQuery.EntityKey}")
            .ToString();

        private List<EntityFieldFilter> GetInvalidFilters(EntityQuery entityQuery, List<string> validTermKeys) =>
             entityQuery
            .Filter
            .FieldFilters
            .Where(filter => !validTermKeys.Contains(filter.TermKey))
            .ToList();
    }
}
