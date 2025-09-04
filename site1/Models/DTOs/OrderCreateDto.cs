using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        public int PersonId { get; set; }
        
        public string? Notes { get; set; }
    }
}