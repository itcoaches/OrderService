using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrderHandlingService
    {
        void PlaceOrder(Order order);
    }
}
