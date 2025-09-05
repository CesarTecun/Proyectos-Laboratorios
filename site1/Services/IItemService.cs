using System.Collections.Generic;
using System.Threading.Tasks;
using MessageApi.Models.DTOs;

namespace MessageApi.Services
{
    public interface IItemService
    {
        Task<IEnumerable<ItemReadDto>> GetAllItemsAsync();
        Task<ItemReadDto> GetItemByIdAsync(int id);
        Task<ItemReadDto> CreateItemAsync(ItemCreateDto itemCreateDto);
        Task<bool> UpdateItemAsync(int id, ItemUpdateDto itemUpdateDto);
        Task<bool> DeleteItemAsync(int id);
    }
}
