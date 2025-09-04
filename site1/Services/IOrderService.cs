using HelloApi.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageApi.Services
{
    public interface IOrderService
    {
        Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto);
        Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync();
        Task<OrderReadDto?> GetOrderByIdAsync(int id);
        Task<OrderReadDto?> UpdateOrderAsync(int id, OrderUpdateDto orderDto);
        Task<bool> DeleteOrderAsync(int id);
        Task<IEnumerable<OrderReadDto>> GetOrdersByPersonIdAsync(int personId);
    }
}