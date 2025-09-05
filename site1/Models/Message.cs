using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models
{
    /// <summary>
    /// Representa un mensaje en el sistema.
    /// Esta entidad se utiliza para almacenar mensajes generados por la aplicación o los usuarios.
    /// </summary>
    [Table("Messages")]  // Especifica el nombre de la tabla en la base de datos
    public class Message
    {
        /// <summary>
        /// Identificador único del mensaje.
        /// </summary>
        [Key]  // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Generación automática del ID
        public int Id { get; set; }

        /// <summary>
        /// Contenido del mensaje.
        /// </summary>
        [Required(ErrorMessage = "El texto del mensaje es obligatorio")]
        [StringLength(1000, ErrorMessage = "El mensaje no puede exceder los 1000 caracteres")]
        [Column("MessageText")]  // Especifica el nombre de la columna en la base de datos
        public string MessageText { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en que se creó el mensaje.
        /// Se establece automáticamente al crear el registro.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización del mensaje.
        /// Es nulo si el mensaje nunca ha sido actualizado.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}

