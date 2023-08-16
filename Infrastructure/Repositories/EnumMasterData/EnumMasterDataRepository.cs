using Application.Interfaces.EnumMasterData;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.EnumMasterData
{
    public class EnumMasterDataRepository : RepositoryAsync<Domain.Entities.EnumMasterData.EnumMasterData, int>, IEnumMasterDataRepository
    {
        public EnumMasterDataRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}