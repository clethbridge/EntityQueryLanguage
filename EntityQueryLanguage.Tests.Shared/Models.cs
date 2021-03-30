using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;

namespace EntityQueryLanguage.Tests.Shared
{
    [EntityKey("ek-0001")]
    public class Customer
    {
        [TermKey("ek-0001|pk")]
        public int Id { get; set; }

        [TermKey("t-0001")]
        public string Name { get; set; }

        [DbDefault]
        [TermKey("t-0002")]
        public DateTime CreatedOn { get; set; }

        [EntityKey("ek-0003")]
        public List<Order> Orders { get; set; } = new List<Order>();
    }

    [EntityKey("ek-0002")]
    public class Product
    {
        [TermKey("ek-0002|pk")]
        public int Id { get; set; }

        [TermKey("t-0003")]
        public string Name { get; set; }

        [TermKey("t-0004")]
        public string Summary { get; set; }

        [TermKey("t-0005")]
        public decimal Price { get; set; }

        [TermKey("t-0006")]
        public ProductType ProductType { get; set; }
    }

    [EntityKey("ek-0003")]
    public class Order
    {
        [TermKey("ek-0003|pk")]
        public int Id { get; set; }

        [TermKey("ek-0001|pk")]
        public int CustomerId { get; set; }

        [TermKey("ek-0002|pk")]
        public int ProductId { get; set; }

        [DbDefault]
        [TermKey("t-0007")]
        public DateTime CreatedOn { get; set; }

        [TermKey("t-0008")]
        public int Quantity { get; set; }
    }

    public enum ProductType
    {
        Bed,
        Bathroom,
        Beyond
    }
}
