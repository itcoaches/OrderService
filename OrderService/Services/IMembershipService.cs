using OrderService.Models;

namespace OrderService.Services
{
    public interface IMembershipService
    {
        void ActivateMembership(Order order);
        void UpgradeMembership(Order order);
    }
}
