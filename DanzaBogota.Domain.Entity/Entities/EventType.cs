using DanzaBogota.Domain.Entity.Common;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class EventType : BaseAuditableEntity<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        // Relaciones
        public ICollection<Event> Events { get; set; }
    }
}
