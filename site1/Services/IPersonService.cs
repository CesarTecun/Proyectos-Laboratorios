using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models.DTOs;

namespace MessageApi.Services
{
    /// <summary>
    /// Interfaz que define las operaciones del servicio para la gestión de personas.
    /// Proporciona métodos para realizar operaciones CRUD en personas.
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Crea una nueva persona de forma asíncrona.
        /// </summary>
        /// <param name="person">DTO con los datos de la persona a crear</param>
        /// <returns>DTO con los datos de la persona creada</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO de la persona es nulo</exception>
        Task<PersonReadDto> CreatePersonAsync(PersonCreateDto person);
        
        /// <summary>
        /// Obtiene todas las personas existentes de forma asíncrona.
        /// </summary>
        /// <returns>Colección de DTOs con los datos de las personas</returns>
        Task<IEnumerable<PersonReadDto>> GetAllPersonsAsync();
        
        /// <summary>
        /// Obtiene una persona por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la persona</param>
        /// <returns>DTO con los datos de la persona o null si no se encuentra</returns>
        Task<PersonReadDto?> GetPersonByIdAsync(int id);
        
        /// <summary>
        /// Actualiza una persona existente de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la persona a actualizar</param>
        /// <param name="person">DTO con los datos actualizados de la persona</param>
        /// <returns>DTO con los datos actualizados de la persona o null si no se encuentra</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO de la persona es nulo</exception>
        Task<PersonReadDto?> UpdatePersonAsync(int id, PersonUpdateDto person);
        
        /// <summary>
        /// Elimina una persona por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la persona a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        Task<bool> DeletePersonAsync(int id);
    }
}

