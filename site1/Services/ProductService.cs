using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessageApi.Models;
using MessageApi.Models.DTOs;
using MessageApi.Repositories;

namespace MessageApi.Services
{
    /// <summary>
    /// Implementación del servicio para la gestión de productos.
    /// Proporciona la lógica de negocio para operaciones CRUD de productos.
    /// </summary>
    public class ProductService : IProductService, IDisposable
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private bool _disposed = false;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de productos.
        /// </summary>
        /// <param name="repository">Repositorio de productos para acceso a datos</param>
        /// <param name="mapper">Mapeador para conversión entre entidades y DTOs</param>
        /// <exception cref="ArgumentNullException">Se lanza si alguno de los parámetros es nulo</exception>
        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Crea un nuevo producto de forma asíncrona.
        /// </summary>
        /// <param name="product">DTO con los datos del producto a crear</param>
        /// <returns>DTO con los datos del producto creado</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO del producto es nulo</exception>
        public async Task<ProductReadDto> CreateProductAsync(ProductCreateDto product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var entity = await _repository.AddProductAsync(product);
            return _mapper.Map<ProductReadDto>(entity);
        }

        /// <summary>
        /// Obtiene todos los productos existentes de forma asíncrona.
        /// </summary>
        /// <returns>Colección de DTOs con los datos de los productos</returns>
        public async Task<IEnumerable<ProductReadDto>> GetAllProductsAsync()
        {
            var entities = await _repository.GetAllProductsAsync();
            return _mapper.Map<IEnumerable<ProductReadDto>>(entities);
        }

        /// <summary>
        /// Obtiene un producto por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del producto</param>
        /// <returns>DTO con los datos del producto o null si no se encuentra</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<ProductReadDto?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");

            var entity = await _repository.GetProductByIdAsync(id);
            return entity == null ? null : _mapper.Map<ProductReadDto>(entity);
        }

        /// <summary>
        /// Actualiza un producto existente de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del producto a actualizar</param>
        /// <param name="product">DTO con los datos actualizados del producto</param>
        /// <returns>DTO con los datos actualizados del producto o null si no se encuentra</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO del producto es nulo</exception>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<ProductReadDto?> UpdateProductAsync(int id, ProductUpdateDto product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
                
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");

            var entity = new Product
            {
                Id = id,
                Name = product.Name ?? throw new ArgumentNullException(nameof(product.Name)),
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                UpdatedAt = DateTime.UtcNow
            };

            var updated = await _repository.UpdateProductAsync(entity);
            return updated == null ? null : _mapper.Map<ProductReadDto>(updated);
        }

        /// <summary>
        /// Elimina un producto por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del producto a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
                
            return await _repository.DeleteProductAsync(id);
        }

        private static ProductReadDto MapToReadDto(Product Product) => new()
        {
            Id = Product.Id,
            Name = Product.Name,
            CreatedAt = Product.CreatedAt,
            UpdatedAt = Product.UpdatedAt
        };

        #region IDisposable Implementation
        
        /// <summary>
        /// Libera los recursos no administrados que usa el objeto y, de forma opcional, libera los recursos administrados.
        /// </summary>
        /// <param name="disposing">Es true para liberar tanto recursos administrados como no administrados; es false para liberar únicamente recursos no administrados.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Liberar recursos administrados
                    _repository.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Libera los recursos utilizados por el servicio de productos.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        #endregion
    }
}

