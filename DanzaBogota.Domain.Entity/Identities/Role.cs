using Microsoft.AspNetCore.Identity;

namespace DanzaBogota.Domain.Entity.Identities
{
    /// <summary>
    /// Rol del sistema (Admin, Gestor, Grupo).
    /// </summary>
    public class Role : IdentityRole<int>
    {
        // Relaciones
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
