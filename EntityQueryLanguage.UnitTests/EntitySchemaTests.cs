using EntityQueryLanguage.Components.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class EntitySchemaTests
    {
        [Fact(DisplayName = "An EntitySchema has an empty EntityType list by default")]
        public void InitEntityTypes()
        {
            EntitySchema entitySchema = new EntitySchema();

            Assert.NotNull(entitySchema.EntityTypes);
            Assert.Empty(entitySchema.EntityTypes);
        }

        [Fact(DisplayName = "An EntitySchema has an empty Lookup list by default")]
        public void InitLookups()
        {
            EntitySchema entitySchema = new EntitySchema();

            Assert.NotNull(entitySchema.Lookups);
            Assert.Empty(entitySchema.Lookups);
        }

        [Fact(DisplayName = "'GetEntityType' returns an EntityType when the entity key exists")]
        public void GetEntityTypeValid()
        {
            string entityKey = "ek-0001";

            EntitySchema entitySchema = new EntitySchema()
            { 
                EntityTypes = new List<EntityType>()
                { 
                    new EntityType(){ EntityKey = entityKey }
                }
            };

            EntityType entityType = entitySchema.GetEntityType(entityKey);

            Assert.NotNull(entityType);
            Assert.Equal(entityKey, entityType.EntityKey);
        }

        [Fact(DisplayName = "'GetEntityType' returns null when the entity key does not exist")]
        public void GetEntityTypeInvalid()
        {
            EntitySchema entitySchema = new EntitySchema();

            EntityType entityType = entitySchema.GetEntityType("ek-001");

            Assert.Null(entityType);
        }

        [Fact(DisplayName = "Can decode a lookup based off of term key and code")]
        public void CanDecodeLookup()
        {
            EntitySchema entitySchema = MockSchema();

            string value = entitySchema.DecodeLookup("t-0001", 2);

            Assert.Equal("Value 2", value);
        }

        [Fact(DisplayName = "Can encode a lookup based off of term key and value")]
        public void CanEncodeLookup()
        {
            EntitySchema entitySchema = MockSchema();

            int code = entitySchema.EncodeLookup("t-0001", "Value 2");

            Assert.Equal(2, code);
        }

        private EntitySchema MockSchema()
        { 
            return new EntitySchema()
            {
                Lookups = new List<EntityLookup>()
                    {
                        new EntityLookup(){ TermKey = "t-0001", Code = 1, Value = "Value 1"},
                        new EntityLookup(){ TermKey = "t-0001", Code = 2, Value = "Value 2"}
                    }
            };
        }
    }
}
