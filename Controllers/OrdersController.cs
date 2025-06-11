using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewStore.Data;
using NewStore.DTOs;
using NewStore.Models;

namespace NewStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly NewStoreContext _context;

        public OrdersController(NewStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var orderDTOs = orders.Select(o => new OrderDTO
            {
                OrderID = o.OrderID,
                CustomerName = o.Customer.Name,
                CustomerEmail = o.Customer.Email,
                OrderDate = o.OrderDate,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductName = oi.Product.ProductName,
                    Quantity = oi.Quantity
                }).ToList()
            });

            return Ok(orderDTOs);
        }

        [HttpGet("customer/{email}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByCustomerEmail(string email)
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Customer.Email == email)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderDTOs = orders.Select(o => new OrderDTO
            {
                OrderID = o.OrderID,
                CustomerName = o.Customer.Name,
                CustomerEmail = o.Customer.Email,
                OrderDate = o.OrderDate,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductName = oi.Product.ProductName,
                    Quantity = oi.Quantity
                }).ToList()
            });

            return Ok(orderDTOs);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> InsertOrder(InsertOrderDTO insertOrderDTO)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == insertOrderDTO.CustomerEmail);

            if (customer == null)
            {
                customer = new Customer
                {
                    Name = insertOrderDTO.CustomerName,
                    Email = insertOrderDTO.CustomerEmail
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductName == insertOrderDTO.ProductName);

            if (product == null)
            {
                product = new Product
                {
                    ProductName = insertOrderDTO.ProductName
                };
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }

            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = insertOrderDTO.OrderDate
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                OrderID = order.OrderID,
                ProductID = product.ProductID,
                Quantity = insertOrderDTO.Quantity
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            var orderDTO = new OrderDTO
            {
                OrderID = order.OrderID,
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                OrderDate = order.OrderDate,
                OrderItems = new List<OrderItemDTO>
                {
                    new OrderItemDTO
                    {
                        ProductName = product.ProductName,
                        Quantity = orderItem.Quantity
                    }
                }
            };

            return CreatedAtAction(nameof(GetOrdersByCustomerEmail), new { email = customer.Email }, orderDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}