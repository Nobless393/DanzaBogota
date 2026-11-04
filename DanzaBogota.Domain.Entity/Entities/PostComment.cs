using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Identities;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class PostComment : BaseAuditableEntity<int>
    {
        public string Content { get; set; }

        // Relaciones
        public int PostId { get; set; }
        public Post Post { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
