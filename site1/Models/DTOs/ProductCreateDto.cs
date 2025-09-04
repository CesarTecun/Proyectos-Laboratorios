using System.ComponentModel.DataAnnotations;

namespace HelloApi.Models.DTOs
{
    /// <summary>
    /// DTO para la creación de un nuevo producto.
    /// Se utiliza para transferir datos del cliente al servidor al crear un nuevo producto.
    /// </summary>
    public class ProductCreateDto
    {
        /// <summary>
        /// Nombre del producto.
        /// Debe ser único en el sistema.
        /// </summary>
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; } = string.Empty;
    }
}