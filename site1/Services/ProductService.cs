using HelloApi.Models;
using HelloApi.Models.DTOs;
using HelloApi.Repositories;

namespace HelloApi.Services
{
    public class ProductService(IProductRepository repository) : IProductService
    {
        private readonly IProductRepository _repository = repository;

        public async Task<ProductReadDto> CreateProductAsync(ProductCreateDto product)
        {
            var entity = await _repository.AddProductAsync(product);
            return MapToReadDto(entity);
        }

        public async Task<IEnumerable<ProductReadDto>> GetAllProductsAsync()
        {
            var entities = await _repository.GetAllProductsAsync();
            return entities.Select(MapToReadDto);
        }

        public async Task<ProductReadDto?> GetProductByIdAsync(int id)
        {
            var entity = await _repository.GetProductByIdAsync(id);
            return entity == null ? null : MapToReadDto(entity);
        }

        public async Task<ProductReadDto?> UpdateProductAsync(int id, ProductUpdateDto product)
        {
            var entity = new Product
            {
                Id = id,
                Name = product.Name,
            };

            var updated = await _repository.UpdateProductAsync(entity);
            return updated == null ? null : MapToReadDto(updated);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _repository.DeleteProductAsync(id);
        }

        private static ProductReadDto MapToReadDto(Product Product) => new()
        {
            Id = Product.Id,
            Name = Product.Name,
            CreatedAt = Product.CreatedAt,
            UpdatedAt = Product.UpdatedAt
        };

    }
}
