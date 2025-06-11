using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        public required Order Order { get; set; }
        public required Product Product { get; set; }
    }
}
