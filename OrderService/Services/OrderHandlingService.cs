using OrderService.Models;

namespace OrderService.Services
{
    public class OrderHandlingService : IOrderHandlingService
    {
        public static string RoyaltyDepartment = "Royalty Department";    // TODO: make this a config setting
        private readonly IPackingSlipService _packingSlipService;

        public OrderHandlingService(IPackingSlipService packingSlipService)
        {
            _packingSlipService = packingSlipService;
        }

        public void PlaceOrder(Order order)
        {
            if (order.IsPhysical)
                _packingSlipService.GeneratePackingSlip(order, order.ShippingAddress);

            if (order.ProductType == ProductType.Book)
                _packingSlipService.GeneratePackingSlip(order, RoyaltyDepartment);


        }
    }
}
