using System.ComponentModel.DataAnnotations;

namespace NewStore.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required]
        public string ProductName { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}