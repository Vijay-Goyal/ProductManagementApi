using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models    
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;
        [Range(0.01,double.MaxValue)]
        public decimal Price { get; set; }

    }
}
