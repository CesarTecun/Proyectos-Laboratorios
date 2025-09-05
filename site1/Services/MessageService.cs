using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessageApi.Models;
using MessageApi.Models.DTOs;
using MessageApi.Repositories;

namespace MessageApi.Services
{
    /// <summary>
    /// Implementación del servicio para la gestión de mensajes.
    /// Proporciona la lógica de negocio para operaciones CRUD de mensajes.
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de mensajes.
        /// </summary>
        /// <param name="repository">Repositorio de mensajes para acceso a datos</param>
        /// <param name="mapper">Mapeador para conversión entre entidades y DTOs</param>
        public MessageService(IMessageRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Crea un nuevo mensaje de forma asíncrona.
        /// </summary>
        /// <param name="message">Contenido del mensaje a crear</param>
        /// <returns>DTO con los datos del mensaje creado</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el mensaje es nulo o vacío</exception>
        public async Task<MessageReadDto> CreateMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message), "El mensaje no puede estar vacío");

            var entity = await _repository.AddMessageAsync(message);
            return _mapper.Map<MessageReadDto>(entity);
        }

        /// <summary>
        /// Obtiene todos los mensajes existentes de forma asíncrona.
        /// </summary>
        /// <returns>Colección de DTOs con los datos de los mensajes</returns>
        public async Task<IEnumerable<MessageReadDto>> GetAllMessagesAsync()
        {
            var entities = await _repository.GetAllMessagesAsync();
            return _mapper.Map<IEnumerable<MessageReadDto>>(entities);
        }

        /// <summary>
        /// Obtiene un mensaje por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del mensaje</param>
        /// <returns>DTO con los datos del mensaje o null si no se encuentra</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<MessageReadDto?> GetMessageByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");

            var entity = await _repository.GetMessageByIdAsync(id);
            return entity == null ? null : _mapper.Map<MessageReadDto>(entity);
        }

        /// <summary>
        /// Actualiza el contenido de un mensaje existente de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del mensaje a actualizar</param>
        /// <param name="message">Nuevo contenido del mensaje</param>
        /// <returns>DTO con los datos actualizados del mensaje o null si no se encuentra</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es inválido</exception>
        /// <exception cref="ArgumentNullException">Se lanza si el mensaje es nulo o vacío</exception>
        public async Task<MessageReadDto?> UpdateMessageAsync(int id, string message)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
                
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message), "El mensaje no puede estar vacío");

            var entity = new Message
            {
                Id = id,
                MessageText = message,
                UpdatedAt = DateTime.UtcNow
            };

            var updated = await _repository.UpdateMessageAsync(entity);
            return updated == null ? null : _mapper.Map<MessageReadDto>(updated);
        }

        /// <summary>
        /// Elimina un mensaje por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del mensaje a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<bool> DeleteMessageAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
                
            return await _repository.DeleteMessageAsync(id);
        }

        #region IDisposable Implementation
        
        private bool _disposed = false;

        /// <summary>
        /// Libera los recursos no administrados que usa el objeto y, de forma opcional, libera los recursos administrados.
        /// </summary>
        /// <param name="disposing">Es true para liberar tanto recursos administrados como no administrados; es false para liberar únicamente recursos no administrados.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Liberar recursos administrados
                    _repository.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Libera los recursos utilizados por el servicio de mensajes.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        #endregion

        private static MessageReadDto MapToReadDto(Message message) => new()
        {
            Id = message.Id,
            Message = message.MessageText,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };
    }
}

