using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MessageApi.Models;
using MessageApi.Models.DTOs;
using MessageApi.Data;

namespace MessageApi.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad Person.
    /// Maneja las operaciones de base de datos para las personas en el sistema.
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de personas.
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public PersonRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Agrega una nueva persona al sistema de forma asíncrona.
        /// </summary>
        /// <param name="person">DTO con los datos de la persona a crear</param>
        /// <returns>La persona creada con su ID generado</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el parámetro person es nulo</exception>
        public async Task<Person> AddPersonAsync(PersonCreateDto person)
        {
            if (person == null) throw new ArgumentNullException(nameof(person));

            var entity = new Person
            {
                FirstName = person.FirstName ?? throw new ArgumentNullException(nameof(person.FirstName)),
                LastName = person.LastName ?? throw new ArgumentNullException(nameof(person.LastName)),
                Email = person.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            
            _context.Persons.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Obtiene todas las personas del sistema de forma asíncrona.
        /// Las personas se ordenan por su ID de forma ascendente.
        /// </summary>
        /// <returns>Colección de todas las personas</returns>
        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return await _context.Persons
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una persona por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la persona a buscar</param>
        /// <returns>La persona encontrada o null si no existe</returns>
        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
            
            return await _context.Persons.FindAsync(id);
        }

        /// <summary>
        /// Actualiza los datos de una persona existente de forma asíncrona.
        /// </summary>
        /// <param name="person">Objeto Person con los datos actualizados</param>
        /// <returns>La persona actualizada o null si no se encontró la persona</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el parámetro person es nulo</exception>
        public async Task<Person?> UpdatePersonAsync(Person person)
        {
            if (person == null) throw new ArgumentNullException(nameof(person));
            
            var existing = await _context.Persons.FindAsync(person.Id);
            if (existing == null) return null;
            
            existing.FirstName = person.FirstName ?? throw new ArgumentNullException(nameof(person.FirstName));
            existing.LastName = person.LastName ?? throw new ArgumentNullException(nameof(person.LastName));
            existing.Email = person.Email;
            existing.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existing;
        }

        /// <summary>
        /// Elimina una persona por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la persona a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false si la persona no existe</returns>
        public async Task<bool> DeletePersonAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
            
            var person = await _context.Persons.FindAsync(id);
            if (person == null) return false;

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
