using Application.Interfaces.EnumMasterData;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.EnumMasterData
{
    public class EnumMasterDataRepository : RepositoryAsync<Domain.Entities.EnumMasterData.EnumMasterData, long>, IEnumMasterDataRepository
    {
        public EnumMasterDataRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}