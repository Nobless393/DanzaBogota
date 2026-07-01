namespace DanzaBogota.Infrastructure.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
    }
}
