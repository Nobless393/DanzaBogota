using DanzaBogota.Application.Dto.Category;

namespace DanzaBogota.Application.Interface
{
    public interface ICategoryApplication
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
    }
}
