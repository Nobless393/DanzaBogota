using DanzaBogota.Application.Dto.Auth;

namespace DanzaBogota.Application.Interface
{
    public interface IAuthApplication
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}
