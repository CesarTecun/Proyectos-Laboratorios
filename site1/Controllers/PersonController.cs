using Microsoft.AspNetCore.Mvc;
using HelloApi.Models.DTOs;
using HelloApi.Services;

namespace HelloApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController(IPersonService service) : ControllerBase
    {
        private readonly IPersonService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var persons = await _service.GetAllPersonsAsync();
            return Ok(persons);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _service.GetPersonByIdAsync(id);
            if (person == null) return NotFound();
            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonCreateDto dto)
        {
            var created = await _service.CreatePersonAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PersonUpdateDto dto)
        {
            var updated = await _service.UpdatePersonAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeletePersonAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
