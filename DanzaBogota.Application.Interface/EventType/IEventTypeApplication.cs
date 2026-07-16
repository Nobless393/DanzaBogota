using DanzaBogota.Application.Dto.Event;

namespace DanzaBogota.Application.Interface.EventType
{
    public interface IEventTypeApplication
    {
        Task<IEnumerable<EventTypeDto>> GetAllAsync();
        Task<EventTypeDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(EventTypeDto dto);
        Task<int> UpdateAsync(EventTypeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
