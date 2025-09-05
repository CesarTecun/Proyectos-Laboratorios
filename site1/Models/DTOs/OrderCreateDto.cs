using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la creación de una nueva orden.
    /// Se utiliza para transferir datos del cliente al servidor al crear una nueva orden.
    /// Incluye la información básica de la orden y los detalles de los ítems incluidos.
    /// </summary>
    public class OrderCreateDto
    {
        /// <summary>
        /// Identificador de la persona que realiza el pedido.
        /// </summary>
        [Required(ErrorMessage = "El ID de la persona es obligatorio")]
        public int PersonId { get; set; }

        /// <summary>
        /// Identificador del usuario que crea la orden.
        /// </summary>
        [Required(ErrorMessage = "El ID del usuario creador es obligatorio")]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Lista de detalles de la orden.
        /// Debe contener al menos un detalle.
        /// </summary>
        [Required(ErrorMessage = "La orden debe contener al menos un detalle")]
        [MinLength(1, ErrorMessage = "La orden debe contener al menos un detalle")]
        public List<OrderDetailCreateDto> OrderDetails { get; set; } = new();
    }
}


