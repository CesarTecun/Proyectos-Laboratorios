using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessageApi.Data;
using MessageApi.Models;
using MessageApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MessageApi.Services
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ItemService(AppDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ItemReadDto>> GetAllItemsAsync()
        {
            var items = await _context.Items.ToListAsync();
            return _mapper.Map<IEnumerable<ItemReadDto>>(items);
        }

        public async Task<ItemReadDto> GetItemByIdAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            return _mapper.Map<ItemReadDto>(item);
        }

        public async Task<ItemReadDto> CreateItemAsync(ItemCreateDto itemCreateDto)
        {
            var item = _mapper.Map<Item>(itemCreateDto);
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return _mapper.Map<ItemReadDto>(item);
        }

        public async Task<bool> UpdateItemAsync(int id, ItemUpdateDto itemUpdateDto)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return false;

            _mapper.Map(itemUpdateDto, item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
