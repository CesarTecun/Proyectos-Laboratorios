using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    public class OrderDetailCreateDto
    {
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public int ItemId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}