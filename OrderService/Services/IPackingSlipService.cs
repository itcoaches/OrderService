using OrderService.Models;

namespace OrderService.Services
{
    public interface IPackingSlipService
    {
        void GeneratePackingSlip(Order order, string shippingAddress);
    }
}
