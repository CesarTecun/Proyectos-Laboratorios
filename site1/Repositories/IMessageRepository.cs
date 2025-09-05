using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models;

namespace MessageApi.Repositories
{
    /// <summary>
    /// Interfaz que define las operaciones del repositorio para la entidad Message.
    /// Proporciona métodos para gestionar mensajes en el sistema.
    /// </summary>
    public interface IMessageRepository : IDisposable
    {
        /// <summary>
        /// Agrega un nuevo mensaje al sistema de forma asíncrona.
        /// </summary>
        /// <param name="message">Contenido del mensaje a agregar</param>
        /// <returns>El mensaje creado con su ID generado</returns>
        Task<Message> AddMessageAsync(string message);
        
        /// <summary>
        /// Obtiene todos los mensajes del sistema de forma asíncrona.
        /// </summary>
        /// <returns>Colección de todos los mensajes</returns>
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        
        /// <summary>
        /// Obtiene un mensaje por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del mensaje a buscar</param>
        /// <returns>El mensaje encontrado o null si no existe</returns>
        Task<Message?> GetMessageByIdAsync(int id);
        
        /// <summary>
        /// Actualiza un mensaje existente de forma asíncrona.
        /// </summary>
        /// <param name="message">Objeto Message con los datos actualizados</param>
        /// <returns>El mensaje actualizado o null si no se pudo actualizar</returns>
        Task<Message?> UpdateMessageAsync(Message message);
        
        /// <summary>
        /// Elimina un mensaje por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del mensaje a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false en caso contrario</returns>
        Task<bool> DeleteMessageAsync(int id);
    }
}

