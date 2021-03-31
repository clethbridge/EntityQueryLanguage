using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Parsers
{
    public interface IJoinParser
    {
        List<string> Parse(EntityQuery entityQuery);
    }

    [EqlService(typeof(IJoinParser))]
    public class JoinParser: IJoinParser
    {
        private EntitySchema entitySchema;

        public JoinParser(EntitySchema entitySchema)
        {
            this.entitySchema = entitySchema;
        }

        /// <summary>
        /// Recurses through the projections and collections of the subject query to build tokens for the SQL join statements 
        /// </summary>
        public List<string> Parse(EntityQuery subjectEntityQuery)
        {
            List<string> tokens = new List<string>();

            foreach (EntitySubQuery entitySubQuery in subjectEntityQuery.SubQueries)
            { 
                AddJoinTokens(subjectEntityQuery.EntityKey, entitySubQuery, tokens);
            }

            return tokens;
        }

        private void AddJoinTokens(string subjectEntityKey, EntitySubQuery entitySubQuery, List<string> tokens)
        {
            AddJoinToken(subjectEntityKey, entitySubQuery, tokens);

            foreach (EntitySubQuery nestedSubQuery in entitySubQuery.EntityQuery.SubQueries)
            {
                AddJoinTokens(entitySubQuery.EntityQuery.EntityKey, nestedSubQuery, tokens);
            }
        }

        private void AddJoinToken(string subjectEntityKey, EntitySubQuery entitySubQuery, List<string> tokens)
        {
            EntityType subjectEntityType = entitySchema.GetEntityType(subjectEntityKey);

            EntityType targetEntityType;
            EntityField targetJoinField, subjectJoinField;

            DetermineJoinFields(
                entitySubQuery,
                subjectEntityType,
                out targetEntityType,
                out targetJoinField,
                out subjectJoinField
            );

            string token =
                $"LEFT JOIN {targetEntityType.DatabaseName} ON {targetEntityType.DatabaseName}.{targetJoinField.ColumnName} = {subjectEntityType.DatabaseName}.{subjectJoinField.ColumnName}";

            tokens.Add(token);
        }

        private void DetermineJoinFields(
            EntitySubQuery entitySubQuery,
            EntityType subjectEntityType,
            out EntityType targetEntityType,
            out EntityField targetJoinField, 
            out EntityField subjectJoinField)
        {
            EntityQuery targetEntityQuery = entitySubQuery.EntityQuery;

            targetEntityType = entitySchema.GetEntityType(targetEntityQuery.EntityKey);

            EntityField targetPrimaryKey = targetEntityType.PrimaryKey;

            EntityField targetForeignKey = targetEntityType.GetEntityField(subjectEntityType.PrimaryKey.TermKey);

            EntityField subjectForeignKey = subjectEntityType.GetEntityField(targetPrimaryKey.TermKey);

            targetJoinField = targetForeignKey == null ? targetPrimaryKey : targetForeignKey;

            subjectJoinField = targetForeignKey == null ? subjectForeignKey : subjectEntityType.PrimaryKey;
        }
    }
}
