namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la lectura de datos de una orden.
    /// Se utiliza para transferir datos del servidor al cliente al consultar información de una orden.
    /// Incluye información detallada de la orden y sus ítems, así como el total calculado.
    /// </summary>
    public class OrderReadDto
    {
        /// <summary>
        /// Identificador único de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la persona asociada a la orden.
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Nombre completo de la persona que realizó el pedido.
        /// </summary>
        public string PersonName { get; set; } = string.Empty;

        /// <summary>
        /// Número de orden único.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó la orden.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Lista de detalles de la orden.
        /// Incluye información sobre los ítems, cantidades y precios.
        /// </summary>
        public List<OrderDetailReadDto> OrderDetails { get; set; } = [];

        /// <summary>
        /// Total calculado de la orden.
        /// Se calcula sumando los totales de cada detalle de la orden.
        /// </summary>
        public decimal Total => OrderDetails.Sum(od => od.Total);
    }
}


