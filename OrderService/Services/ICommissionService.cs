using OrderService.Models;

namespace OrderService.Services
{
    public interface ICommissionService
    {
        void CreateCommissionPayment(Order order);
    }
}
