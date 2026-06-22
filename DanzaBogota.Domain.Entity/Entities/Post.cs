using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Identities;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class Post : BaseAuditableEntity<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }

        // Relaciones
        public int CommunityId { get; set; }
        public Community Community { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<PostComment> Comments { get; set; }
    }
}
