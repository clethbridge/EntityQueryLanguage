using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Deserialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class DataRowDeserializerTests
    {
        private DataRowDeserializer sut;

        private EntitySchema entitySchema;

        public DataRowDeserializerTests()
        {
            entitySchema = MockSchema();

            sut = new DataRowDeserializer(entitySchema);
        }

        [Theory(DisplayName = "Is able to deserialize a DataRow into an ExpandoObject")]
        [InlineData("Id", 1)]
        [InlineData("Name", "John Smith")]
        //[InlineData("CreatedOn", 1)] can't use DateTime since not compile time constant
        [InlineData("IsActive", true)]
        [InlineData("Amount", 100.55)]
        public void Deserialize(string propertyName, object expectedValue )
        {
            DataTable dataTable = MockDataTable();

            EntityType entityType = entitySchema.EntityTypes.First();
            List<string> termKeys = entityType.EntityFields.Select(f => f.TermKey).ToList();
            ExpandoObject model = sut.Deserialize(dataTable.Rows[0], entityType, termKeys);

            Assert.Equal(expectedValue, ((IDictionary<string, dynamic>)model)[propertyName]);
        }

        private DataTable MockDataTable()
        {
            DataTable dataTable = new DataTable();
            
            dataTable.Columns.Add("t-0001", typeof(int));
            dataTable.Columns.Add("t-0002", typeof(string));
            dataTable.Columns.Add("t-0003", typeof(DateTime));
            dataTable.Columns.Add("t-0004", typeof(bool));
            dataTable.Columns.Add("t-0005", typeof(double));

            DataRow dataRow = dataTable.NewRow();

            dataRow["t-0001"] = 1;
            dataRow["t-0002"] = "John Smith";
            dataRow["t-0003"] = new DateTime(2000, 1, 1);
            dataRow["t-0004"] = true;
            dataRow["t-0005"] = 100.55;

            dataTable.Rows.Add(dataRow);

            return dataTable;
        }

        private EntitySchema MockSchema()
        {
            return new EntitySchema()
            {
                EntityTypes = new List<EntityType>()
                {
                    new EntityType()
                    {
                        EntityKey = "ek-0001",
                        EntityFields = new List<EntityField>()
                        {
                            new EntityField(){ TermKey = "t-0001", ColumnName = "[Id]"},
                            new EntityField(){ TermKey = "t-0002", ColumnName = "[Name]"},
                            new EntityField(){ TermKey = "t-0003", ColumnName = "[CreatedOn]"},
                            new EntityField(){ TermKey = "t-0004", ColumnName = "[IsActive]"},
                            new EntityField(){ TermKey = "t-0005", ColumnName = "[Amount]"}
                        }
                    }
                }
            };
        }
    }
}
