using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models;
using MessageApi.Models.DTOs;

namespace MessageApi.Repositories
{
    /// <summary>
    /// Interfaz que define las operaciones del repositorio para la entidad Person.
    /// Proporciona métodos para gestionar personas en el sistema.
    /// </summary>
    public interface IPersonRepository : IDisposable
    {
        /// <summary>
        /// Agrega una nueva persona al sistema de forma asíncrona.
        /// </summary>
        /// <param name="person">DTO con los datos de la persona a crear</param>
        /// <returns>La persona creada con su ID generado</returns>
        Task<Person> AddPersonAsync(PersonCreateDto person);
        
        /// <summary>
        /// Obtiene todas las personas del sistema de forma asíncrona.
        /// </summary>
        /// <returns>Colección de todas las personas</returns>
        Task<IEnumerable<Person>> GetAllPersonsAsync();
        
        /// <summary>
        /// Obtiene una persona por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la persona a buscar</param>
        /// <returns>La persona encontrada o null si no existe</returns>
        Task<Person?> GetPersonByIdAsync(int id);
        
        /// <summary>
        /// Actualiza los datos de una persona existente de forma asíncrona.
        /// </summary>
        /// <param name="person">Objeto Person con los datos actualizados</param>
        /// <returns>La persona actualizada o null si no se pudo actualizar</returns>
        Task<Person?> UpdatePersonAsync(Person person);
        
        /// <summary>
        /// Elimina una persona por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la persona a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false en caso contrario</returns>
        Task<bool> DeletePersonAsync(int id);
    }
}

