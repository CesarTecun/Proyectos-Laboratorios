using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    /// <summary>
    /// Representa un detalle de factura en el sistema.
    /// Esta entidad almacena los ítems individuales que componen una factura.
    /// </summary>
    [Table("Details")]  // Especifica el nombre de la tabla en la base de datos
    public class Detail
    {
        /// <summary>
        /// Identificador único del detalle.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Identificador del estado del detalle.
        /// 1 = Activo, 2 = Cancelado, etc.
        /// </summary>
        [Required(ErrorMessage = "El ID de estado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de estado debe ser mayor que cero")]
        public int StatusId { get; set; }

        /// <summary>
        /// Precio unitario del producto en el momento de la compra.
        /// Se almacena con precisión de 18 dígitos en total, 2 de ellos decimales.
        /// </summary>
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad de unidades del producto.
        /// </summary>
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero")]
        public int Quantity { get; set; }

        /// <summary>
        /// Total del detalle (Precio * Cantidad).
        /// Se almacena con precisión de 18 dígitos en total, 2 de ellos decimales.
        /// </summary>
        [Required(ErrorMessage = "El total es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor que cero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el detalle.
        /// Se establece automáticamente al crear el registro.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización del detalle.
        /// Es nulo si el detalle nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Identificador de la factura a la que pertenece este detalle.
        /// Clave foránea que referencia a la tabla Invoices.
        /// </summary>
        [Required(ErrorMessage = "El ID de la factura es obligatorio")]
        public int InvoiceId { get; set; }

        /// <summary>
        /// Navegación a la entidad Factura asociada a este detalle.
        /// Relación muchos a uno con Invoice.
        /// </summary>
        [ForeignKey("InvoiceId")]
        public Invoice? Invoice { get; set; }

        /// <summary>
        /// Identificador del producto asociado a este detalle.
        /// Clave foránea que referencia a la tabla Products.
        /// </summary>
        [Required(ErrorMessage = "El ID del producto es obligatorio")]
        public int ProductId { get; set; }

        /// <summary>
        /// Navegación a la entidad Producto asociada a este detalle.
        /// Relación muchos a uno con Product.
        /// </summary>
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}

