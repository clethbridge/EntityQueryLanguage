using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Components.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class DataTableBuilderTests
    {
        private DataTableBuilder sut;

        public DataTableBuilderTests()
        {
            EntitySchema entitySchema = MockSchema();

            sut = new DataTableBuilder(
                entitySchema,
                new SqlServerDataTypeMap()
            );
        }

        [Fact(DisplayName = "Able to build bulk insert data table from entity mutation")]
        public void Fact()
        {
            BulkEntityMutation bulkEntityMutation = new BulkEntityMutation()
            {
                EntityKey = "ek-0001",
                Records = Enumerable.Range(1, 10).Select(i => 
                {
                    return new Dictionary<string, dynamic>()
                    {
                        { "t-0001", $"Name {i}"},
                        { "t-0002", DateTime.Now.AddDays(i)}
                    };
                })
                .ToList()
            };

            string name;
            DataTable result = sut.Build(bulkEntityMutation, out name);

            Assert.NotNull(result);
            Assert.Equal(10, result.Rows.Count);
            Assert.Equal(2, result.Columns.Count);
        }

        private static EntitySchema MockSchema()
        {
            return new EntitySchema()
            {
                EntityTypes = new List<EntityType>()
                {
                    new EntityType()
                    {
                        EntityKey = "ek-0001",
                        DatabaseName = "[shop].[Customers]",
                        EntityFields = new List<EntityField>()
                        {
                            new EntityField(){ TermKey = "ek-0001|pk", ColumnName = "[Id]", DataType = "int", IsIdentity = true },
                            new EntityField(){ TermKey = "t-0001", ColumnName = "[Name]", DataType = "varchar" },
                            new EntityField(){ TermKey = "t-0002", ColumnName = "[CreatedOn]", DataType = "datetime" }
                        }
                    }
                }
            };
        }
    }
}
