using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models.DTOs;

namespace MessageApi.Services
{
    /// <summary>
    /// Interfaz que define las operaciones del servicio para la gestión de mensajes.
    /// Proporciona métodos para realizar operaciones CRUD en mensajes.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Crea un nuevo mensaje de forma asíncrona.
        /// </summary>
        /// <param name="message">Contenido del mensaje a crear</param>
        /// <returns>DTO con los datos del mensaje creado</returns>
        Task<MessageReadDto> CreateMessageAsync(string message);
        
        /// <summary>
        /// Obtiene todos los mensajes existentes de forma asíncrona.
        /// </summary>
        /// <returns>Colección de DTOs con los datos de los mensajes</returns>
        Task<IEnumerable<MessageReadDto>> GetAllMessagesAsync();
        
        /// <summary>
        /// Obtiene un mensaje por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del mensaje</param>
        /// <returns>DTO con los datos del mensaje o null si no se encuentra</returns>
        Task<MessageReadDto?> GetMessageByIdAsync(int id);
        
        /// <summary>
        /// Actualiza el contenido de un mensaje existente de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del mensaje a actualizar</param>
        /// <param name="message">Nuevo contenido del mensaje</param>
        /// <returns>DTO con los datos actualizados del mensaje o null si no se encuentra</returns>
        Task<MessageReadDto?> UpdateMessageAsync(int id, string message);
        
        /// <summary>
        /// Elimina un mensaje por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del mensaje a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        Task<bool> DeleteMessageAsync(int id);
    }
}

