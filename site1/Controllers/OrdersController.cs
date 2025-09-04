using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HelloApi.Models.DTOs;
using HelloApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelloApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger,
            IMapper mapper)
        {
            _orderService = orderService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las órdenes");
                return StatusCode(500, "Error interno del servidor al obtener las órdenes");
            }
        }

        // GET: api/orders/5
        [HttpGet("{id}", Name = "GetOrderById")]
        public async Task<ActionResult<OrderReadDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la orden con ID {id}");
                return StatusCode(500, "Error interno del servidor al obtener la orden");
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderReadDto>> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Obtener el ID del usuario autenticado (si hay autenticación)
                // var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                // orderDto.CreatedBy = userId;
                
                // Por ahora, si no hay autenticación, usamos un valor por defecto
                if (orderDto.CreatedBy <= 0)
                {
                    orderDto.CreatedBy = 1; // Usuario por defecto
                }

                var order = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtRoute(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden");
                return StatusCode(500, "Error interno del servidor al crear la orden");
            }
        }

        // PUT: api/orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderCreateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Obtener el ID del usuario autenticado (si hay autenticación)
                // var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                // orderDto.CreatedBy = userId;
                
                // Por ahora, si no hay autenticación, usamos un valor por defecto
                if (orderDto.CreatedBy <= 0)
                {
                    orderDto.CreatedBy = 1; // Usuario por defecto
                }

                var result = await _orderService.UpdateOrderAsync(id, orderDto);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la orden con ID {id}");
                return StatusCode(500, "Error interno del servidor al actualizar la orden");
            }
        }

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la orden con ID {id}");
                return StatusCode(500, "Error interno del servidor al eliminar la orden");
            }
        }
    }
}
