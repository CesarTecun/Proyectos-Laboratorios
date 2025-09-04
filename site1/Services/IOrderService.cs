using System.Collections.Generic;
using System.Threading.Tasks;
using HelloApi.Models;
using HelloApi.Models.DTOs;

namespace HelloApi.Services
{
    public interface IOrderService
    {
        Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto);
        Task<OrderReadDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderAsync(int id, OrderCreateDto orderDto);
        Task<bool> DeleteOrderAsync(int id);
    }
}
