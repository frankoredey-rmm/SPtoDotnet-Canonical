using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStore.Models
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

        [ForeignKey("OrderID")]
        public Order Order { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
    }
}