using HelloApi.Models.DTOs;

namespace HelloApi.Services
{
    public interface IProductService
    {
        Task<ProductReadDto> CreateProductAsync(ProductCreateDto product);
        Task<IEnumerable<ProductReadDto>> GetAllProductsAsync();
        Task<ProductReadDto?> GetProductByIdAsync(int id);
        Task<ProductReadDto?> UpdateProductAsync(int id, ProductUpdateDto product);
        Task<bool> DeleteProductAsync(int id);
    }
}
