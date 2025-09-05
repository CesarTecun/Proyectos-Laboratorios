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
    /// Implementación del servicio para la gestión de personas.
    /// Proporciona la lógica de negocio para operaciones CRUD de personas.
    /// </summary>
    public class PersonService : IPersonService, IDisposable
    {
        private readonly IPersonRepository _repository;
        private readonly IMapper _mapper;
        private bool _disposed = false;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de personas.
        /// </summary>
        /// <param name="repository">Repositorio de personas para acceso a datos</param>
        /// <param name="mapper">Mapeador para conversión entre entidades y DTOs</param>
        /// <exception cref="ArgumentNullException">Se lanza si alguno de los parámetros es nulo</exception>
        public PersonService(IPersonRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Crea una nueva persona de forma asíncrona.
        /// </summary>
        /// <param name="person">DTO con los datos de la persona a crear</param>
        /// <returns>DTO con los datos de la persona creada</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO de la persona es nulo</exception>
        public async Task<PersonReadDto> CreatePersonAsync(PersonCreateDto person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));

            var entity = await _repository.AddPersonAsync(person);
            return _mapper.Map<PersonReadDto>(entity);
        }

        /// <summary>
        /// Obtiene todas las personas existentes de forma asíncrona.
        /// </summary>
        /// <returns>Colección de DTOs con los datos de las personas</returns>
        public async Task<IEnumerable<PersonReadDto>> GetAllPersonsAsync()
        {
            var entities = await _repository.GetAllPersonsAsync();
            return _mapper.Map<IEnumerable<PersonReadDto>>(entities);
        }

        /// <summary>
        /// Obtiene una persona por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la persona</param>
        /// <returns>DTO con los datos de la persona o null si no se encuentra</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<PersonReadDto?> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");

            var entity = await _repository.GetPersonByIdAsync(id);
            return entity == null ? null : _mapper.Map<PersonReadDto>(entity);
        }

        /// <summary>
        /// Actualiza una persona existente de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la persona a actualizar</param>
        /// <param name="person">DTO con los datos actualizados de la persona</param>
        /// <returns>DTO con los datos actualizados de la persona o null si no se encuentra</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO de la persona es nulo</exception>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<PersonReadDto?> UpdatePersonAsync(int id, PersonUpdateDto person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));
                
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");

            var entity = new Person
            {
                Id = id,
                FirstName = person.FirstName ?? throw new ArgumentNullException(nameof(person.FirstName)),
                LastName = person.LastName ?? throw new ArgumentNullException(nameof(person.LastName)),
                Email = person.Email,
                UpdatedAt = DateTime.UtcNow
            };

            var updated = await _repository.UpdatePersonAsync(entity);
            return updated == null ? null : _mapper.Map<PersonReadDto>(updated);
        }

        /// <summary>
        /// Elimina una persona por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la persona a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<bool> DeletePersonAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
                
            return await _repository.DeletePersonAsync(id);
        }

        private static PersonReadDto MapToReadDto(Person Person) => new()
        {
            Id = Person.Id,
            FirstName = Person.FirstName,
            LastName = Person.LastName,
            Email = Person.Email,
            CreatedAt = Person.CreatedAt,
            UpdatedAt = Person.UpdatedAt
        };

        #region IDisposable Implementation
        
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
        /// Libera los recursos utilizados por el servicio de personas.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        #endregion
    }
}

