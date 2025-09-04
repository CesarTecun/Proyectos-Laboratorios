using Microsoft.AspNetCore.Mvc;
using MessageApi.Services;
using MessageApi.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las órdenes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAll()
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

        /// <summary>
        /// Obtiene una orden por su ID
        /// </summary>
        /// <param name="id">ID de la orden</param>
        [HttpGet("{id}", Name = "GetOrderById")]
        public async Task<ActionResult<OrderReadDto>> GetById(int id)
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
                _logger.LogError(ex, $"Error al obtener la orden con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al obtener la orden con ID: {id}");
            }
        }

        /// <summary>
        /// Crea una nueva orden
        /// </summary>
        /// <param name="orderDto">Datos de la orden a crear</param>
        [HttpPost]
        public async Task<ActionResult<OrderReadDto>> Create(OrderCreateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdOrder = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtRoute(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden");
                return StatusCode(500, "Error interno del servidor al crear la orden");
            }
        }

        /// <summary>
        /// Actualiza una orden existente
        /// </summary>
        /// <param name="id">ID de la orden a actualizar</param>
        /// <param name="orderDto">Datos actualizados de la orden</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderUpdateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _orderService.UpdateOrderAsync(id, orderDto);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la orden con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al actualizar la orden con ID: {id}");
            }
        }

        /// <summary>
        /// Elimina una orden
        /// </summary>
        /// <param name="id">ID de la orden a eliminar</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
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
                _logger.LogError(ex, $"Error al eliminar la orden con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al eliminar la orden con ID: {id}");
            }
        }

        /// <summary>
        /// Obtiene las órdenes de una persona específica
        /// </summary>
        /// <param name="personId">ID de la persona</param>
        [HttpGet("person/{personId}")]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetByPersonId(int personId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByPersonIdAsync(personId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las órdenes de la persona con ID: {personId}");
                return StatusCode(500, $"Error interno del servidor al obtener las órdenes de la persona con ID: {personId}");
            }
        }
    }
}
