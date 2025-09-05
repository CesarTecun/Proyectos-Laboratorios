using Microsoft.AspNetCore.Mvc;
using MessageApi.Models.DTOs;
using MessageApi.Services;

namespace MessageApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones CRUD de mensajes.
    /// Proporciona endpoints para crear, leer, actualizar y eliminar mensajes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de mensajes.
        /// </summary>
        /// <param name="service">Servicio de mensajes inyectado por dependencia.</param>
        public MessageController(IMessageService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene todos los mensajes existentes.
        /// </summary>
        /// <returns>Una lista de todos los mensajes.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _service.GetAllMessagesAsync();
            return Ok(messages);
        }

        /// <summary>
        /// Obtiene un mensaje específico por su ID.
        /// </summary>
        /// <param name="id">ID del mensaje a buscar.</param>
        /// <returns>El mensaje encontrado o NotFound si no existe.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            var message = await _service.GetMessageByIdAsync(id);
            if (message == null) return NotFound();
            return Ok(message);
        }

        /// <summary>
        /// Crea un nuevo mensaje.
        /// </summary>
        /// <param name="dto">DTO con los datos del mensaje a crear.</param>
        /// <returns>El mensaje recién creado con su ID asignado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] MessageCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _service.CreateMessageAsync(dto.Message);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza un mensaje existente.
        /// </summary>
        /// <param name="id">ID del mensaje a actualizar.</param>
        /// <param name="dto">DTO con los nuevos datos del mensaje.</param>
        /// <returns>El mensaje actualizado o NotFound si no se encuentra.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] MessageUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _service.UpdateMessageAsync(id, dto.Message);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Elimina un mensaje existente.
        /// </summary>
        /// <param name="id">ID del mensaje a eliminar.</param>
        /// <returns>NoContent si se eliminó correctamente, o NotFound si no se encontró el mensaje.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteMessageAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}

