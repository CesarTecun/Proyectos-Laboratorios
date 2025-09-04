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
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public OrderDetailService(
            IOrderDetailRepository orderDetailRepository,
            IOrderRepository orderRepository,
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDetailReadDto>> GetByOrderIdAsync(int orderId)
        {
            var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(orderId);
            return _mapper.Map<IEnumerable<OrderDetailReadDto>>(orderDetails);
        }

        public async Task<OrderDetailReadDto> AddOrderDetailAsync(OrderDetailCreateDto orderDetailDto)
        {
            // Verificar que la orden existe
            var order = await _orderRepository.GetOrderByIdAsync(orderDetailDto.OrderId);
            if (order == null)
                throw new KeyNotFoundException($"No se encontró la orden con ID {orderDetailDto.OrderId}");

            // Verificar que el ítem existe
            var item = await _itemRepository.GetItemByIdAsync(orderDetailDto.ItemId);
            if (item == null)
                throw new KeyNotFoundException($"No se encontró el ítem con ID {orderDetailDto.ItemId}");

            // Mapear el DTO a la entidad OrderDetail
            var orderDetail = _mapper.Map<OrderDetail>(orderDetailDto);
            orderDetail.Price = item.Price; // Establecer el precio actual del ítem
            orderDetail.Total = item.Price * orderDetail.Quantity;

            // Agregar el detalle a la orden
            var createdDetail = await _orderDetailRepository.AddOrderDetailAsync(orderDetail);

            // Actualizar el total de la orden
            await UpdateOrderTotalAsync(orderDetail.OrderId);

            return _mapper.Map<OrderDetailReadDto>(createdDetail);
        }

        public async Task<bool> RemoveOrderDetailAsync(int orderDetailId)
        {
            var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(orderDetailId);
            if (orderDetail == null)
                return false;

            var orderId = orderDetail.OrderId;
            var result = await _orderDetailRepository.RemoveOrderDetailAsync(orderDetailId);

            if (result)
            {
                // Actualizar el total de la orden
                await UpdateOrderTotalAsync(orderId);
            }

            return result;
        }

        public async Task<bool> UpdateOrderDetailQuantityAsync(int orderDetailId, int quantity)
        {
            var result = await _orderDetailRepository.UpdateOrderDetailQuantityAsync(orderDetailId, quantity);

            if (result)
            {
                // Obtener el orderId para actualizar el total de la orden
                var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(orderDetailId);
                if (orderDetail != null)
                {
                    await UpdateOrderTotalAsync(orderDetail.OrderId);
                }
            }

            return result;
        }

        private async Task UpdateOrderTotalAsync(int orderId)
        {
            var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(orderId);
            var total = orderDetails.Sum(od => od.Total);

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                order.Total = total;
                await _orderRepository.UpdateOrderAsync(order);
            }
        }
    }
}