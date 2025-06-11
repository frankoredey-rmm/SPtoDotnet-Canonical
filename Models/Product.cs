using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required]
        public required string ProductName { get; set; }
    }
}
