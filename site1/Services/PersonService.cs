using HelloApi.Models;
using HelloApi.Models.DTOs;
using MessageApi.Repositories;

namespace HelloApi.Services
{
    public class PersonService(IPersonRepository repository) : IPersonService
    {
        private readonly IPersonRepository _repository = repository;

        public async Task<PersonReadDto> CreatePersonAsync(PersonCreateDto person)
        {
            var entity = await _repository.AddPersonAsync(person);
            return MapToReadDto(entity);
        }

        public async Task<IEnumerable<PersonReadDto>> GetAllPersonsAsync()
        {
            var entities = await _repository.GetAllPersonsAsync();
            return entities.Select(MapToReadDto);
        }

        public async Task<PersonReadDto?> GetPersonByIdAsync(int id)
        {
            var entity = await _repository.GetPersonByIdAsync(id);
            return entity == null ? null : MapToReadDto(entity);
        }

        public async Task<PersonReadDto?> UpdatePersonAsync(int id, PersonUpdateDto person)
        {
            var entity = new Person
            {
                Id = id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Email = person.Email
            };

            var updated = await _repository.UpdatePersonAsync(entity);
            return updated == null ? null : MapToReadDto(updated);
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            return await _repository.DeletePersonAsync(id);
        }

        private static PersonReadDto MapToReadDto(Person Person) => new()
        {
            Id = Person.Id,
            FirstName = Person.FirstName,
            LastName = Person.LastName,
            Email = Person.Email,
            CreatedAt = Person.CreatedAt,
            UpdatedAt = Person.UpdatedAt
        };

    }
}
