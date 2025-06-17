// Path: Models/Order.cs
using System;
using System.Collections.Generic;

namespace SPtoDotnet_Canonical.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }

        // Navigation property for Customer
        public Customer Customer { get; set; }

        // Navigation property for OrderItems
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
