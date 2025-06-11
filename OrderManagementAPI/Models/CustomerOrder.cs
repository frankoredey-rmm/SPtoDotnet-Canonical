using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagementAPI.Models
{
    // Legacy table model for migration purposes
    public class CustomerOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [StringLength(255)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(255)]
        public string CustomerEmail { get; set; } = string.Empty;

        [StringLength(255)]
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; }
    }
}