using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OrderService.Services
{
    public class OrderHandlingService : IOrderHandlingService
    {
        private readonly IPackingSlipService _packingSlipService;

        public OrderHandlingService(IPackingSlipService packingSlipService)
        {
            _packingSlipService = packingSlipService;
        }

        public void PlaceOrder(Order order)
        {
            if (order.IsPhysical)
                _packingSlipService.GeneratePackingSlip(order);
        }
    }
}
