using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services
{
    public interface IParameterParser
    {
        void Evaluate(EntityQuery rootQuery, Dictionary<string, dynamic> parameters);

        void Evaluate(EntityMutation mutation, Dictionary<string, dynamic> parameters);
    }

    [EqlService(typeof(IParameterParser))]
    public class ParameterParser : IParameterParser
    {
        public void Evaluate(EntityQuery rootQuery, Dictionary<string, dynamic> parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    Evaluate(rootQuery, parameter);
                }
            }
        }

        public void Evaluate(EntityMutation mutation, Dictionary<string, dynamic> parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    EvaluateEntityId(mutation, parameter);

                    GetParameterizedFields(mutation, parameter)
                   .ForEach(field => mutation.Fields[field.Key] = parameter.Value);

                    if (mutation.Return != null)
                    {
                        Evaluate(mutation.Return, parameter);
                    }
                }
            }
        }

        private List<KeyValuePair<string, dynamic>> GetParameterizedFields(EntityMutation mutation, KeyValuePair<string, dynamic> parameter) =>
             mutation
            .Fields
            .Where(field => FilterMatches(field.Value, parameter))
            .ToList();

        private void Evaluate(EntityQuery query, KeyValuePair<string, dynamic> parameter)
        {
            EvaluateEntityId(query, parameter);

            GetFilterCandidates(query, parameter)
           .ForEach(candidate => candidate.Value = parameter.Value);

            GetJoinConditionCandidates(query, parameter)
           .ForEach(candidate => candidate.Value = parameter.Value);

            query
           .SubQueries
           .ForEach(subQuery => Evaluate(subQuery.EntityQuery, parameter));
        }

        private List<EntityFieldFilter> GetFilterCandidates(EntityQuery query, KeyValuePair<string, dynamic> parameter) =>
             query
            .Filter
            .FieldFilters
            .Where(f => FilterMatches(f.Value, parameter))
            .ToList();

        private List<EntityJoinCondition> GetJoinConditionCandidates(EntityQuery query, KeyValuePair<string, dynamic> parameter) =>
             query
            .SubQueries
            .SelectMany(subQuery => subQuery.Conditions)
            .Where(condition => FilterMatches(condition.Value, parameter))
            .ToList();

        private bool FilterMatches(dynamic value, KeyValuePair<string, dynamic> parameter) =>
            value != null && parameter.Key == value.ToString();

        private void EvaluateEntityId(IEntityId command, KeyValuePair<string, dynamic> parameter)
        {
            if (command.EntityId != null && command.EntityId.GetType() == typeof(string))
            {
                string stringValue = command.EntityId.ToString();

                if (parameter.Key == stringValue)
                {
                    command.EntityId = parameter.Value;
                }
            }
        }
    }
}
