using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Identities;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class EventCommentLike : BaseEntity<int>
    {
        // Relaciones
        public int CommentId { get; set; }
        public EventComment Comment { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
