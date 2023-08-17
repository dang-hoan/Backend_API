using Application.Interfaces.Repositories;

namespace Application.Interfaces.EnumMasterData
{
    public interface IEnumMasterDataRepository : IRepositoryAsync<Domain.Entities.EnumMasterData.EnumMasterData, int>
    {
    }
}
