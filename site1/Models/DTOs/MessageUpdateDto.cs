using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la actualización de un mensaje existente.
    /// Se utiliza para transferir datos del cliente al servidor al actualizar un mensaje.
    /// </summary>
    public class MessageUpdateDto
    {
        /// <summary>
        /// Nuevo contenido del mensaje.
        /// No puede estar vacío y debe tener entre 1 y 1000 caracteres.
        /// </summary>
        [Required(ErrorMessage = "El mensaje es obligatorio")]
        [StringLength(1000, ErrorMessage = "El mensaje no puede exceder los 1000 caracteres")]
        public string Message { get; set; } = string.Empty;
    }
}

