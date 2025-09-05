using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models.DTOs;

namespace MessageApi.Services
{
    /// <summary>
    /// Interfaz que define las operaciones del servicio para la gestión de productos.
    /// Proporciona métodos para realizar operaciones CRUD en productos.
    /// </summary>
    public interface IProductService : IDisposable
    {
        /// <summary>
        /// Crea un nuevo producto de forma asíncrona.
        /// </summary>
        /// <param name="product">DTO con los datos del producto a crear</param>
        /// <returns>DTO con los datos del producto creado</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO del producto es nulo</exception>
        Task<ProductReadDto> CreateProductAsync(ProductCreateDto product);
        
        /// <summary>
        /// Obtiene todos los productos existentes de forma asíncrona.
        /// </summary>
        /// <returns>Colección de DTOs con los datos de los productos</returns>
        Task<IEnumerable<ProductReadDto>> GetAllProductsAsync();
        
        /// <summary>
        /// Obtiene un producto por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del producto</param>
        /// <returns>DTO con los datos del producto o null si no se encuentra</returns>
        Task<ProductReadDto?> GetProductByIdAsync(int id);
        
        /// <summary>
        /// Actualiza un producto existente de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del producto a actualizar</param>
        /// <param name="product">DTO con los datos actualizados del producto</param>
        /// <returns>DTO con los datos actualizados del producto o null si no se encuentra</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO del producto es nulo</exception>
        Task<ProductReadDto?> UpdateProductAsync(int id, ProductUpdateDto product);
        
        /// <summary>
        /// Elimina un producto por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del producto a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        Task<bool> DeleteProductAsync(int id);
    }
}

