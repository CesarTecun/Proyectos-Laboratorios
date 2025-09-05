namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la lectura de datos de un mensaje.
    /// Se utiliza para transferir datos del servidor al cliente al consultar información de un mensaje.
    /// Incluye metadatos como fechas de creación y actualización.
    /// </summary>
    public class MessageReadDto
    {
        /// <summary>
        /// Identificador único del mensaje.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Contenido del mensaje.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se creó el mensaje.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha y hora de la última actualización del mensaje.
        /// Es nulo si el mensaje nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}


