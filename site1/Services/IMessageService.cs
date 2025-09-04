using MessageApi.Models.DTOs;

namespace HelloApi.Services
{
    public interface IMessageService
    {
        Task<MessageReadDto> CreateMessageAsync(string message);
        Task<IEnumerable<MessageReadDto>> GetAllMessagesAsync();
        Task<MessageReadDto?> GetMessageByIdAsync(int id);
        Task<MessageReadDto?> UpdateMessageAsync(int id, string message);
        Task<bool> DeleteMessageAsync(int id);
    }
}
