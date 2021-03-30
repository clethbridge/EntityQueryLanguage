using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class EntityMutationBuilderTests
    {
        private EntitySchema entitySchema;

        public EntityMutationBuilderTests()
        {
            entitySchema = MockSchema();
        }

        [Fact(DisplayName = "Can create an insert entity mutation based off of a model")]
        public void Insert()
        {
            Customer customer = MockCustomer();

            EntityMutation addCustomer;
            using (EntityMutationBuilder<Customer> entityMutationBuilder = new EntityMutationBuilder<Customer>(entitySchema))
            {
                addCustomer = 
                     entityMutationBuilder
                    .Insert(customer)
                    .ToMutation();
            }

            Assert.Equal("ek-0001", addCustomer.EntityKey);
            Assert.NotEmpty(addCustomer.Fields);
            Assert.DoesNotContain(addCustomer.Fields, f => f.Key == "ek-0001|pk");
        }

        [Fact(DisplayName = "Able to specify a return query")]
        public void ReturnQuery()
        {
            Customer customer = MockCustomer();

            EntityMutation addCustomer;
            using (EntityMutationBuilder<Customer> entityMutationBuilder = new EntityMutationBuilder<Customer>(entitySchema))
            {
                addCustomer =
                     entityMutationBuilder
                    .Insert(customer)
                    .Return(new EntityQuery() 
                    { 
                        EntityKey = "ek-0001"
                    })
                    .ToMutation();
            }

            Assert.NotNull(addCustomer.Return);
            Assert.Equal("ek-0001", addCustomer.Return.EntityKey);
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
                            new EntityField(){ TermKey = "ek-0001|pk", IsIdentity = true, IsPrimaryKey = true},
                            new EntityField(){ TermKey = "t-0001" },
                            new EntityField(){ TermKey = "t-0002"}
                        }
                    }
                }
            };
        }

        private Customer MockCustomer()
        {
            return new Customer()
            {
                Name = "Bilbo Baggins",
                CreatedOn = DateTime.Now
            };
        }
    }
}
