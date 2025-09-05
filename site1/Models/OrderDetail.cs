using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    /// <summary>
    /// Representa un detalle de pedido en el sistema.
    /// Esta entidad almacena los ítems individuales que componen un pedido.
    /// </summary>
    [Table("OrderDetails")]  // Especifica el nombre de la tabla en la base de datos
    public class OrderDetail
    {
        /// <summary>
        /// Identificador único del detalle de pedido.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Identificador del pedido al que pertenece este detalle.
        /// Clave foránea que referencia a la tabla Orders.
        /// </summary>
        [Required(ErrorMessage = "El ID del pedido es obligatorio")]
        public int OrderId { get; set; }

        /// <summary>
        /// Navegación a la entidad Pedido asociada a este detalle.
        /// Relación muchos a uno con Order.
        /// </summary>
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        /// <summary>
        /// Identificador del ítem incluido en este detalle.
        /// Clave foránea que referencia a la tabla Items.
        /// </summary>
        [Required(ErrorMessage = "El ID del ítem es obligatorio")]
        public int ItemId { get; set; }

        /// <summary>
        /// Navegación a la entidad Ítem asociada a este detalle.
        /// Relación muchos a uno con Item.
        /// </summary>
        [ForeignKey("ItemId")]
        public Item Item { get; set; } = null!;

        /// <summary>
        /// Cantidad de unidades del ítem en este detalle.
        /// </summary>
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero")]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del ítem en el momento de la compra.
        /// Se almacena con precisión de 18 dígitos en total, 2 de ellos decimales.
        /// </summary>
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }

        /// <summary>
        /// Total del detalle (Precio * Cantidad).
        /// Se almacena con precisión de 18 dígitos en total, 2 de ellos decimales.
        /// </summary>
        [Required(ErrorMessage = "El total es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor que cero")]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Total { get; set; }

        /// <summary>
        /// ID del usuario que creó el registro.
        /// </summary>
        [Required(ErrorMessage = "El ID del usuario creador es obligatorio")]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el detalle.
        /// Se establece automáticamente al crear el registro.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID del último usuario que actualizó el registro.
        /// </summary>
        public int UpdatedBy { get; set; }

        /// <summary>
        /// Fecha y hora de la última actualización del detalle.
        /// Es nulo si el detalle nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}

