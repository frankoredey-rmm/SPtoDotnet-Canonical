using OrderManagementAPI.DTOs;

namespace OrderManagementAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerEmailAsync(string email);
        Task<OrderDto?> InsertOrderAsync(CreateOrderDto createOrderDto);
        Task<bool> DeleteOrderByIdAsync(int orderId);
        Task MigrateDataAsync();
    }
}