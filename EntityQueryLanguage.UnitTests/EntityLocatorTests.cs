using EntityQueryLanguage.Components.Services.Deserialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class EntityLocatorTests
    {
        private IEntityLocator sut;

        public EntityLocatorTests()
        {
            sut = new EntityLocator();
        }

        [Fact(DisplayName = "Can filter table rows by id")]
        public void CanFilterById()
        {
            var rows = ArrangeWithIntId();

            var filtered = sut.FilterById(rows, "Id", 1);

            var ids = filtered.Select(row => row.Field<int>("Id") == 1).Distinct();

            Assert.Single(ids);
            Assert.True(ids.First());
        }

        [Fact(DisplayName = "Can determine unique ids within a column")]
        public void CanGetIds()
        {
            var rows = ArrangeWithIntId();

            var ids = sut.GetUniqueIds(rows, "Id");

            Assert.Equal(9, ids.Count());
        }

        private List<DataRow> ArrangeWithIntId()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));

            for (int i = 1; i <= 100; i++)
            {
                DataRow row = table.NewRow();

                int initial = (i % 10);

                if (initial == 9)
                {
                    row["Id"] = DBNull.Value;
                }
                else
                {
                    row["Id"] = (int?)initial + 1;
                }

                row["Name"] = $"Row {i}";

                table.Rows.Add(row);
            }

            return table.Rows.Cast<DataRow>().ToList();
        }
    }
}
