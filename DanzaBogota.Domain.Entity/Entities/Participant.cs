using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Enums;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class Participant : BaseAuditableEntity<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public int Age { get; set; }
        public DateOnly BirthDate { get; set; }
        public bool IsActive { get; set; }

        // Relaciones
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<ApplicationParticipant> ApplicationParticipants { get; set; }
    }
}
