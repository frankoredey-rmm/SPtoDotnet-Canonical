// Path: Models/OrderItem.cs
namespace SPtoDotnet_Canonical.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        // Navigation property for Order
        public Order Order { get; set; }

        // Navigation property for Product
        public Product Product { get; set; }
    }
}
