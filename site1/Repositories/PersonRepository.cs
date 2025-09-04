using HelloApi.Models;
using HelloApi.Models.DTOs;
using MessageApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MessageApi.Repositories
{
    public class PersonRepository(AppDbContext context) : IPersonRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Person> AddPersonAsync(PersonCreateDto person)
        {
            var entity = new Person
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Email = person.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            _context.Persons.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return await _context.Persons.OrderBy(m => m.Id).ToListAsync();
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            return await _context.Persons.FindAsync(id);
        }

        public async Task<Person?> UpdatePersonAsync(Person person)
        {
            var existing = await _context.Persons.FindAsync(person.Id);
            if (existing == null) return null;
            existing.FirstName = person.FirstName;
            existing.LastName = person.LastName;
            existing.Email = person.Email;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            var entity = await _context.Messages.FindAsync(id);
            if (entity == null) return false;
            _context.Messages.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
