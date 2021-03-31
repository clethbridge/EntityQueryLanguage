using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Components.Services.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class SelectStatementBuilderTests
    {
        private SelectStatementBuilder sut;

        public SelectStatementBuilderTests()
        {
            EntitySchema entitySchema = MockSchema(); 

            sut = new SelectStatementBuilder(
                entitySchema, 
                new JoinParser(entitySchema),
                new ColumnParser(entitySchema, new ColumnBuilder()));
        }

        [Fact(DisplayName = "'Build' derives a valid sql statement")]
        public void Build()
        {
            EntityQuery entityQuery = new EntityQuery()
            { 
                EntityKey = "ek-0001",
                TermKeys = new List<string>() { "t-0001", "t-0002", "t-0003"}
            };

            SqlStatement sql = sut.Build(entityQuery);

            string expected = $"SELECT\r\n\t [t-0001] = [dbo].[Customer].[Id]\r\n\t,[t-0002] = [dbo].[Customer].[Name]\r\n\t,[t-0003] = [dbo].[Customer].[Summary]\r\nFROM [dbo].[Customer]";
            Assert.Equal(expected, sql.Sql);
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
                        DatabaseName = "[dbo].[Customer]",
                        EntityFields = new List<EntityField>()
                        {
                            new EntityField(){ TermKey = "t-0001", ColumnName = "[Id]", IsPrimaryKey = true},
                            new EntityField(){ TermKey = "t-0002", ColumnName = "[Name]"},
                            new EntityField(){ TermKey = "t-0003", ColumnName = "[Summary]"}
                        }
                    }
                }
            };
        }
    }
}
