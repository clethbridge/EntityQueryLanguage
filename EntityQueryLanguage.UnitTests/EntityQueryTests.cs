using EntityQueryLanguage.Components.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class EntityQueryTests
    {
        [Fact(DisplayName = "An EntityQuery has an empty TermKey list by default")]
        public void InitTermKeys()
        {
            EntityQuery entityQuery = new EntityQuery();

            Assert.NotNull(entityQuery.TermKeys);
            Assert.Empty(entityQuery.TermKeys);
        }

        [Fact(DisplayName = "An EntityQuery has an empty Projection list by default")]
        public void InitProjections()
        {
            EntityQuery entityQuery = new EntityQuery();

            Assert.NotNull(entityQuery.Projections);
            Assert.Empty(entityQuery.Projections);
        }

        [Fact(DisplayName = "An EntityQuery has an empty Collections list by default")]
        public void InitCollections()
        {
            EntityQuery entityQuery = new EntityQuery();

            Assert.NotNull(entityQuery.Collections);
            Assert.Empty(entityQuery.Collections);
        }

        [Fact(DisplayName = "'Strategy' is set to 'Join' by default")]
        public void StrategyInit()
        {
            EntityQuery entityQuery = new EntityQuery();

            Assert.Equal(QueryStrategy.Join, entityQuery.Strategy);
        }
    }
}
