using HelloApi.Models;
using HelloApi.Models.DTOs;

namespace HelloApi.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddProductAsync(ProductCreateDto product);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);

    }
}
