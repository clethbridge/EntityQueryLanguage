using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public interface IDeleteStatementBuilder : IMutationStatementBuilder { }

    [EqlService(typeof(IDeleteStatementBuilder))]
    public class DeleteStatementBuilder : IDeleteStatementBuilder
    {
        private EntitySchema schema;

        public DeleteStatementBuilder(EntitySchema schema)
        {
            this.schema = schema;
        }

        public SqlStatement Build(EntityMutation entityMutation, Dictionary<string, dynamic> model)
        {
            EntityType entityType = schema.GetEntityType(entityMutation.EntityKey);

            var parameters = new Dictionary<string, dynamic>();

            string primaryKey = entityType.PrimaryKey.ColumnName.UnquoteName();

            parameters.Add($"@{primaryKey}", entityMutation.EntityId);

            return new SqlStatement()
            {
                Sql = $"DELETE FROM {entityType.DatabaseName} WHERE {primaryKey} = @{primaryKey}",
                Parameters = parameters
            };
        }
    }
}
