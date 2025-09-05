using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models;
using MessageApi.Models.DTOs;

namespace MessageApi.Repositories
{
    /// <summary>
    /// Interfaz que define las operaciones del repositorio para la entidad Product.
    /// Proporciona métodos para gestionar productos en el sistema.
    /// </summary>
    public interface IProductRepository : IDisposable
    {
        /// <summary>
        /// Agrega un nuevo producto al sistema de forma asíncrona.
        /// </summary>
        /// <param name="product">DTO con los datos del producto a crear</param>
        /// <returns>El producto creado con su ID generado</returns>
        Task<Product> AddProductAsync(ProductCreateDto product);
        
        /// <summary>
        /// Obtiene todos los productos del sistema de forma asíncrona.
        /// </summary>
        /// <returns>Colección de todos los productos</returns>
        Task<IEnumerable<Product>> GetAllProductsAsync();
        
        /// <summary>
        /// Obtiene un producto por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del producto a buscar</param>
        /// <returns>El producto encontrado o null si no existe</returns>
        Task<Product?> GetProductByIdAsync(int id);
        
        /// <summary>
        /// Actualiza los datos de un producto existente de forma asíncrona.
        /// </summary>
        /// <param name="product">Objeto Product con los datos actualizados</param>
        /// <returns>El producto actualizado o null si no se pudo actualizar</returns>
        Task<Product?> UpdateProductAsync(Product product);
        
        /// <summary>
        /// Elimina un producto por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del producto a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false en caso contrario</returns>
        Task<bool> DeleteProductAsync(int id);
    }
}

