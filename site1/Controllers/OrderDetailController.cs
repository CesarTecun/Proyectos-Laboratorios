using Microsoft.AspNetCore.Mvc;
using MessageApi.Services;
using MessageApi.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly ILogger<OrderDetailController> _logger;

        public OrderDetailController(IOrderDetailService orderDetailService, ILogger<OrderDetailController> logger)
        {
            _orderDetailService = orderDetailService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los detalles de una orden específica
        /// </summary>
        /// <param name="orderId">ID de la orden</param>
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderDetailReadDto>>> GetByOrderId(int orderId)
        {
            try
            {
                var orderDetails = await _orderDetailService.GetByOrderIdAsync(orderId);
                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los detalles de la orden con ID: {orderId}");
                return StatusCode(500, $"Error interno del servidor al obtener los detalles de la orden con ID: {orderId}");
            }
        }

        /// <summary>
        /// Obtiene un detalle de orden por su ID
        /// </summary>
        /// <param name="id">ID del detalle de orden</param>
        [HttpGet("{id}", Name = "GetOrderDetailById")]
        public async Task<ActionResult<OrderDetailReadDto>> GetById(int id)
        {
            try
            {
                var orderDetail = await _orderDetailService.GetOrderDetailByIdAsync(id);
                if (orderDetail == null)
                {
                    return NotFound();
                }
                return Ok(orderDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el detalle de orden con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al obtener el detalle de orden con ID: {id}");
            }
        }

        /// <summary>
        /// Agrega un nuevo detalle a una orden
        /// </summary>
        /// <param name="orderDetailDto">Datos del detalle a agregar</param>
        [HttpPost]
        public async Task<ActionResult<OrderDetailReadDto>> Create(OrderDetailCreateDto orderDetailDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdDetail = await _orderDetailService.AddOrderDetailAsync(orderDetailDto);
                return CreatedAtRoute(nameof(GetById), new { id = createdDetail.Id }, createdDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el detalle de orden");
                return StatusCode(500, "Error interno del servidor al crear el detalle de orden");
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un detalle de orden
        /// </summary>
        /// <param name="id">ID del detalle a actualizar</param>
        /// <param name="quantity">Nueva cantidad</param>
        [HttpPut("{id}/quantity")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return BadRequest("La cantidad debe ser mayor a cero");
                }

                var result = await _orderDetailService.UpdateOrderDetailQuantityAsync(id, quantity);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la cantidad del detalle con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al actualizar la cantidad del detalle con ID: {id}");
            }
        }

        /// <summary>
        /// Elimina un detalle de orden
        /// </summary>
        /// <param name="id">ID del detalle a eliminar</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _orderDetailService.RemoveOrderDetailAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el detalle con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al eliminar el detalle con ID: {id}");
            }
        }
    }
}
