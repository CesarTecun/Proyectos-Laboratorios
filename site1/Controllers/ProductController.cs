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
    /// Controlador para gestionar las operaciones CRUD de productos.
    /// Proporciona endpoints para crear, leer, actualizar y eliminar productos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(
            IProductService productService,
            ILogger<ProductController> logger,
            IMapper mapper)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
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
                var products = await _productService.GetAllProductsAsync();
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
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<ProductReadDto>(product));
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

                var product = await _productService.CreateProductAsync(productCreateDto);
                var productReadDto = _mapper.Map<ProductReadDto>(product);
                return CreatedAtRoute(nameof(GetById), new { id = productReadDto.Id }, productReadDto);
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

                var updatedProduct = await _productService.UpdateProductAsync(id, productUpdateDto);
                if (updatedProduct == null)
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
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                await _productService.DeleteProductAsync(id);
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
