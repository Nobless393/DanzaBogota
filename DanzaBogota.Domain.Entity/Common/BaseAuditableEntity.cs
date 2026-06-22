using DanzaBogota.Domain.Entity.Common.Interfaces;

namespace DanzaBogota.Domain.Entity.Common
{
    public abstract class BaseAuditableEntity<TKey> : BaseEntity<TKey>, IAuditableEntity where TKey : IEquatable<TKey>
    {
        /// <summary>Fecha de creación</summary>
        public DateTimeOffset Created { get; set; }
        /// <summary>Creado por</summary>
        public string? CreatedBy { get; set; }
        /// <summary>Última fecha de modificación</summary>
        public DateTimeOffset? LastModified { get; set; }
        /// <summary>Última modificación por</summary>
        public string? LastModifiedBy { get; set; }
    }
}