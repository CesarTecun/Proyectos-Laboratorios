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

            // Persist as Item and map back to Product shape
            var item = new Item
            {
                Name = product.Name ?? throw new ArgumentNullException(nameof(product.Name)),
                Price = product.Price,
                Description = product.Description ?? string.Empty,
                Stock = 0,
                CreatedBy = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return new Product
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description ?? string.Empty,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }

        /// <summary>
        /// Obtiene todos los productos del sistema de forma asíncrona.
        /// Los productos se ordenan por su ID de forma ascendente.
        /// </summary>
        /// <returns>Colección de todos los productos</returns>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var items = await _context.Items
                .OrderBy(i => i.Id)
                .ToListAsync();

            return items.Select(item => new Product
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description ?? string.Empty,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            });
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

            var item = await _context.Items.FindAsync(id);
            if (item == null) return null;
            return new Product
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description ?? string.Empty,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
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

            var item = await _context.Items.FindAsync(product.Id);
            if (item == null) return null;

            item.Name = product.Name ?? throw new ArgumentNullException(nameof(product.Name));
            item.Price = product.Price;
            item.Description = product.Description ?? string.Empty;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Product
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description ?? string.Empty,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
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

            var item = await _context.Items.FindAsync(id);
            if (item == null) return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

