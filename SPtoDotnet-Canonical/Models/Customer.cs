// Path: Models/Customer.cs
namespace SPtoDotnet_Canonical.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // Navigation property for Orders
        public ICollection<Order> Orders { get; set; }
    }
}
