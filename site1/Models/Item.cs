using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    /// <summary>
    /// Representa un artículo o producto en el sistema.
    /// Esta entidad se utiliza para gestionar los artículos que pueden ser incluidos en los pedidos.
    /// </summary>
    [Table("Items")]  // Especifica el nombre de la tabla en la base de datos
    public class Item
    {
        /// <summary>
        /// Identificador único del artículo.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Nombre del artículo.
        /// </summary>
        [Required(ErrorMessage = "El nombre del artículo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public required string Name { get; set; }

        /// <summary>
        /// Precio del artículo.
        /// Se almacena con precisión de 18 dígitos en total, 2 de ellos decimales.
        /// </summary>
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }

        /// <summary>
        /// ID del usuario que creó el registro.
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el registro.
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
        /// Fecha y hora de la última actualización del registro.
        /// Es nulo si el registro nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Colección de detalles de pedido que incluyen este artículo.
        /// Relación uno a muchos con OrderDetail.
        /// </summary>
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}

