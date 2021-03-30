using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Components.Services.Executors;
using EntityQueryLanguage.Components.Services.Factories;
using EntityQueryLanguage.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace EntityQueryLanguage.IntegrationTests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class CRUD_Tests: BaseIntegrationTest
    {
        private IEntityQueryExecutor entityQueryExecutor;

        private IEntityMutationExecutor entityMutationExecutor;

        private IEntityMutationBuilderFactory entityMutationBuilderFactory; 

        public CRUD_Tests(
            IEntityQueryExecutor entityQueryExecutor,
            IEntityMutationExecutor entityMutationExecutor,
            IEntityMutationBuilderFactory entityMutationBuilderFactory)
        {
            this.entityQueryExecutor = entityQueryExecutor;
            this.entityMutationExecutor = entityMutationExecutor;
            this.entityMutationBuilderFactory = entityMutationBuilderFactory;
        }

        [Fact(DisplayName = "Can create a customer"), Priority(1)]
        public async Task Create()
        {
            EntityMutation addCustomer = BuildAddCustomerMutation(new Customer()
            {
                Name = "John Smith"
            });

            ExpandoObject addedCustomer = await entityMutationExecutor.ExecuteAsync(addCustomer);
            var asDictionary = (IDictionary<string, dynamic>)addedCustomer;
            
            Assert.NotNull(asDictionary["Id"]);
            Assert.Equal("John Smith", asDictionary["Name"]);
            Assert.NotNull(asDictionary["CreatedOn"]);
        }

        [Fact(DisplayName = "Can return a list of customers"), Priority(2)]
        public async Task Read()
        {
            EntityQuery entityQuery;
            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                entityQuery =
                     entityQueryBuilder
                    .SelectAll()
                    .ToQuery();
            }
            
            List<ExpandoObject> customers = await entityQueryExecutor.ExecuteAsync(entityQuery);

            Assert.NotEmpty(customers);
        }

        [Fact(DisplayName = "Can update a customer"), Priority(3)]
        public async Task Update()
        {
            int customerId = await GetCustomerIdAsync();
            EntityMutation updateCustomer = new EntityMutation()
            {
                EntityId = customerId,
                EntityKey = "ek-0001",
                Type = EntityMutationType.Update,
                Fields = new Dictionary<string, dynamic>()
                {
                    { "t-0001", "Jane Doe"}
                },
                Return = new EntityQuery()
                {
                    EntityKey = "ek-0001",
                    TermKeys = new List<string>() { "ek-0001|pk", "t-0001", "t-0002" }
                }
            };

            List<ExpandoObject> updatedCustomerResult = await entityMutationExecutor.ExecuteAsync(updateCustomer);
            var asDictionary = (IDictionary<string, dynamic>)updatedCustomerResult.First();

            Assert.NotNull(asDictionary["Id"]);
            Assert.Equal("Jane Doe", asDictionary["Name"]);
            Assert.NotNull(asDictionary["CreatedOn"]);
        }

        [Fact(DisplayName = "Can delete a customer"), Priority(4)]
        public async Task Delete()
        {
            int customerId = await GetCustomerIdAsync();
            EntityMutation deleteCustomer = new EntityMutation()
            {
                EntityId = customerId,
                EntityKey = "ek-0001",
                Type = EntityMutationType.Delete,
                Return = new EntityQuery()
                {
                    EntityKey = "ek-0001",
                    TermKeys = new List<string>() { "ek-0001|pk" }
                }
            };

            List<ExpandoObject> deleteCustomerResult = await entityMutationExecutor.ExecuteAsync(deleteCustomer);
            List<int> remainingIds = deleteCustomerResult.Select(x => (int)((IDictionary<string, dynamic>)x)["Id"]).ToList();

            Assert.DoesNotContain(customerId, remainingIds);
        }

        [Fact(DisplayName = "Can bulk insert many products"), Priority(5)]
        public async Task CanBulkInsertProducts()
        {
            BulkEntityMutation bulkEntityMutation = new BulkEntityMutation()
            {
                EntityKey = "ek-0002",
                Type = BulkEntityMutationType.BulkInsert,
                Records = Enumerable.Range(1, 10).Select(i =>
                {
                    return new Dictionary<string, dynamic>()
                    {
                        { "t-0003", $"Product {i}"},
                        { "t-0004", $"This is  a summary for product {i}."},
                        { "t-0005", 100.00m + Convert.ToDecimal(i) },
                        { "t-0006", i % 3 }
                    };
                })
                .ToList()
            };

            Task bulkInsert = entityMutationExecutor.BulkInsertAsync(bulkEntityMutation);
            await bulkInsert;

            Assert.True(bulkInsert.IsCompletedSuccessfully);
        }

        [Fact(DisplayName = "Can bulk insert many customers"), Priority(6)]
        public async Task CanBulkInsertCustomers()
        {
            BulkEntityMutation bulkEntityMutation = new BulkEntityMutation()
            {
                EntityKey = "ek-0001",
                Type = BulkEntityMutationType.BulkInsert,
                Records = Enumerable.Range(1, 10).Select(i =>
                {
                    return new Dictionary<string, dynamic>()
                    {
                        { "t-0001", $"Customer {i}"},
                        { "t-0002", DateTime.Now.AddDays(i)}
                    };
                })
                .ToList()
            };

            Task bulkInsert = entityMutationExecutor.BulkInsertAsync(bulkEntityMutation);
            await bulkInsert;

            Assert.True(bulkInsert.IsCompletedSuccessfully);
        }

        [Fact(DisplayName = "A Customer can order a product"), Priority(7)]
        public async Task ACustomerCanOrderAProduct()
        {
            List<ExpandoObject> products = await GetProducts();
            List<ExpandoObject> customers = await GetCustomers();
            Order order = new Order()
            { 
                CustomerId = ((IDictionary<string, dynamic>)customers.First())["Id"],
                ProductId = ((IDictionary<string, dynamic>)products.First())["Id"],
                Quantity = 2
            };
            EntityMutation orderProduct = BuildOrderProductMutation(order);

            ExpandoObject orderedProduct = await entityMutationExecutor.ExecuteAsync(orderProduct);
            var asDictionary = (IDictionary<string, dynamic>)orderedProduct;

            Assert.NotNull(asDictionary["Id"]);
            Assert.NotNull(asDictionary["CreatedOn"]);
            Assert.Equal(order.CustomerId, asDictionary["CustomerId"]);
            Assert.Equal(order.ProductId, asDictionary["ProductId"]);
            Assert.Equal(order.Quantity, asDictionary["Quantity"]);
        }

        [Fact(DisplayName = "Can return a customer and their orders"), Priority(8)]
        public async Task CanQueryForChildEntities()
        {
            EntityQuery getCustomersWithOrders = BuildGetCustomersWithOrders();

            List<ExpandoObject> customersWithOrders = await entityQueryExecutor.ExecuteAsync(getCustomersWithOrders);

            Assert.NotNull(customersWithOrders);
        }

        private async Task<int>GetCustomerIdAsync()
        {
            EntityQuery getCustomerIds;
            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                getCustomerIds = 
                     entityQueryBuilder
                    .Select(c => c.Id)
                    .ToQuery();
            }

            List<ExpandoObject> customerIdsResult = await entityQueryExecutor.ExecuteAsync(getCustomerIds);

            int customerId = ((IDictionary<string, dynamic>)customerIdsResult.First())["Id"];

            return customerId;
        }

        private async Task<List<ExpandoObject>> GetProducts()
        {
            EntityQuery getProducts;

            using (EntityQueryBuilder<Product> entityQueryBuilder = new EntityQueryBuilder<Product>())
            {
                getProducts = entityQueryBuilder.SelectAll().ToQuery();
            }

            return await entityQueryExecutor.ExecuteAsync(getProducts);
        }

        private async Task<List<ExpandoObject>> GetCustomers()
        {
            EntityQuery getCustomers;
            
            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                getCustomers = entityQueryBuilder.SelectAll().ToQuery();
            }

            return await entityQueryExecutor.ExecuteAsync(getCustomers);
        }

        private EntityMutation BuildAddCustomerMutation(Customer customer)
        {
            EntityQuery getCustomer;
            using (EntityQueryBuilder<Customer> entityQueryBuilder = new EntityQueryBuilder<Customer>())
            {
                getCustomer =
                     entityQueryBuilder
                    .SelectAll()
                    .ToQuery();
            }

            EntityMutation addCustomer;
            using (IEntityMutationBuilder<Customer> entityMutationBuilder = entityMutationBuilderFactory.Get<Customer>())
            {
                addCustomer =
                     entityMutationBuilder
                    .Insert(customer)
                    .Return(getCustomer)
                    .ToMutation();
            }

            return addCustomer;
        }

        private EntityMutation BuildOrderProductMutation(Order order)
        {
            EntityQuery getProduct;
            using (EntityQueryBuilder<Order> entityQueryBuilder = new EntityQueryBuilder<Order>())
            {
                getProduct = entityQueryBuilder.SelectAll().ToQuery();
            }

            using (IEntityMutationBuilder<Order> entityMutationBuilder = entityMutationBuilderFactory.Get<Order>())
            {
                return
                     entityMutationBuilder
                    .Insert(order)
                    .Return(getProduct)
                    .ToMutation();
            }
        }

        private EntityQuery BuildGetCustomersWithOrders()
        {
            return new EntityQuery()
            {
                EntityKey = "ek-0001",
                TermKeys = new List<string>() { "ek-0001|pk", "t-0001" },
                Collections = new List<EntitySubQuery>()
                {
                    new EntitySubQuery()
                    {
                        Name = "orders",
                        EntityQuery = new EntityQuery()
                        {
                            EntityKey = "ek-0003",
                            TermKeys = new List<string>()
                            {
                                "t-0007", "t-0008"
                            },
                            Projections = new List<EntitySubQuery>()
                            {
                                new EntitySubQuery()
                                {
                                    Name = "product",
                                    EntityQuery = new EntityQuery()
                                    {
                                        EntityKey = "ek-0002",
                                        TermKeys = new List<string>()
                                        {
                                            "t-0003", "t-0004", "t-0005", "t-0006"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
