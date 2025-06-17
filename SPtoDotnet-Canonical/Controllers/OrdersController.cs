// Path: Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPtoDotnet_Canonical.Data;
using SPtoDotnet_Canonical.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Added for DateTime

namespace SPtoDotnet_Canonical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        // Includes Customer and OrderItems for a more complete view
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Include Product details for each OrderItem
                .ToListAsync();
        }

        // GET: api/Orders/5
        // Includes Customer and OrderItems for a more complete view
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Include Product details for each OrderItem
                .FirstOrDefaultAsync(o => o.OrderID == id); // Use FirstOrDefaultAsync with the condition

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // GET: api/Orders/bycustomer?email=test@example.com
        // Endpoint to get orders by customer email, similar to the legacy stored procedure
        [HttpGet("bycustomer")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomerEmail([FromQuery] string email)
        {
             if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Customer email is required.");
            }

            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Customer.Email == email)
                .OrderByDescending(o => o.OrderDate) // Order by date descending as per legacy requirement
                .ToListAsync();

            if (!orders.Any())
            {
                return NotFound($"No orders found for customer email: {email}");
            }

            return orders;
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            // Basic implementation: Adds the order and its associated items if included in the object graph.
            // A more robust implementation might use a DTO and handle item creation/linking explicitly.
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Load related entities for the CreatedAtAction response
            await _context.Entry(order)
                .Reference(o => o.Customer).LoadAsync();
            await _context.Entry(order)
                .Collection(o => o.OrderItems).LoadAsync();
            foreach (var item in order.OrderItems)
            {
                 await _context.Entry(item).Reference(oi => oi.Product).LoadAsync();
            }


            return CreatedAtAction("GetOrder", new { id = order.OrderID }, order);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderID)
            {
                return BadRequest();
            }

            // Basic implementation: Updates the main order entity.
            // Updating associated OrderItems would require more complex logic
            // to handle additions, removals, and modifications of items.
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems) // Include items to delete them as well
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            // Remove associated OrderItems first (if cascade delete is not configured or preferred)
            _context.OrderItems.RemoveRange(order.OrderItems);

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
