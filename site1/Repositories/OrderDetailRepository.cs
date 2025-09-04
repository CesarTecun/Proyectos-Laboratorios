using Microsoft.EntityFrameworkCore;
using MessageApi.Data;
using HelloApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageApi.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Item)
                .ToListAsync();
        }

        public async Task<OrderDetail> AddOrderDetailAsync(OrderDetail orderDetail)
        {
            orderDetail.CreatedAt = DateTime.UtcNow;
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<bool> RemoveOrderDetailAsync(int orderDetailId)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(orderDetailId);
            if (orderDetail == null)
                return false;

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderDetailQuantityAsync(int orderDetailId, int quantity)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(orderDetailId);
            if (orderDetail == null)
                return false;

            orderDetail.Quantity = quantity;
            orderDetail.UpdatedAt = DateTime.UtcNow;
            orderDetail.Total = orderDetail.Price * quantity;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}