using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Identities;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class CommunityMember : BaseEntity<int>
    {
        public DateTimeOffset Created { get; set; }

        // Relaciones
        public int CommunityId { get; set; }
        public Community Community { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
