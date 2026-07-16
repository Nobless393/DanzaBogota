using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Identities;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class Group : BaseAuditableEntity<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string DirectorName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? SocialMediaUrl { get; set; }
        public string? RutUrl { get; set; }
        public bool UseDirectorRut { get; set; }
        public bool IsActive { get; set; }

        // Relaciones
        public int LocalityId { get; set; }
        public Locality Locality { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Participant> Participants { get; set; }
        public ICollection<EventApplication> Applications { get; set; }
    }
}
