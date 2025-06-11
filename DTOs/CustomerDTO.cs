using System.ComponentModel.DataAnnotations;

namespace NewStore.DTOs
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class CreateCustomerDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}