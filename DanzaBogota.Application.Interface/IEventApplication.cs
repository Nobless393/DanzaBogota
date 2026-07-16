using DanzaBogota.Application.Dto.Event;

namespace DanzaBogota.Application.Interface
{
    public interface IEventApplication
    {
        Task<IEnumerable<EventDto>> GetAllAsync();
        Task<IEnumerable<EventDto>> GetPublishedAsync();
        Task<EventDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateEventDto dto, int createdByUserId);
        Task<int> UpdateAsync(UpdateEventDto dto);
        Task<bool> PublishAsync(int id);
        Task<bool> CancelAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
