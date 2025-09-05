using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MessageApi.Models;
using MessageApi.Models.DTOs;
using MessageApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessageApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones CRUD de ítems (productos).
    /// Proporciona endpoints para listar, crear, actualizar y eliminar ítems.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemController> _logger;
        private readonly IMapper _mapper;

        public ItemController(
            IItemService itemService,
            ILogger<ItemController> logger,
            IMapper mapper)
        {
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Obtiene todos los ítems disponibles en el sistema.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ItemReadDto>>> GetAllItems()
        {
            try
            {
                var items = await _itemService.GetAllItemsAsync();
                return Ok(_mapper.Map<IEnumerable<ItemReadDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de ítems");
                return StatusCode(500, "Error interno del servidor al obtener los ítems");
            }
        }

        /// <summary>
        /// Obtiene un ítem por su ID.
        /// </summary>
        /// <param name="id">ID del ítem a buscar.</param>
        [HttpGet("{id}", Name = "GetItemById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ItemReadDto>> GetItemById(int id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<ItemReadDto>(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el ítem con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al obtener el ítem con ID: {id}");
            }
        }

        /// <summary>
        /// Crea un nuevo ítem en el sistema.
        /// </summary>
        /// <param name="itemDto">Datos del ítem a crear.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ItemReadDto>> CreateItem([FromBody] ItemCreateDto itemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var itemReadDto = await _itemService.CreateItemAsync(itemDto);
                
                return CreatedAtRoute(nameof(GetItemById), new { id = itemReadDto.Id }, itemReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo ítem");
                return StatusCode(500, "Error interno del servidor al crear el ítem");
            }
        }

        /// <summary>
        /// Actualiza un ítem existente.
        /// </summary>
        /// <param name="id">ID del ítem a actualizar.</param>
        /// <param name="itemDto">Datos actualizados del ítem.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemUpdateDto itemDto)
        {
            try
            {
                if (id != itemDto.Id)
                {
                    return BadRequest("El ID de la ruta no coincide con el ID del ítem");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _itemService.UpdateItemAsync(id, itemDto);
                
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el ítem con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al actualizar el ítem con ID: {id}");
            }
        }

        /// <summary>
        /// Elimina un ítem del sistema.
        /// </summary>
        /// <param name="id">ID del ítem a eliminar.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var result = await _itemService.DeleteItemAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el ítem con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al eliminar el ítem con ID: {id}");
            }
        }
    }
}
