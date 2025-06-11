using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ECommerceApi.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public required Customer Customer { get; set; }
        public required ICollection<OrderItem> OrderItems { get; set; }
    }
}
