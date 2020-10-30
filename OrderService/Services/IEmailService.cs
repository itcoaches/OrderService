using OrderService.Models;

namespace OrderService.Services
{
    public interface IEmailService
    {
        void SendEmail(Order order);
    }
}
