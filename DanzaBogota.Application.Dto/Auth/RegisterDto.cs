namespace DanzaBogota.Application.Dto.Auth
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        /// <summary>Rol solicitado al registrarse: "Grupo" (por defecto) o "Gestor".</summary>
        public string? Role { get; set; }
    }
}
