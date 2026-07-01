namespace DanzaBogota.Infrastructure.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string tableName);
        Task<T?> GetByIdAsync(string tableName, int id);
        Task<int> InsertAsync(string sql, object parameters);
        Task<int> UpdateAsync(string sql, object parameters);
        Task<int> DeleteAsync(string tableName, int id);
    }
}
