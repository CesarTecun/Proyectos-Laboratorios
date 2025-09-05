using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    /// <summary>
    /// Representa un pedido en el sistema.
    /// Esta entidad almacena la información de los pedidos realizados por los usuarios.
    /// </summary>
    [Table("Orders")]  // Especifica el nombre de la tabla en la base de datos
    public class Order
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la persona que realizó el pedido.
        /// Clave foránea que referencia a la tabla Persons.
        /// </summary>
        [Required(ErrorMessage = "El ID de la persona es obligatorio")]
        public int PersonId { get; set; }

        /// <summary>
        /// Navegación a la entidad Persona asociada a este pedido.
        /// Relación muchos a uno con Person.
        /// </summary>
        [ForeignKey("PersonId")]
        public Person Person { get; set; } = null!;

        /// <summary>
        /// Número de pedido.
        /// Identificador único secuencial para el control interno.
        /// </summary>
        [Required(ErrorMessage = "El número de pedido es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de pedido debe ser mayor que cero")]
        public int Number { get; set; }

        /// <summary>
        /// ID del usuario que creó el registro.
        /// </summary>
        [Required(ErrorMessage = "El ID del usuario creador es obligatorio")]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el pedido.
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
        /// Fecha y hora de la última actualización del pedido.
        /// Es nulo si el pedido nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Colección de detalles asociados a este pedido.
        /// Relación uno a muchos con OrderDetail.
        /// </summary>
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}

