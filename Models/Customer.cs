using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        
        [Required]
        public required string Name { get; set; }
        
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
