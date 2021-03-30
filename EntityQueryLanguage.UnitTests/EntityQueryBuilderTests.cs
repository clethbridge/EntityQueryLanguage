using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class EntityQueryBuilderTests
    {
        private Dictionary<OperatorType, Expression<Func<Customer, bool>>> predicateLookup;

        public EntityQueryBuilderTests()
        {
            predicateLookup = new Dictionary<OperatorType, Expression<Func<Customer, bool>>>()
            {
                { OperatorType.Equals, c => c.Id == 1 },
                { OperatorType.DoesNotEqual, c => c.Id != 1 },
                { OperatorType.GreaterThan, c => c.Id > 1 },
                { OperatorType.GreaterThanOrEqualTo, c => c.Id >= 1 },
                { OperatorType.LessThan, c => c.Id < 1 },
                { OperatorType.LessThanOrEqualTo, c => c.Id <= 1 }
            };
        }

        [Fact(DisplayName = "Initializes entity key based off of generic type")]
        public void InitializesEntityKey()
        {
            EntityQuery entityQuery;

            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            { 
                entityQuery = 
                     entityQueryBuilder
                    .ToQuery();
            }

            Assert.NotNull(entityQuery);
            Assert.Equal("ek-0001", entityQuery.EntityKey);
        }

        [Fact(DisplayName = "Can select all terms for a given type")]
        public void CanSelectAllTerms()
        {
            EntityQuery entityQuery;

            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                entityQuery =
                     entityQueryBuilder
                    .SelectAll()
                    .ToQuery();
            }

            Assert.NotNull(entityQuery);
            Assert.NotEmpty(entityQuery.TermKeys);
            Assert.Equal(3, entityQuery.TermKeys.Count);
        }

        [Fact(DisplayName = "Can select specific terms for a given type")]
        public void CanSelectSpecificTerms()
        {
            EntityQuery entityQuery;

            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                entityQuery =
                     entityQueryBuilder
                    .Select
                    (
                         c => c.Id,
                         c => c.Name
                    )
                    .ToQuery();
            }

            Assert.NotNull(entityQuery);
            Assert.NotEmpty(entityQuery.TermKeys);
            Assert.Equal(2, entityQuery.TermKeys.Count);
            Assert.DoesNotContain(entityQuery.TermKeys, t => string.IsNullOrEmpty(t));
            Assert.Equal("ek-0001|pk", entityQuery.TermKeys[0]);
            Assert.Equal("t-0001", entityQuery.TermKeys[1]);
        }

        [Theory(DisplayName = "Can add a filter to the query with based on a binary expression")]
        [InlineData(OperatorType.Equals)]
        [InlineData(OperatorType.DoesNotEqual)]
        [InlineData(OperatorType.GreaterThan)]
        [InlineData(OperatorType.GreaterThanOrEqualTo)]
        [InlineData(OperatorType.LessThan)]
        [InlineData(OperatorType.LessThanOrEqualTo)]
        public void CanFilterByBinaryExpression(OperatorType operatorType)
        {
            EntityQuery entityQuery;
            Expression<Func<Customer, bool>> predicate = predicateLookup[operatorType];

            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                entityQuery = 
                     entityQueryBuilder
                    .Where(predicate)
                    .ToQuery();
            }

            EntityFieldFilter entityFieldFilter = entityQuery.Filter.FieldFilters.First();
            
            Assert.NotNull(entityQuery);
            Assert.Single(entityQuery.Filter.FieldFilters);
            Assert.Equal("ek-0001|pk", entityFieldFilter.TermKey);
            Assert.Equal(operatorType, entityFieldFilter.Operator);
            Assert.Equal(1, entityFieldFilter.Value);
        }

        [Fact(DisplayName = "Can add a filter to the query with a variable as the value")]
        public void CanFilterEqualsVariable()
        {
            EntityQuery entityQuery;
            int id = 1;

            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                entityQuery =
                     entityQueryBuilder
                    .Where(c => c.Id == id)
                    .ToQuery();
            }

            EntityFieldFilter entityFieldFilter = entityQuery.Filter.FieldFilters.First();

            Assert.NotNull(entityQuery);
            Assert.Single(entityQuery.Filter.FieldFilters);
            Assert.Equal("ek-0001|pk", entityFieldFilter.TermKey);
            Assert.Equal(OperatorType.Equals, entityFieldFilter.Operator);
            Assert.Equal(id, entityFieldFilter.Value);
        }

        [Theory(DisplayName = "Sets the specified conjunction type")]
        [InlineData(ConjunctionType.Or)]
        [InlineData(ConjunctionType.And)]
        public void CanSetConjunctionType(ConjunctionType conjunctionType)
        {
            EntityQuery entityQuery;
            
            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                entityQuery = 
                     entityQueryBuilder
                    .SetEntityFilterConjunctionType(conjunctionType)
                    .ToQuery();
            }

            Assert.Equal(conjunctionType, entityQuery.Filter.Conjunction);
        }

        [Fact(DisplayName = "Can add a field to sort by (Ascending)")]
        public void CanSortAscending()
        {
            EntityQuery entityQuery;

            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                entityQuery =
                     entityQueryBuilder
                    .OrderBy(c => c.Name)
                    .ToQuery();
            }

            EntitySort entitySort = entityQuery.Sortings.First();
            Assert.Single(entityQuery.Sortings);
            Assert.Equal(EntitySortType.Ascending,entitySort.Type);
            Assert.Equal("t-0001", entitySort.TermKey);
        }

        [Fact(DisplayName = "Can add a field to sort by (Descending)")]
        public void CanSortDescending()
        {
            EntityQuery entityQuery;

            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                entityQuery =
                     entityQueryBuilder
                    .OrderByDescending(c => c.Name)
                    .ToQuery();
            }

            EntitySort entitySort = entityQuery.Sortings.First();
            Assert.Single(entityQuery.Sortings);
            Assert.Equal(EntitySortType.Descending, entitySort.Type);
            Assert.Equal("t-0001", entitySort.TermKey);
        }

        [Fact(DisplayName = "Can add a projection to the subject entity query")]
        public void CanProject()
        {
            EntityQuery ordersQuery;
            using (EntityQueryBuilder<Order> orderBuilder = new EntityQueryBuilder<Order>())
            {
                ordersQuery = orderBuilder.SelectAll().ToQuery();
            }

            EntityQuery customersQuery;
            using (EntityQueryBuilder<Customer> customerBuilder = new EntityQueryBuilder<Customer>())
            {
                customersQuery = 
                     customerBuilder
                    .SelectAll()
                    .Project("orders", ordersQuery)
                    .ToQuery();
            }

            Assert.Single(customersQuery.Projections);
            Assert.Equal("orders", customersQuery.Projections[0].Name);
        }
        
        [Fact(DisplayName = "Can add a collection to the subject entity query")]
        public void CanCollect()
        {
            EntityQuery ordersQuery;
            using (EntityQueryBuilder<Order> orderBuilder = new EntityQueryBuilder<Order>())
            {
                ordersQuery = orderBuilder.SelectAll().ToQuery();
            }

            EntityQuery customersQuery;
            using (EntityQueryBuilder<Customer> customerBuilder = new EntityQueryBuilder<Customer>())
            {
                customersQuery =
                     customerBuilder
                    .SelectAll()
                    .Collect("orders", ordersQuery)
                    .ToQuery();
            }

            Assert.Single(customersQuery.Collections);
            Assert.Equal("orders", customersQuery.Collections[0].Name);
        }
    }
}