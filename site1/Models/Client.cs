using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    /// <summary>
    /// Representa un cliente en el sistema.
    /// Esta entidad almacena la información básica de los clientes que realizan compras.
    /// </summary>
    [Table("Clients")]  // Especifica el nombre de la tabla en la base de datos
    public class Client
    {
        /// <summary>
        /// Identificador único del cliente.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Primer nombre del cliente.
        /// </summary>
        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El primer nombre no puede exceder los 50 caracteres")]
        public required string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del cliente.
        /// </summary>
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
        public required string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico del cliente.
        /// </summary>
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Número de Identificación Tributaria (NIT) del cliente.
        /// </summary>
        [Required(ErrorMessage = "El NIT es obligatorio")]
        [StringLength(20, ErrorMessage = "El NIT no puede exceder los 20 caracteres")]
        public required string Nit { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se creó el registro del cliente.
        /// Se establece automáticamente al crear el registro.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización del registro del cliente.
        /// Es nulo si el registro nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Colección de facturas asociadas a este cliente.
        /// Relación uno a muchos con Invoice.
        /// </summary>
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}

