using System.ComponentModel.DataAnnotations;

namespace NewStore.DTOs
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }

    public class CreateOrderDTO
    {
        [Required]
        public string CustomerEmail { get; set; }

        [Required]
        public List<CreateOrderItemDTO> OrderItems { get; set; }
    }

    public class OrderItemDTO
    {
        public int OrderItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderItemDTO
    {
        [Required]
        public string ProductName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}