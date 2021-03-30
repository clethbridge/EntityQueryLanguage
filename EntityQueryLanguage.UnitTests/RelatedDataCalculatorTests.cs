using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class RelatedDataCalculatorTests
    {
        [Fact(DisplayName = "If the primary key exists within another entity, the two entities are related.")]
        public void Related()
        {
            EntitySchema entitySchema = MockRelatedSchema();
            IRelatedDataCalculator sut = new RelatedDataCalculator(entitySchema);

            var relations = sut.Calculate("ek-0001");

            Assert.Single(relations);
            Assert.Contains(relations, r => r == "ek-0002");
        }

        [Fact(DisplayName = "If the primary key does not exist within another entity, the two entities are not related.")]
        public void NotRelated()
        {
            EntitySchema entitySchema = MockNonRelatedSchema();
            IRelatedDataCalculator sut = new RelatedDataCalculator(entitySchema);

            var relations = sut.Calculate("ek-0001");

            Assert.Empty(relations);
        }

        private EntitySchema MockRelatedSchema()
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
                            new EntityField(){ TermKey = "ek-0001|pk", IsPrimaryKey = true },
                            new EntityField(){ TermKey = "ek-0002|pk" }
                        }
                    },
                    new EntityType()
                    {
                        EntityKey = "ek-0002",
                        EntityFields = new List<EntityField>()
                        {
                            new EntityField(){ TermKey = "ek-0002|pk", IsPrimaryKey = true  }
                        }
                    }
                }
            };
        }

        private EntitySchema MockNonRelatedSchema()
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
                            new EntityField(){ TermKey = "ek-0001|pk" }
                        }
                    },
                    new EntityType()
                    {
                        EntityKey = "ek-0002",
                        EntityFields = new List<EntityField>()
                        {
                            new EntityField(){ TermKey = "ek-0002|pk" }
                        }
                    }
                }
            };
        }
    }
}
