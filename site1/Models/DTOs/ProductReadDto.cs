namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la lectura de datos de un producto.
    /// Se utiliza para transferir datos del servidor al cliente al consultar información de un producto.
    /// Incluye metadatos como fechas de creación y actualización.
    /// </summary>
    public class ProductReadDto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se creó el registro del producto.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha y hora de la última actualización del registro del producto.
        /// Es nulo si el registro nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}


