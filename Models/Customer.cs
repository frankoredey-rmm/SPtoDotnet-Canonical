using System.ComponentModel.DataAnnotations;

namespace NewStore.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}