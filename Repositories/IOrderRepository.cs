using NewStore.Models;

namespace NewStore.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByCustomerEmailAsync(string email);
        Task<Order> CreateOrderWithItemsAsync(Order order, IEnumerable<OrderItem> orderItems);
    }
}