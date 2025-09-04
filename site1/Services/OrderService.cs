using AutoMapper;
using MessageApi.Repositories;
using HelloApi.Models;
using HelloApi.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IPersonRepository personRepository,
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _personRepository = personRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto)
        {
            // Verificar que la persona existe
            var person = await _personRepository.GetPersonByIdAsync(orderDto.PersonId);
            if (person == null)
                throw new KeyNotFoundException($"No se encontró la persona con ID {orderDto.PersonId}");

            // Mapear el DTO a la entidad Order
            var order = _mapper.Map<Order>(orderDto);
            order.CreatedAt = DateTime.UtcNow;

            // Crear la orden
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // Mapear y retornar el resultado
            return _mapper.Map<OrderReadDto>(createdOrder);
        }

        public async Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return _mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }

        public async Task<OrderReadDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderReadDto>(order);
        }

        public async Task<OrderReadDto?> UpdateOrderAsync(int id, OrderUpdateDto orderDto)
        {
            var existingOrder = await _orderRepository.GetOrderByIdAsync(id);
            if (existingOrder == null)
                return null;

            _mapper.Map(orderDto, existingOrder);
            existingOrder.UpdatedAt = DateTime.UtcNow;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(existingOrder);
            return _mapper.Map<OrderReadDto>(updatedOrder);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await _orderRepository.DeleteOrderAsync(id);
        }

        public async Task<IEnumerable<OrderReadDto>> GetOrdersByPersonIdAsync(int personId)
        {
            var orders = await _orderRepository.GetOrdersByPersonIdAsync(personId);
            return _mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }
    }
}