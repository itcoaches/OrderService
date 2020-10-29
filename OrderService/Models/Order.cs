using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public enum ProductType
    {
        Unspecified = 0,
        Book = 1,
        Video = 2,

        Membership = 100,
        MembershipUpgrade = 101,
    }

    public class Order
    {
        public int Id { get; set; }
        public ProductType ProductType { get; set; }
        public bool IsPhysical { get { return (int)this.ProductType < 99; } }   // TODO: a bit smelly ?
        public string ProductName { get; set; }
        public double Price { get; set; }
        
        public string ShippingAddress { get; set; }
    }
}
