using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    /// <summary>
    /// Representa una persona en el sistema.
    /// Esta entidad almacena la información básica de las personas que interactúan con la aplicación.
    /// </summary>
    [Table("Persons")]  // Especifica el nombre de la tabla en la base de datos
    public class Person
    {
        /// <summary>
        /// Identificador único de la persona.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Primer nombre de la persona.
        /// </summary>
        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El primer nombre no puede exceder los 50 caracteres")]
        [Column("FirstName")]  // Especifica el nombre de la columna en la base de datos
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido de la persona.
        /// </summary>
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
        [Column("LastName")]  // Especifica el nombre de la columna en la base de datos
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico de la persona.
        /// Debe ser único en el sistema.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres")]
        [Column("Email")]  // Especifica el nombre de la columna en la base de datos
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se creó el registro de la persona.
        /// Se establece automáticamente al crear el registro.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("CreatedAt")]  // Especifica el nombre de la columna en la base de datos
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización del registro de la persona.
        /// Es nulo si el registro nunca ha sido actualizado.
        /// </summary>
        [Column("UpdatedAt")]  // Especifica el nombre de la columna en la base de datos
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Colección de pedidos realizados por esta persona.
        /// Relación uno a muchos con Order.
        /// </summary>
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}

