using DanzaBogota.Application.Dto.Auth;
using DanzaBogota.Application.Interface;
using DanzaBogota.Domain.Entity.Identities;
using DanzaBogota.Transversal.Common.Jwt;
using Microsoft.AspNetCore.Identity;

namespace DanzaBogota.Application.Main.Auth
{
    public class AuthApplication : IAuthApplication
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthApplication(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = "El correo ya está registrado."
                };
            }

            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                DocumentType = registerDto.DocumentType,
                DocumentNumber = registerDto.DocumentNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = errors
                };
            }

            // Asignar rol por defecto "Grupo" a todo usuario nuevo
            await _userManager.AddToRoleAsync(user, "Grupo");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = "Correo o contraseña incorrectos."
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = "Correo o contraseña incorrectos."
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles
            };
        }
    }
}
