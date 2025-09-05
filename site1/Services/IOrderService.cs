using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models.DTOs;

namespace MessageApi.Services
{
    /// <summary>
    /// Interfaz que define las operaciones del servicio para la gestión de órdenes.
    /// Proporciona métodos para realizar operaciones CRUD en órdenes.
    /// </summary>
    public interface IOrderService : IDisposable
    {
        /// <summary>
        /// Crea una nueva orden de forma asíncrona.
        /// </summary>
        /// <param name="orderDto">DTO con los datos de la orden a crear</param>
        /// <returns>DTO con los datos de la orden creada</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO de la orden es nulo</exception>
        Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto);
        
        /// <summary>
        /// Obtiene una orden por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la orden</param>
        /// <returns>DTO con los datos de la orden o null si no se encuentra</returns>
        Task<OrderReadDto?> GetOrderByIdAsync(int id);
        
        /// <summary>
        /// Obtiene todas las órdenes existentes de forma asíncrona.
        /// </summary>
        /// <returns>Colección de DTOs con los datos de las órdenes</returns>
        Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync();
        
        /// <summary>
        /// Actualiza una orden existente de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la orden a actualizar</param>
        /// <param name="orderDto">DTO con los datos actualizados de la orden</param>
        /// <returns>True si la actualización fue exitosa, False en caso contrario</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO de la orden es nulo</exception>
        Task<bool> UpdateOrderAsync(int id, OrderCreateDto orderDto);
        
        /// <summary>
        /// Elimina una orden por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la orden a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        Task<bool> DeleteOrderAsync(int id);
    }
}

