using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la creación de un nuevo ítem.
    /// </summary>
    public class ItemCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public required string Name { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
    }
}
