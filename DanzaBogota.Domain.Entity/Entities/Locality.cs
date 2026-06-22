using DanzaBogota.Domain.Entity.Common;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class Locality : BaseAuditableEntity<int>
    {
        public string Name { get; set; }

        // Relaciones
        public ICollection<Community> Communities { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}
