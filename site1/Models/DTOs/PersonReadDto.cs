namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la lectura de datos de una persona.
    /// Se utiliza para transferir datos del servidor al cliente al consultar información de una persona.
    /// Incluye metadatos como fechas de creación y actualización.
    /// </summary>
    public class PersonReadDto
    {
        /// <summary>
        /// Identificador único de la persona.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Primer nombre de la persona.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido de la persona.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico de la persona.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se creó el registro de la persona.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha y hora de la última actualización del registro de la persona.
        /// Es nulo si el registro nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}


