using System.ComponentModel.DataAnnotations;

namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la creación de una nueva persona.
    /// Se utiliza para transferir datos del cliente al servidor al crear un nuevo registro de persona.
    /// </summary>
    public class PersonCreateDto
    {
        /// <summary>
        /// Primer nombre de la persona.
        /// </summary>
        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El primer nombre no puede exceder los 50 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido de la persona.
        /// </summary>
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico de la persona.
        /// Debe ser una dirección de correo electrónico válida.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres")]
        public string Email { get; set; } = string.Empty;
    }
}

