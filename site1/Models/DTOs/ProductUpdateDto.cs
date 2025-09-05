using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la actualización de un producto existente.
    /// Se utiliza para transferir datos del cliente al servidor al actualizar un producto.
    /// </summary>
    public class ProductUpdateDto
    {
        /// <summary>
        /// Nuevo nombre del producto.
        /// Debe ser único en el sistema.
        /// </summary>
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Nueva cantidad en inventario.
        /// </summary>
        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
    }
}

