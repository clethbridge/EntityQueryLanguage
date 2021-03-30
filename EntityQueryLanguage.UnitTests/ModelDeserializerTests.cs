using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Deserialization;
using EntityQueryLanguage.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class ModelDeserializerTests
    {
        private ModelDeserializer sut;

        public ModelDeserializerTests()
        {
            sut = new ModelDeserializer();
        }

        [Fact(DisplayName = "Can deserialize a data row based off of generic type.")]
        public void CanDeserializeToModel()
        {
            DataRow dataRow = MockDataRow();

            CustomerModel customer = sut.Deserialize<CustomerModel>(dataRow);

            Assert.NotNull(customer);
            Assert.Equal(1, customer.Id);
            Assert.Equal("John Smith", customer.Name);
            Assert.Equal(new DateTime(2000, 1, 1), customer.CreatedOn);
            Assert.Equal(100.50m, customer.Savings);
            Assert.True(customer.IsActive);
            Assert.Null(customer.LastUpdated);
        }

        private DataRow MockDataRow()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("ek-0001|pk", typeof(int));
            dataTable.Columns.Add("t-0001", typeof(string));
            dataTable.Columns.Add("t-0002", typeof(DateTime));
            dataTable.Columns.Add("t-0003", typeof(decimal));
            dataTable.Columns.Add("t-0004", typeof(bool));
            dataTable.Columns.Add("t-0005", typeof(DateTime));

            DataRow dataRow = dataTable.NewRow();

            dataRow["ek-0001|pk"] = 1;
            dataRow["t-0001"] = "John Smith";
            dataRow["t-0002"] = new DateTime(2000, 1, 1);
            dataRow["t-0003"] = 100.50m;
            dataRow["t-0004"] = true;
            dataRow["t-0005"] = DBNull.Value;

            return dataRow;
        }

        [EntityKey("ek-0001")]
        public class CustomerModel
        {
            [TermKey("ek-0001|pk")]
            public int Id { get; set; }

            [TermKey("t-0001")]
            public string Name { get; set; }

            [DbDefault]
            [TermKey("t-0002")]
            public DateTime CreatedOn { get; set; }

            [TermKey("t-0003")]
            public decimal Savings { get; set; }

            [TermKey("t-0004")]
            public bool IsActive {get; set;}

            [TermKey("t-0005")]
            public DateTime? LastUpdated { get; set; }
        }
    }
}
