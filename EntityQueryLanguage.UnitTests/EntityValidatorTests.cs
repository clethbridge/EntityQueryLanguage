using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class EntityValidatorTests
    {
        private EntityValidator sut;

        public EntityValidatorTests()
        {
            EntitySchema entitySchema = MockSchema();

            sut = new EntityValidator(entitySchema);
        }

        [Fact(DisplayName = "'Validate' returns true when the entity key exists and all term keys are defined for that entity.")]
        public void Valid()
        {
            string entityKey = "ek-0001";
            var termkeys = new List<string>() { "t-0001", "t-0002"};

            EntityType entityType;
            EqlValidation eqlValidation = sut.Validate(entityKey, termkeys, out entityType);

            Assert.True(eqlValidation.IsValid);
        }

        [Fact(DisplayName = "'Validate' returns false when the entity key does not exist.")]
        public void InvalidEntityKey()
        {
            string entityKey = "ek-0002";
            var termkeys = new List<string>() { "t-0001", "t-0002" };

            EntityType entityType;
            EqlValidation eqlValidation = sut.Validate(entityKey, termkeys, out entityType);

            Assert.False(eqlValidation.IsValid);
            Assert.NotEmpty(eqlValidation.Errors);
        }

        [Fact(DisplayName = "'Validate' returns false when a term key is not defined for the subject Entity Type.")]
        public void InvalidTermKey()
        {
            string entityKey = "ek-0001";
            var termkeys = new List<string>() { "t-0001", "t-0002", "t-0003" };

            EntityType entityType;
            EqlValidation eqlValidation = sut.Validate(entityKey, termkeys, out entityType);

            Assert.False(eqlValidation.IsValid);
            Assert.NotEmpty(eqlValidation.Errors);
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
                        EntityFields = new List<EntityField>()
                        {
                            new EntityField(){ TermKey = "t-0001"},
                            new EntityField(){ TermKey = "t-0002"}
                        }
                    }
                }
            };
        }
    }
}
