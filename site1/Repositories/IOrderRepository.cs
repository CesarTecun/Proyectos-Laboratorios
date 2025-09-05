using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models;

namespace MessageApi.Repositories
{
    /// <summary>
    /// Interfaz que define las operaciones del repositorio para la entidad Order.
    /// Proporciona métodos para gestionar órdenes en la base de datos.
    /// </summary>
    public interface IOrderRepository : IDisposable
    {
        /// <summary>
        /// Crea una nueva orden en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="order">Objeto Order con los datos de la orden a crear</param>
        /// <returns>La orden creada con su ID generado</returns>
        Task<Order> CreateAsync(Order order);
        
        /// <summary>
        /// Obtiene una orden por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la orden a buscar</param>
        /// <returns>La orden encontrada o null si no existe</returns>
        Task<Order?> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtiene todas las órdenes del sistema de forma asíncrona.
        /// </summary>
        /// <returns>Colección de todas las órdenes</returns>
        Task<IEnumerable<Order>> GetAllAsync();
        
        /// <summary>
        /// Actualiza una orden existente en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="order">Objeto Order con los datos actualizados</param>
        /// <returns>True si la actualización fue exitosa, false en caso contrario</returns>
        Task<bool> UpdateAsync(Order order);
        
        /// <summary>
        /// Elimina una orden por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la orden a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false en caso contrario</returns>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Obtiene el siguiente número de orden disponible de forma asíncrona.
        /// </summary>
        /// <returns>El siguiente número de orden disponible</returns>
        Task<int> GetNextOrderNumberAsync();
    }
}

