using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApi.Data;
using ECommerceApi.Models;
using ECommerceApi.DTOs;
using System.Threading.Tasks;
using System.Linq;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
            return Ok(orders);
        }

        [HttpGet("customer/{email}")]
        public async Task<IActionResult> GetOrdersByCustomerEmail(string email)
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Customer.Email == email)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto orderDto)
        {
            // Get or create customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == orderDto.CustomerEmail);

            if (customer == null)
            {
                customer = new Customer
                {
                    Name = orderDto.CustomerName,
                    Email = orderDto.CustomerEmail
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            // Get or create product
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductName == orderDto.ProductName);

            if (product == null)
            {
                product = new Product
                {
                    ProductName = orderDto.ProductName
                };
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }

            // Create order
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = orderDto.OrderDate
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order item
            var orderItem = new OrderItem
            {
                OrderID = order.OrderID,
                ProductID = product.ProductID,
                Quantity = orderDto.Quantity
            };
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllOrders), new { id = order.OrderID }, order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}