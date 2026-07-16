using DanzaBogota.Application.Dto.User;

namespace DanzaBogota.Application.Interface
{
    public interface IUserApplication
    {
        Task<bool> ChangeRoleAsync(ChangeRoleDto dto);
        Task<IEnumerable<UserDto>> GetAllAsync();
    }
}
