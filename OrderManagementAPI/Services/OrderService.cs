using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Data;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderManagementContext _context;

        public OrderService(OrderManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapToOrderDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerEmailAsync(string email)
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Customer!.Email == email)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapToOrderDto);
        }

        public async Task<OrderDto?> InsertOrderAsync(CreateOrderDto createOrderDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get or create customer
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == createOrderDto.CustomerEmail);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        Name = createOrderDto.CustomerName,
                        Email = createOrderDto.CustomerEmail
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                // Get or create product
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == createOrderDto.ProductName);

                if (product == null)
                {
                    product = new Product
                    {
                        ProductName = createOrderDto.ProductName
                    };
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                }

                // Create order
                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = createOrderDto.OrderDate
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order item
                var orderItem = new OrderItem
                {
                    OrderID = order.OrderID,
                    ProductID = product.ProductID,
                    Quantity = createOrderDto.Quantity
                };
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Return the created order
                var createdOrder = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstAsync(o => o.OrderID == order.OrderID);

                return MapToOrderDto(createdOrder);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task MigrateDataAsync()
        {
            var customerOrders = await _context.CustomerOrders.ToListAsync();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Migrate customers
                var uniqueCustomers = customerOrders
                    .GroupBy(co => co.CustomerEmail)
                    .Select(g => g.First())
                    .ToList();

                foreach (var customerOrder in uniqueCustomers)
                {
                    var existingCustomer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Email == customerOrder.CustomerEmail);

                    if (existingCustomer == null)
                    {
                        var customer = new Customer
                        {
                            Name = customerOrder.CustomerName,
                            Email = customerOrder.CustomerEmail
                        };
                        _context.Customers.Add(customer);
                    }
                }
                await _context.SaveChangesAsync();

                // Migrate products
                var uniqueProducts = customerOrders
                    .GroupBy(co => co.ProductName)
                    .Select(g => g.First())
                    .ToList();

                foreach (var customerOrder in uniqueProducts)
                {
                    var existingProduct = await _context.Products
                        .FirstOrDefaultAsync(p => p.ProductName == customerOrder.ProductName);

                    if (existingProduct == null)
                    {
                        var product = new Product
                        {
                            ProductName = customerOrder.ProductName
                        };
                        _context.Products.Add(product);
                    }
                }
                await _context.SaveChangesAsync();

                // Migrate orders and order items
                foreach (var customerOrder in customerOrders)
                {
                    var customer = await _context.Customers
                        .FirstAsync(c => c.Email == customerOrder.CustomerEmail);
                    var product = await _context.Products
                        .FirstAsync(p => p.ProductName == customerOrder.ProductName);

                    var existingOrder = await _context.Orders
                        .FirstOrDefaultAsync(o => o.OrderID == customerOrder.OrderID);

                    if (existingOrder == null)
                    {
                        var order = new Order
                        {
                            OrderID = customerOrder.OrderID,
                            CustomerID = customer.CustomerID,
                            OrderDate = customerOrder.OrderDate
                        };
                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();

                        var orderItem = new OrderItem
                        {
                            OrderID = order.OrderID,
                            ProductID = product.ProductID,
                            Quantity = customerOrder.Quantity
                        };
                        _context.OrderItems.Add(orderItem);
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                OrderID = order.OrderID,
                CustomerName = order.Customer?.Name ?? string.Empty,
                CustomerEmail = order.Customer?.Email ?? string.Empty,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product?.ProductName ?? string.Empty,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }
    }
}