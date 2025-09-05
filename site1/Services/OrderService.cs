using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessageApi.Models;
using MessageApi.Models.DTOs;
using MessageApi.Repositories;

namespace MessageApi.Services
{
    /// <summary>
    /// Implementación del servicio para la gestión de órdenes.
    /// Proporciona la lógica de negocio para operaciones CRUD de órdenes.
    /// </summary>
    public class OrderService : IOrderService, IDisposable
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;
        private bool _disposed = false;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de órdenes.
        /// </summary>
        /// <param name="orderRepository">Repositorio de órdenes</param>
        /// <param name="itemRepository">Repositorio de ítems</param>
        /// <param name="personRepository">Repositorio de personas</param>
        /// <param name="mapper">Mapeador para conversión entre entidades y DTOs</param>
        /// <exception cref="ArgumentNullException">Se lanza si alguno de los parámetros es nulo</exception>
        public OrderService(
            IOrderRepository orderRepository,
            IItemRepository itemRepository,
            IPersonRepository personRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Crea una nueva orden de forma asíncrona.
        /// </summary>
        /// <param name="orderDto">DTO con los datos de la orden a crear</param>
        /// <returns>DTO con los datos de la orden creada</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO de la orden es nulo</exception>
        /// <exception cref="KeyNotFoundException">Se lanza si la persona o algún ítem no existen</exception>
        public async Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));
                
            if (orderDto.OrderDetails == null || !orderDto.OrderDetails.Any())
                throw new ArgumentException("La orden debe contener al menos un detalle", nameof(orderDto.OrderDetails));
            // Verificar que la persona existe
            var person = await _personRepository.GetPersonByIdAsync(orderDto.PersonId);
            if (person == null)
            {
                throw new KeyNotFoundException("La persona especificada no existe.");
            }

            // Verificar que los ítems existen y obtener sus precios actuales
            var orderDetails = new List<OrderDetail>();
            foreach (var detailDto in orderDto.OrderDetails)
            {
                var item = await _itemRepository.GetItemByIdAsync(detailDto.ItemId);
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
                var item = await _itemRepository.GetItemByIdAsync(detail.ItemId);
                detail.ItemName = item?.Name ?? "Producto no encontrado";
            }

            return result;
        }

        /// <summary>
        /// Obtiene una orden por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la orden</param>
        /// <returns>DTO con los datos de la orden o null si no se encuentra</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<OrderReadDto?> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            var result = _mapper.Map<OrderReadDto>(order);
            
            // Obtener el nombre de la persona
            var person = await _personRepository.GetPersonByIdAsync(order.PersonId);
            result.PersonName = person != null ? $"{person.FirstName} {person.LastName}" : "Cliente no encontrado";
            
            return result;
        }

        /// <summary>
        /// Obtiene todas las órdenes existentes de forma asíncrona.
        /// </summary>
        /// <returns>Colección de DTOs con los datos de las órdenes</returns>
        public async Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var result = new List<OrderReadDto>();

            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<OrderReadDto>(order);
                
                // Obtener el nombre de la persona
                var person = await _personRepository.GetPersonByIdAsync(order.PersonId);
                orderDto.PersonName = person != null ? $"{person.FirstName} {person.LastName}" : "Cliente no encontrado";
                
                result.Add(orderDto);
            }

            return result;
        }

        /// <summary>
        /// Actualiza una orden existente de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la orden a actualizar</param>
        /// <param name="orderDto">DTO con los datos actualizados de la orden</param>
        /// <returns>True si la actualización fue exitosa, False si la orden no existe</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el DTO de la orden es nulo</exception>
        /// <exception cref="KeyNotFoundException">Se lanza si la persona o algún ítem no existen</exception>
        public async Task<bool> UpdateOrderAsync(int id, OrderCreateDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));
                
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
            // Verificar que la orden existe
            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return false;
            }

            // Verificar que la persona existe
            var person = await _personRepository.GetPersonByIdAsync(orderDto.PersonId);
            if (person == null)
            {
                throw new KeyNotFoundException("La persona especificada no existe.");
            }

            // Mapear los detalles del DTO a entidades
            var orderDetails = new List<OrderDetail>();
            foreach (var detailDto in orderDto.OrderDetails)
            {
                var item = await _itemRepository.GetItemByIdAsync(detailDto.ItemId);
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

        /// <summary>
        /// Elimina una orden por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único de la orden a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID es menor o igual a cero</exception>
        public async Task<bool> DeleteOrderAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "El ID debe ser mayor que cero");
                
            return await _orderRepository.DeleteAsync(id);
        }

        #region IDisposable Implementation
        
        /// <summary>
        /// Libera los recursos no administrados que usa el objeto y, de forma opcional, libera los recursos administrados.
        /// </summary>
        /// <param name="disposing">Es true para liberar tanto recursos administrados como no administrados; es false para liberar únicamente recursos no administrados.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Liberar recursos administrados
                    _orderRepository?.Dispose();
                    _itemRepository?.Dispose();
                    _personRepository?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Libera los recursos utilizados por el servicio de órdenes.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        #endregion
    }
}

