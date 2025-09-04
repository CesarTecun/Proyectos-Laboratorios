using HelloApi.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageApi.Services
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailReadDto>> GetByOrderIdAsync(int orderId);
        Task<OrderDetailReadDto> AddOrderDetailAsync(OrderDetailCreateDto orderDetailDto);
        Task<bool> RemoveOrderDetailAsync(int orderDetailId);
        Task<bool> UpdateOrderDetailQuantityAsync(int orderDetailId, int quantity);
    }
}