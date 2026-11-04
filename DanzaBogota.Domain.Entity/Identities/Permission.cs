using DanzaBogota.Domain.Entity.Common;

namespace DanzaBogota.Domain.Entity.Identities
{
    /// <summary>
    /// Permiso individual del sistema (ej: Events.Create, Groups.Read).
    /// </summary>
    public class Permission : BaseEntity<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        // Relaciones
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
