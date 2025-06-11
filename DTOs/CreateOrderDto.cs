using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public required string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public required string CustomerEmail { get; set; }

        [Required]
        public required string ProductName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
    }
}
