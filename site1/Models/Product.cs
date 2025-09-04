using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloApi.Models
{
    /// <summary>
    /// Representa un producto en el sistema.
    /// Esta entidad almacena la información de los productos disponibles para la venta.
    /// </summary>
    [Table("Products")]  // Especifica el nombre de la tabla en la base de datos
    public class Product
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se creó el registro del producto.
        /// Se establece automáticamente al crear el registro.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización del registro del producto.
        /// Es nulo si el registro nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Colección de detalles de factura que incluyen este producto.
        /// Relación uno a muchos con Detail.
        /// </summary>
        public List<Detail> Details { get; set; } = new List<Detail>();
    }
}
