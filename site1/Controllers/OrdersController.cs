using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MessageApi.Models.DTOs;
using MessageApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelloApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones CRUD de órdenes.
    /// Proporciona endpoints para crear, leer, actualizar y eliminar órdenes,
    /// así como para gestionar los detalles de las mismas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de órdenes.
        /// </summary>
        /// <param name="orderService">Servicio de órdenes inyectado por dependencia.</param>
        /// <param name="logger">Logger para el registro de eventos.</param>
        /// <param name="mapper">Mapeador para transformar entre entidades y DTOs.</param>
        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger,
            IMapper mapper)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Obtiene todas las órdenes existentes en el sistema.
        /// </summary>
        /// <returns>Una lista de todas las órdenes con sus detalles.</returns>
        /// <response code="200">Retorna la lista de órdenes.</response>
        /// <response code="500">Error interno del servidor al procesar la solicitud.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("Obteniendo todas las órdenes");
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
        /// Obtiene una orden específica por su ID.
        /// </summary>
        /// <param name="id">ID de la orden a buscar.</param>
        /// <returns>La orden solicitada o NotFound si no existe.</returns>
        /// <response code="200">Retorna la orden solicitada.</response>
        /// <response code="404">No se encontró la orden con el ID especificado.</response>
        /// <response code="500">Error interno del servidor al procesar la solicitud.</response>
        [HttpGet("{id}", Name = "GetOrderById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderReadDto>> GetOrderById(int id)
        {
            try
            {
                _logger.LogInformation($"Buscando orden con ID: {id}");
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning($"No se encontró la orden con ID: {id}");
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

        /// <summary>
        /// Crea una nueva orden en el sistema.
        /// </summary>
        /// <param name="orderDto">DTO con los datos de la orden a crear.</param>
        /// <returns>La orden recién creada con su ID asignado.</returns>
        /// <response code="201">Retorna la orden creada.</response>
        /// <response code="400">Los datos de la orden no son válidos.</response>
        /// <response code="500">Error interno del servidor al procesar la solicitud.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderReadDto>> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de nueva orden");
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Datos de orden no válidos. Errores: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                // TODO: Implementar autenticación
                // var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                // orderDto.CreatedBy = userId;
                
                // Usuario por defecto hasta implementar autenticación
                if (orderDto.CreatedBy <= 0)
                {
                    orderDto.CreatedBy = 1; // Usuario por defecto
                    _logger.LogInformation("Usando usuario por defecto (ID: 1) para la orden");
                }

                var order = await _orderService.CreateOrderAsync(orderDto);
                _logger.LogInformation($"Orden creada exitosamente con ID: {order.Id}");
                
                return CreatedAtRoute(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden");
                return StatusCode(500, "Error interno del servidor al crear la orden");
            }
        }

        /// <summary>
        /// Actualiza una orden existente.
        /// </summary>
        /// <param name="id">ID de la orden a actualizar.</param>
        /// <param name="orderDto">DTO con los nuevos datos de la orden.</param>
        /// <returns>NoContent si la actualización fue exitosa, o NotFound si la orden no existe.</returns>
        /// <response code="204">La orden se actualizó correctamente.</response>
        /// <response code="400">Los datos de la orden no son válidos o hay un error en la solicitud.</response>
        /// <response code="404">No se encontró la orden con el ID especificado.</response>
        /// <response code="500">Error interno del servidor al procesar la solicitud.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderCreateDto orderDto)
        {
            try
            {
                _logger.LogInformation($"Iniciando actualización de orden con ID: {id}");
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Datos de orden no válidos para actualización. Errores: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                // TODO: Implementar autenticación
                // var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                // orderDto.CreatedBy = userId;
                
                // Usuario por defecto hasta implementar autenticación
                if (orderDto.CreatedBy <= 0)
                {
                    orderDto.CreatedBy = 1; // Usuario por defecto
                    _logger.LogInformation("Usando usuario por defecto (ID: 1) para la actualización");
                }

                var result = await _orderService.UpdateOrderAsync(id, orderDto);
                if (!result)
                {
                    _logger.LogWarning($"No se encontró la orden con ID: {id} para actualizar");
                    return NotFound();
                }

                _logger.LogInformation($"Orden con ID: {id} actualizada exitosamente");
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la orden con ID {id}");
                return StatusCode(500, "Error interno del servidor al actualizar la orden");
            }
        }

        /// <summary>
        /// Elimina una orden existente.
        /// </summary>
        /// <param name="id">ID de la orden a eliminar.</param>
        /// <returns>NoContent si la eliminación fue exitosa, o NotFound si la orden no existe.</returns>
        /// <response code="204">La orden se eliminó correctamente.</response>
        /// <response code="404">No se encontró la orden con el ID especificado.</response>
        /// <response code="500">Error interno del servidor al procesar la solicitud.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                _logger.LogInformation($"Iniciando eliminación de orden con ID: {id}");
                
                var result = await _orderService.DeleteOrderAsync(id);
                if (!result)
                {
                    _logger.LogWarning($"No se encontró la orden con ID: {id} para eliminar");
                    return NotFound();
                }

                _logger.LogInformation($"Orden con ID: {id} eliminada exitosamente");
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

