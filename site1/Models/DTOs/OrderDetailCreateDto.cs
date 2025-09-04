using System.ComponentModel.DataAnnotations;

namespace HelloApi.Models.DTOs
{
    public class OrderDetailCreateDto
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }
    }
}
