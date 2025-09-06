using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Controlador para gestionar las operaciones CRUD de productos.
    /// Proporciona endpoints para crear, leer, actualizar y eliminar productos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(
            IItemService itemService,
            ILogger<ProductController> logger,
            IMapper mapper)
        {
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Obtiene todos los productos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAll()
        {
            try
            {
                var items = await _itemService.GetAllItemsAsync();
                // Map ItemReadDto -> ProductReadDto to preserve response shape
                var products = items.Select(i => new ProductReadDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    Description = i.Description,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                });
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos");
                return StatusCode(500, "Error interno del servidor al obtener los productos");
            }
        }

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<ProductReadDto>> GetById(int id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                var productDto = new ProductReadDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Description = item.Description,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt
                };
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el producto con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al obtener el producto con ID: {id}");
            }
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> Create(ProductCreateDto productCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Map ProductCreateDto -> ItemCreateDto (Stock default 0)
                var itemCreate = new ItemCreateDto
                {
                    Name = productCreateDto.Name,
                    Price = productCreateDto.Price,
                    Description = productCreateDto.Description,
                    Stock = 0
                };
                var itemRead = await _itemService.CreateItemAsync(itemCreate);
                var productReadDto = new ProductReadDto
                {
                    Id = itemRead.Id,
                    Name = itemRead.Name,
                    Price = itemRead.Price,
                    Description = itemRead.Description,
                    CreatedAt = itemRead.CreatedAt,
                    UpdatedAt = itemRead.UpdatedAt
                };
                return CreatedAtAction(nameof(GetById), new { id = productReadDto.Id }, productReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto");
                return StatusCode(500, "Error interno del servidor al crear el producto");
            }
        }

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductUpdateDto productUpdateDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID de producto inválido");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Map ProductUpdateDto -> ItemUpdateDto
                var itemUpdate = new ItemUpdateDto
                {
                    Id = id,
                    Name = productUpdateDto.Name,
                    Price = productUpdateDto.Price,
                    Description = productUpdateDto.Description,
                    Stock = productUpdateDto.Stock
                };
                var updated = await _itemService.UpdateItemAsync(id, itemUpdate);
                if (!updated)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el producto con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al actualizar el producto con ID: {id}");
            }
        }

        /// <summary>
        /// Elimina un producto por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                await _itemService.DeleteItemAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el producto con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al eliminar el producto con ID: {id}");
            }
        }
    }
}
