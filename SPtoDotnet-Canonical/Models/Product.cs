// Path: Models/Product.cs
namespace SPtoDotnet_Canonical.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        // Navigation property for OrderItems
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
