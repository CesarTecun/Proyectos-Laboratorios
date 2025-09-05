namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la lectura de datos de un detalle de orden.
    /// Se utiliza para transferir información de los detalles de una orden desde el servidor al cliente.
    /// Incluye información del ítem, cantidad, precio y total calculado.
    /// </summary>
    public class OrderDetailReadDto
    {
        /// <summary>
        /// Identificador único del detalle de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador del ítem asociado a este detalle.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Nombre del ítem en el momento de la compra.
        /// Se incluye para facilitar la visualización sin necesidad de consultas adicionales.
        /// </summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad de unidades del ítem en este detalle.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del ítem en el momento de la compra.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Total calculado para este detalle de orden.
        /// Se calcula como la multiplicación de la cantidad por el precio unitario.
        /// </summary>
        public decimal Total => Quantity * Price;
    }
}


