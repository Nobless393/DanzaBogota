using DanzaBogota.Application.Dto.Locality;

namespace DanzaBogota.Application.Interface
{
    public interface ILocalityApplication
    {
        Task<IEnumerable<LocalityDto>> GetAllAsync();
        Task<LocalityDto?> GetByIdAsync(int id);
    }
}
