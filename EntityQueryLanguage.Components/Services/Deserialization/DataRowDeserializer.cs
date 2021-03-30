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
    public interface IDataRowDeserializer
    {
        ExpandoObject Deserialize(DataRow dataRow, EntityType entityType, List<string> termKeys);
    }

    [EqlService(typeof(IDataRowDeserializer))]
    public class DataRowDeserializer: IDataRowDeserializer
    {
        private EntitySchema entitySchema;

        public DataRowDeserializer(EntitySchema entitySchema)
        {
            this.entitySchema = entitySchema;
        }

        public ExpandoObject Deserialize(DataRow dataRow, EntityType entityType, List<string> termKeys)
        {
            ExpandoObject model = new ExpandoObject();

            foreach (string termKey in termKeys)
            {
                var value = dataRow[termKey];

                var propertyName = entityType.GetEntityField(termKey).PropertyName;

                if (value != DBNull.Value)
                {
                    model.AddField(propertyName, value);
                }
            }

            return model;
        }
    }
}
