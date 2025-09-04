using HelloApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageApi.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
        Task<OrderDetail> AddOrderDetailAsync(OrderDetail orderDetail);
        Task<bool> RemoveOrderDetailAsync(int orderDetailId);
        Task<bool> UpdateOrderDetailQuantityAsync(int orderDetailId, int quantity);
    }
}