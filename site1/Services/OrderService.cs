using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HelloApi.Models;
using HelloApi.Models.DTOs;
using HelloApi.Repositories;

namespace HelloApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IItemRepository itemRepository,
            IPersonRepository personRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public async Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto)
        {
            // Verificar que la persona existe
            var person = await _personRepository.GetByIdAsync(orderDto.PersonId);
            if (person == null)
            {
                throw new KeyNotFoundException("La persona especificada no existe.");
            }

            // Verificar que los ítems existen y obtener sus precios actuales
            var orderDetails = new List<OrderDetail>();
            foreach (var detailDto in orderDto.OrderDetails)
            {
                var item = await _itemRepository.GetByIdAsync(detailDto.ItemId);
                if (item == null)
                {
                    throw new KeyNotFoundException($"El ítem con ID {detailDto.ItemId} no existe.");
                }

                var orderDetail = new OrderDetail
                {
                    ItemId = detailDto.ItemId,
                    Quantity = detailDto.Quantity,
                    Price = item.Price, // Usar el precio actual del ítem
                    Total = item.Price * detailDto.Quantity,
                    CreatedBy = orderDto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                orderDetails.Add(orderDetail);
            }

            // Crear la orden
            var order = new Order
            {
                PersonId = orderDto.PersonId,
                CreatedBy = orderDto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                OrderDetails = orderDetails
            };

            // Guardar la orden
            var createdOrder = await _orderRepository.CreateAsync(order);
            
            // Mapear a DTO para la respuesta
            var result = _mapper.Map<OrderReadDto>(createdOrder);
            result.PersonName = $"{person.FirstName} {person.LastName}";
            
            // Mapear los detalles
            foreach (var detail in result.OrderDetails)
            {
                var item = await _itemRepository.GetByIdAsync(detail.ItemId);
                detail.ItemName = item?.Name ?? "Producto no encontrado";
            }

            return result;
        }

        public async Task<OrderReadDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            var result = _mapper.Map<OrderReadDto>(order);
            
            // Obtener el nombre de la persona
            var person = await _personRepository.GetByIdAsync(order.PersonId);
            result.PersonName = person != null ? $"{person.FirstName} {person.LastName}" : "Cliente no encontrado";
            
            return result;
        }

        public async Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var result = new List<OrderReadDto>();

            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<OrderReadDto>(order);
                
                // Obtener el nombre de la persona
                var person = await _personRepository.GetByIdAsync(order.PersonId);
                orderDto.PersonName = person != null ? $"{person.FirstName} {person.LastName}" : "Cliente no encontrado";
                
                result.Add(orderDto);
            }

            return result;
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderCreateDto orderDto)
        {
            // Verificar que la orden existe
            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return false;
            }

            // Verificar que la persona existe
            var person = await _personRepository.GetByIdAsync(orderDto.PersonId);
            if (person == null)
            {
                throw new KeyNotFoundException("La persona especificada no existe.");
            }

            // Mapear los detalles del DTO a entidades
            var orderDetails = new List<OrderDetail>();
            foreach (var detailDto in orderDto.OrderDetails)
            {
                var item = await _itemRepository.GetByIdAsync(detailDto.ItemId);
                if (item == null)
                {
                    throw new KeyNotFoundException($"El ítem con ID {detailDto.ItemId} no existe.");
                }

                var orderDetail = new OrderDetail
                {
                    OrderId = id,
                    ItemId = detailDto.ItemId,
                    Quantity = detailDto.Quantity,
                    Price = item.Price, // Usar el precio actual del ítem
                    Total = item.Price * detailDto.Quantity,
                    CreatedBy = existingOrder.CreatedBy,
                    CreatedAt = existingOrder.CreatedAt,
                    UpdatedBy = orderDto.CreatedBy,
                    UpdatedAt = DateTime.UtcNow
                };

                orderDetails.Add(orderDetail);
            }

            // Actualizar la orden
            existingOrder.PersonId = orderDto.PersonId;
            existingOrder.UpdatedBy = orderDto.CreatedBy;
            existingOrder.UpdatedAt = DateTime.UtcNow;
            existingOrder.OrderDetails = orderDetails;

            return await _orderRepository.UpdateAsync(existingOrder);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await _orderRepository.DeleteAsync(id);
        }
    }
}
