using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    public class OrderDetailUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}