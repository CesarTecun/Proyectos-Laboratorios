using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models;

namespace MessageApi.Repositories
{
    /// <summary>
    /// Interfaz que define las operaciones del repositorio para la entidad Item.
    /// Proporciona métodos para gestionar ítems en la base de datos.
    /// </summary>
    public interface IItemRepository : IDisposable
    {
        /// <summary>
        /// Obtiene todos los ítems de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de ítems</returns>
        Task<IEnumerable<Item>> GetAllItemsAsync();

        /// <summary>
        /// Obtiene un ítem por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del ítem a buscar</param>
        /// <returns>El ítem encontrado o null si no existe</returns>
        Task<Item?> GetItemByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo ítem en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="item">Objeto Item con los datos del ítem a crear</param>
        /// <returns>El ítem creado con su ID generado</returns>
        Task<Item> CreateItemAsync(Item item);

        /// <summary>
        /// Actualiza un ítem existente en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="item">Objeto Item con los datos actualizados</param>
        /// <returns>True si la actualización fue exitosa, false si el ítem no existe</returns>
        Task<bool> UpdateItemAsync(Item item);

        /// <summary>
        /// Elimina un ítem de la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del ítem a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false si el ítem no existe</returns>
        Task<bool> DeleteItemAsync(int id);
    }
}

