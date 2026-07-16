using DanzaBogota.Application.Dto.Group;

namespace DanzaBogota.Application.Interface
{
    public interface IGroupApplication
    {
        Task<IEnumerable<GroupDto>> GetAllAsync();
        Task<IEnumerable<GroupDto>> GetMyGroupsAsync(int userId);
        Task<GroupDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateGroupDto dto, int userId);
        Task<int> UpdateAsync(UpdateGroupDto dto, int userId);
        Task<bool> UploadRutAsync(int groupId, int userId, Stream fileStream, string fileName);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
