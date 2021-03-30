using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public interface IDataTableBuilder
    {
        DataTable Build(BulkEntityMutation bulkEntityMutation, out string tableName);
    }

    [EqlService(typeof(IDataTableBuilder))]
    public class DataTableBuilder: IDataTableBuilder
    {
        private EntitySchema entitySchema;
        
        private ISqlServerDataTypeMap sqlServerDataTypeMap;

        public DataTableBuilder(
            EntitySchema entitySchema,
            ISqlServerDataTypeMap sqlServerDataTypeMap
            )
        {
            this.entitySchema = entitySchema;
            this.sqlServerDataTypeMap = sqlServerDataTypeMap;
        }

        public DataTable Build(BulkEntityMutation bulkEntityMutation, out string tableName)
        {
            EntityType entityType = entitySchema.GetEntityType(bulkEntityMutation.EntityKey);

            tableName = entityType.DatabaseName;

            var entityFields = GetEntityFields(entityType);
            
            DataTable dataTable = DefineTable(entityFields);

            foreach (var record in bulkEntityMutation.Records)
            {
                DataRow dataRow = dataTable.NewRow();

                foreach (EntityField entityField in entityFields)
                {
                    var value = GetValue(record, entityField);

                    dataRow[entityField.ColumnName.UnquoteName()] = value;
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private dynamic GetValue(Dictionary<string, dynamic> record, EntityField entityField) =>
            record.ContainsKey(entityField.TermKey) ? record[entityField.TermKey] : DBNull.Value;

        private DataTable DefineTable(List<EntityField> entityFields)
        {
            DataTable dataTable = new DataTable();

            foreach (var entityField in entityFields)
            { 
                Type csharpDataType = sqlServerDataTypeMap.Elements[entityField.DataType];

                dataTable.Columns.Add(entityField.ColumnName.UnquoteName(), csharpDataType);
            }

            return dataTable;
        }

        private List<EntityField> GetEntityFields(EntityType entityType) =>
             entityType
            .EntityFields
            .Where(entityField => !entityField.IsIdentity)
            .ToList();
    }
}
