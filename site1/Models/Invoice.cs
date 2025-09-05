using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    /// <summary>
    /// Representa una factura en el sistema.
    /// Esta entidad almacena la información de las facturas generadas a los clientes.
    /// </summary>
    [Table("Invoices")]  // Especifica el nombre de la tabla en la base de datos
    public class Invoice
    {
        /// <summary>
        /// Identificador único de la factura.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Serie de la factura (ejemplo: "A", "B", "C").
        /// </summary>
        [Required(ErrorMessage = "La serie de la factura es obligatoria")]
        [StringLength(10, ErrorMessage = "La serie no puede exceder los 10 caracteres")]
        public required string Serial { get; set; } = string.Empty;

        /// <summary>
        /// Número correlativo de la factura.
        /// </summary>
        [Required(ErrorMessage = "El número de factura es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de factura debe ser mayor que cero")]
        public required int Number { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó la factura.
        /// Se establece automáticamente al crear el registro.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización de la factura.
        /// Es nulo si la factura nunca ha sido actualizada.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Identificador del cliente al que pertenece esta factura.
        /// Clave foránea que referencia a la tabla Clients.
        /// </summary>
        [Required(ErrorMessage = "El ID del cliente es obligatorio")]
        public int ClientId { get; set; }

        /// <summary>
        /// Navegación a la entidad Cliente asociada a esta factura.
        /// Relación muchos a uno con Client.
        /// </summary>
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        /// <summary>
        /// Colección de detalles asociados a esta factura.
        /// Relación uno a muchos con Detail.
        /// </summary>
        public List<Detail> Details { get; set; } = new List<Detail>();
    }
}

