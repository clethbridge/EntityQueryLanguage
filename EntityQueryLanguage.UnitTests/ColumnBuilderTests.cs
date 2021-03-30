using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class ColumnBuilderTests
    {
        private ColumnBuilder sut;

        public ColumnBuilderTests()
        {
            sut = new ColumnBuilder();
        }

        [Theory(DisplayName = "'Build' returns a string with the term key as the result column name, and points towards the fully qualified database column")]
        [InlineData(0, "[t-0001]", "[Id]")]
        [InlineData(1, "[t-0002]", "[Name]")]
        public void BuildsColumn(int index, string label, string expectedColumn)
        {
            EntityType entityType = new EntityType()
            {
                EntityKey = "ek-0001",
                DatabaseName = "[dbo].[Customer]",
                EntityFields = new List<EntityField>()
                { 
                    new EntityField(){ TermKey = "t-0001", ColumnName = "[Id]"},
                    new EntityField(){ TermKey = "t-0002", ColumnName = "[Name]"}
                }
            };

            string column = sut.Build(entityType, entityType.EntityFields[index]);

            Assert.Equal($"{label} = [dbo].[Customer].{expectedColumn}", column);
        }
    }
}
