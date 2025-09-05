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
    /// Implementación del repositorio para la entidad Product.
    /// Maneja las operaciones de base de datos para los productos en el sistema.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de productos.
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public ProductRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Agrega un nuevo producto al sistema de forma asíncrona.
        /// </summary>
        /// <param name="product">DTO con los datos del producto a crear</param>
        /// <returns>El producto creado con su ID generado</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el parámetro product es nulo</exception>
        public async Task<Product> AddProductAsync(ProductCreateDto product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            var entity = new Product
            {
                Name = product.Name ?? throw new ArgumentNullException(nameof(product.Name)),
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Obtiene todos los productos del sistema de forma asíncrona.
        /// Los productos se ordenan por su ID de forma ascendente.
        /// </summary>
        /// <returns>Colección de todos los productos</returns>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un producto por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del producto a buscar</param>
        /// <returns>El producto encontrado o null si no existe</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
            
            return await _context.Products.FindAsync(id);
        }

        /// <summary>
        /// Actualiza los datos de un producto existente de forma asíncrona.
        /// </summary>
        /// <param name="product">Objeto Product con los datos actualizados</param>
        /// <returns>El producto actualizado o null si no se encontró el producto</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el parámetro product es nulo</exception>
        public async Task<Product?> UpdateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null) return null;
            
            existing.Name = product.Name ?? throw new ArgumentNullException(nameof(product.Name));
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existing;
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

        /// <summary>
        /// Elimina un producto por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">ID del producto a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false si el producto no existe</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
            
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

