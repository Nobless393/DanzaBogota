using DanzaBogota.Application.Dto.Event;
using DanzaBogota.Transversal.Common.Responses;

namespace DanzaBogota.Application.Interface
{
    public interface IEventApplication
    {
        Task<PaginatedResult<EventDto>> GetAllAsync(EventQueryParameters query);
        Task<PaginatedResult<EventDto>> GetPublishedAsync(EventQueryParameters query);
        Task<EventDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateEventDto dto, int createdByUserId);
        Task<int> UpdateAsync(UpdateEventDto dto);
        Task<bool> PublishAsync(int id);
        Task<bool> CancelAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<int> GetFinishedCountAsync();
    }
}
