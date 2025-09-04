using System.ComponentModel.DataAnnotations;

namespace HelloApi.Models.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        public int PersonId { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        public List<OrderDetailCreateDto> OrderDetails { get; set; } = new();
    }
}
