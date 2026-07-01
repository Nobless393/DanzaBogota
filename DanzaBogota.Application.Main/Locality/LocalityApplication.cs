using DanzaBogota.Application.Dto.Locality;
using DanzaBogota.Application.Interface;

namespace DanzaBogota.Application.Main.Locality
{
    public class LocalityApplication : ILocalityApplication
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocalityApplication(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LocalityDto>> GetAllAsync()
        {
            var repository = _unitOfWork.Repository<DanzaBogota.Domain.Entity.Entities.Locality>();
            var localities = await repository.GetAllAsync("COMMON.Localities");

            return localities.Select(l => new LocalityDto
            {
                Id = l.Id,
                Name = l.Name
            });
        }

        public async Task<LocalityDto?> GetByIdAsync(int id)
        {
            var repository = _unitOfWork.Repository<DanzaBogota.Domain.Entity.Entities.Locality>();
            var locality = await repository.GetByIdAsync("COMMON.Localities", id);

            if (locality == null)
            {
                return null;
            }

            return new LocalityDto
            {
                Id = locality.Id,
                Name = locality.Name
            };
        }
    }
}
