using EntityQueryLanguage.Components;
using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            List<DataRow> rows = dataTable.GetRows();

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

                foreach (EntitySubQuery projection in entityQuery.Projections)
                {
                    List<ExpandoObject> payloads = DeserializeSubQuery(projection, subjectRows);

                    ExpandoObject projectionPayload = payloads.FirstOrDefault();

                    payload.AddField(projection.Name, projectionPayload);
                }

                foreach (EntitySubQuery collection in entityQuery.Collections)
                {
                    List<ExpandoObject> collectionPayloads = DeserializeSubQuery(collection, subjectRows);

                    payload.AddField(collection.Name, collectionPayloads);
                }

                return payload;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ExpandoObject> DeserializeSubQuery(EntitySubQuery entitySubQuery, List<DataRow> subjectRows)
        {
            var payloads = new List<ExpandoObject>();

            EntityType subQueryEntityType = entitySchema.GetEntityType(entitySubQuery.EntityQuery.EntityKey);

            var ids = entityLocator.GetUniqueIds(subjectRows, subQueryEntityType.PrimaryKey.TermKey);

            if (ids.Count() > 0)
            {
                Parallel.ForEach(ids, id =>
                {
                    var payload = DeserializeEntity(entitySubQuery.EntityQuery, subQueryEntityType, subjectRows,id);

                    payloads.Add(payload);
                });
            }

            return payloads;
        }
    }
}
