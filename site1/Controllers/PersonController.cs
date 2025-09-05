using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MessageApi.Models.DTOs;
using MessageApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessageApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones CRUD de personas (clientes).
    /// Proporciona endpoints para crear, leer, actualizar y eliminar personas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly ILogger<PersonController> _logger;
        private readonly IMapper _mapper;

        public PersonController(
            IPersonService personService,
            ILogger<PersonController> logger,
            IMapper mapper)
        {
            _personService = personService ?? throw new ArgumentNullException(nameof(personService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Obtiene todas las personas registradas en el sistema.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PersonReadDto>>> GetAllPersons()
        {
            try
            {
                var persons = await _personService.GetAllPersonsAsync();
                return Ok(_mapper.Map<IEnumerable<PersonReadDto>>(persons));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de personas");
                return StatusCode(500, "Error interno del servidor al obtener las personas");
            }
        }

        /// <summary>
        /// Obtiene una persona por su ID.
        /// </summary>
        /// <param name="id">ID de la persona a buscar.</param>
        [HttpGet("{id}", Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonReadDto>> GetPersonById(int id)
        {
            try
            {
                var person = await _personService.GetPersonByIdAsync(id);
                if (person == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<PersonReadDto>(person));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la persona con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al obtener la persona con ID: {id}");
            }
        }

        /// <summary>
        /// Crea una nueva persona en el sistema.
        /// </summary>
        /// <param name="personDto">Datos de la persona a crear.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonReadDto>> CreatePerson([FromBody] PersonCreateDto personDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdPerson = await _personService.CreatePersonAsync(personDto);
                var personReadDto = _mapper.Map<PersonReadDto>(createdPerson);
                
                return CreatedAtRoute(nameof(GetPersonById), new { id = personReadDto.Id }, personReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva persona");
                return StatusCode(500, "Error interno del servidor al crear la persona");
            }
        }

        /// <summary>
        /// Actualiza una persona existente.
        /// </summary>
        /// <param name="id">ID de la persona a actualizar.</param>
        /// <param name="personDto">Datos actualizados de la persona.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonUpdateDto personDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedPerson = await _personService.UpdatePersonAsync(id, personDto);
                if (updatedPerson == null)
                {
                    return NotFound();
                }

                return Ok(updatedPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la persona con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al actualizar la persona con ID: {id}");
            }
        }

        /// <summary>
        /// Elimina una persona del sistema.
        /// </summary>
        /// <param name="id">ID de la persona a eliminar.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                var result = await _personService.DeletePersonAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la persona con ID: {id}");
                return StatusCode(500, $"Error interno del servidor al eliminar la persona con ID: {id}");
            }
        }
    }
}
