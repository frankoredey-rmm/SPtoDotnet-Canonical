using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get orders by customer email
        /// </summary>
        /// <param name="email">Customer email address</param>
        /// <returns>List of orders for the specified customer</returns>
        [HttpGet("customer/{email}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomerEmail(string email)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustomerEmailAsync(email);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="createOrderDto">Order creation data</param>
        /// <returns>Created order</returns>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _orderService.InsertOrderAsync(createOrderDto);
                if (order == null)
                {
                    return BadRequest("Failed to create order");
                }

                return CreatedAtAction(nameof(GetAllOrders), new { id = order.OrderID }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete an order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderByIdAsync(id);
                if (!result)
                {
                    return NotFound($"Order with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Migrate data from legacy CustomerOrders table to normalized schema
        /// </summary>
        /// <returns>Success status</returns>
        [HttpPost("migrate")]
        public async Task<ActionResult> MigrateData()
        {
            try
            {
                await _orderService.MigrateDataAsync();
                return Ok("Data migration completed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Migration failed: {ex.Message}");
            }
        }
    }
}