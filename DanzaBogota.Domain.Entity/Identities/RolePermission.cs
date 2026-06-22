using DanzaBogota.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanzaBogota.Domain.Entity.Identities
{
    /// <summary>
    /// Relación muchos a muchos entre roles y permisos.
    /// </summary>
    public class RolePermission : BaseEntity<int>
    {
        public DateTimeOffset Created { get; set; }

        // Relaciones
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
