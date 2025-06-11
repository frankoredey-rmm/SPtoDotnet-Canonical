using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewStore.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}