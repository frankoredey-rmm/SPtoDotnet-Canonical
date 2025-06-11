using System.ComponentModel.DataAnnotations;

namespace NewStore.DTOs
{
    public class ProductDTO
    {
        public int ProductID { get; set; }

        [Required]
        public string ProductName { get; set; }
    }

    public class CreateProductDTO
    {
        [Required]
        public string ProductName { get; set; }
    }
}