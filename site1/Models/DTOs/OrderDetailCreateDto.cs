using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la creación de un detalle de orden.
    /// Representa un ítem específico dentro de una orden, incluyendo cantidad y precio.
    /// </summary>
    public class OrderDetailCreateDto
    {
        /// <summary>
        /// Identificador del ítem que se está ordenando.
        /// </summary>
        [Required(ErrorMessage = "El ID del ítem es obligatorio")]
        public int ItemId { get; set; }

        /// <summary>
        /// Cantidad del ítem que se está ordenando.
        /// Debe ser un número entero mayor a 0.
        /// </summary>
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del ítem en el momento de la compra.
        /// Debe ser un valor numérico mayor a 0 con hasta 2 decimales.
        /// </summary>
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El precio debe tener hasta 2 decimales")]
        public decimal Price { get; set; }
    }
}


