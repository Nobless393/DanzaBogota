using DanzaBogota.Application.Dto.User;
using DanzaBogota.Application.Interface;
using Microsoft.AspNetCore.Identity;

namespace DanzaBogota.Application.Main.User
{
    public class UserApplication : IUserApplication
    {
        private readonly UserManager<Domain.Entity.Identities.User> _userManager;

        public UserApplication(UserManager<Domain.Entity.Identities.User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ChangeRoleAsync(ChangeRoleDto dto)
        {
            var validRoles = new[] { "Admin", "Gestor", "Grupo" };

            if (!validRoles.Contains(dto.NewRole))
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.NewRole);

            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    DocumentType = user.DocumentType,
                    DocumentNumber = user.DocumentNumber,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    Roles = roles
                });
            }

            return result;
        }
    }
}
