namespace DanzaBogota.Domain.Entity.Common.Interfaces
{
    public interface IAuditableEntity
    {
        /// <summary>Fecha de creación</summary>
        DateTimeOffset Created { get; set; }
        /// <summary>Creado por</summary>
        string? CreatedBy { get; set; }
        /// <summary>Última fecha de modificación</summary>
        DateTimeOffset? LastModified { get; set; }
        /// <summary>Última modificación por</summary>
        string? LastModifiedBy { get; set; }
    }
}
