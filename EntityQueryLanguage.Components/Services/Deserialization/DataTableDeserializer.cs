using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Deserialization
{
    public interface IDataTableDeserializer
    {
        List<ExpandoObject> Deserialize(DataTable dataTable, EntityQuery entityQuery);
    }

    [EqlService(typeof(IDataTableDeserializer))]
    public class DataTableDeserializer : IDataTableDeserializer
    {
        private EntitySchema entitySchema;

        private IDataRowDeserializer dataRowDeserializer;

        private IEntityLocator entityLocator;

        public DataTableDeserializer(
            EntitySchema entitySchema,
            IDataRowDeserializer dataRowDeserializer,
            IEntityLocator entityLocator)
        {
            this.entitySchema = entitySchema;
            this.dataRowDeserializer = dataRowDeserializer;
            this.entityLocator = entityLocator;
        }

        public List<ExpandoObject> Deserialize(DataTable dataTable, EntityQuery entityQuery)
        {
            EntityType entityType = entitySchema.GetEntityType(entityQuery.EntityKey);

            List<DataRow> rows = GetRows(dataTable);

            return DeserializeEntities(entityQuery, entityType, rows);
        }

        private List<ExpandoObject> DeserializeEntities(
            EntityQuery entityQuery, 
            EntityType entityType,
            List<DataRow> rows)
        {
            var entityIds = entityLocator.GetUniqueIds(rows, entityType.PrimaryKey.TermKey).ToList();

            var payloads = new List<ExpandoObject>();

            if (entityIds.Count() > 0)
            {
                foreach (dynamic entityId in entityIds)
                {
                    var payload = DeserializeEntity(entityQuery, entityType, rows, entityId);

                    payloads.Add(payload);
                }
            }

            return payloads;
        }

        private ExpandoObject DeserializeEntity(
            EntityQuery entityQuery,
            EntityType entityType,
            List<DataRow> rows,
            dynamic entityId)
        {
            try
            {
                var subjectRows = entityLocator.FilterById(rows, entityType.PrimaryKey.TermKey, entityId);

                ExpandoObject payload = dataRowDeserializer.Deserialize(subjectRows[0], entityType, entityQuery.TermKeys);

                return payload;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<DataRow> GetRows(DataTable records) =>
             records
            .Rows
            .Cast<DataRow>()
            .ToList();
    }
}
