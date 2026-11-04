using DanzaBogota.Application.Dto.Participant;

namespace DanzaBogota.Application.Interface
{
    public interface IParticipantApplication
    {
        Task<IEnumerable<ParticipantDto>> GetByGroupAsync(int groupId, int userId);
        Task<ParticipantDto?> GetByIdAsync(int id, int userId);
        Task<int> CreateAsync(CreateParticipantDto dto, int userId);
        Task<int> UpdateAsync(UpdateParticipantDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
