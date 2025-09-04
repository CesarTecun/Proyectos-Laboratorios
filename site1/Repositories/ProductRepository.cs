using HelloApi.Models;
using HelloApi.Models.DTOs;
using MessageApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HelloApi.Repositories
{
    public class ProductRepository(AppDbContext context) : IProductRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Product> AddProductAsync(ProductCreateDto product)
        {
            var entity = new Product
            {
                Name = product.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.OrderBy(m => m.Id).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null) return null;
            existing.Name = product.Name;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var entity = await _context.Messages.FindAsync(id);
            if (entity == null) return false;
            _context.Messages.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
