using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public enum ProductType
    {
        Unspecified,
        Book,
        Video,
        Membership,
        MembershipUpgrade,
    }

    public class Order
    {
        public int Id { get; set; }
        public ProductType ProductType { get; set; }
        public bool IsPhysical { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        
        public string ShippingAddress { get; set; }
    }
}
