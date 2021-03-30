using EntityQueryLanguage.Components.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class EntityTypeTests
    {
        [Fact(DisplayName = "An EntityType has an empty EntityField list by default")]
        public void Fact()
        {
            EntityType entityType = new EntityType();

            Assert.NotNull(entityType.EntityFields);
            Assert.Empty(entityType.EntityFields);
        }

        [Fact(DisplayName = "'GetEntityField' returns an EntityField when the term key exists")]
        public void GetEntityFieldValid()
        {
            string termKey = "t-0001";

            EntityType entityType = new EntityType()
            {
                EntityFields = new List<EntityField>()
                {
                    new EntityField(){ TermKey = termKey }
                }
            };

            EntityField entityField = entityType.GetEntityField(termKey);

            Assert.NotNull(entityField);
            Assert.Equal(termKey, entityField.TermKey);
        }

        [Fact(DisplayName = "'GetEntityField' returns an EntityField when the term key exists")]
        public void GetEntityFieldInvalid()
        {
            EntityType entityType = new EntityType();

            EntityField entityField = entityType.GetEntityField("t-0001");

            Assert.Null(entityField);
        }
    }
}
