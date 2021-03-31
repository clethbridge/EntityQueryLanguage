using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Parsers;
using EntityQueryLanguage.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class RelatedEntityParserTests
    {
        private IJoinParser sut;

        public RelatedEntityParserTests()
        {
            EntitySchema entitySchema = SchemaProvider.GetSchema();

            sut = new JoinParser(entitySchema);
        }

        [Fact(DisplayName = "Returns an empty list when there are no sub queries")]
        public void NoSubQueries()
        {
            EntityQuery entityQuery = new EntityQuery();

            var tokens = sut.Parse(entityQuery);

            Assert.Empty(tokens);
        }

        [Fact(DisplayName = "Returns a valid token when an entity query has a collection sub query")]
        public void Collection()
        {
            EntityQuery entityQuery = new EntityQuery()
            { 
                EntityKey = "ek-0001",
                Collections = new List<EntitySubQuery>()
                { 
                    new EntitySubQuery()
                    { 
                        Name = "orders",
                        EntityQuery = new EntityQuery()
                        { 
                            EntityKey = "ek-0003"
                        }
                    }
                }
            };

            var tokens = sut.Parse(entityQuery);

            Assert.Single(tokens);
            Assert.Contains(tokens, 
                t => t == $"LEFT JOIN [shop].[Orders] ON [shop].[Orders].[CustomerId] = [shop].[Customers].[Id]"
            );
        }

        [Fact(DisplayName = "Returns a valid token when an entity query has a projection sub query")]
        public void Projection()
        {
            EntityQuery entityQuery = new EntityQuery()
            {
                EntityKey = "ek-0002",
                Projections = new List<EntitySubQuery>()
                {
                    new EntitySubQuery()
                    {
                        Name = "order",
                        EntityQuery = new EntityQuery()
                        {
                            EntityKey = "ek-0003"
                        }
                    }
                }
            };

            var tokens = sut.Parse(entityQuery);

            Assert.Single(tokens);
            Assert.Contains(tokens,
                t => t == $"LEFT JOIN [shop].[Orders] ON [shop].[Orders].[ProductId] = [shop].[Products].[Id]"
            );
        }

        [Fact(DisplayName = "Returns a valid token when an entity query joins from the context of a child to the parent")]
        public void InvertedProjection()
        {
            EntityQuery entityQuery = new EntityQuery()
            {
                EntityKey = "ek-0003",
                Projections = new List<EntitySubQuery>()
                {
                    new EntitySubQuery()
                    {
                        Name = "product",
                        EntityQuery = new EntityQuery()
                        {
                            EntityKey = "ek-0002"
                        }
                    }
                }
            };

            var tokens = sut.Parse(entityQuery);

            Assert.Single(tokens);
            Assert.Contains(tokens,
                t => t == $"LEFT JOIN [shop].[Products] ON [shop].[Products].[Id] = [shop].[Orders].[ProductId]"
            );
        }

        [Fact(DisplayName = "Returns multiple tokens when a projection is nested within a collection")]
        public void RecursiveTest()
        {
            EntityQuery entityQuery = new EntityQuery()
            {
                EntityKey = "ek-0001",
                Collections = new List<EntitySubQuery>()
                { 
                    new EntitySubQuery()
                    { 
                        Name = "orders",
                        EntityQuery = new EntityQuery()
                        { 
                            EntityKey = "ek-0003",
                            Projections = new List<EntitySubQuery>()
                            { 
                                new EntitySubQuery()
                                { 
                                    Name = "product",
                                    EntityQuery = new EntityQuery()
                                    { 
                                        EntityKey = "ek-0002"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var tokens = sut.Parse(entityQuery);

            Assert.Equal(2, tokens.Count);
        }
    }
}
