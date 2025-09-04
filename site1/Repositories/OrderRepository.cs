using Microsoft.EntityFrameworkCore;
using MessageApi.Data;
using HelloApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            order.Number = await GenerateOrderNumberAsync();
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Person)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Item)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Person)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Item)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> UpdateOrderAsync(Order order)
        {
            var existingOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
                return null;

            existingOrder.Notes = order.Notes;
            existingOrder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetOrdersByPersonIdAsync(int personId)
        {
            return await _context.Orders
                .Where(o => o.PersonId == personId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Item)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        private async Task<int> GenerateOrderNumberAsync()
        {
            var lastOrder = await _context.Orders
                .OrderByDescending(o => o.Number)
                .FirstOrDefaultAsync();

            return lastOrder?.Number + 1 ?? 1000;
        }
    }
}