using Application.Interfaces;
using Application.Interfaces.EnumMasterData;

namespace Infrastructure.Services
{
    public class EnumService : IEnumService
    {
        private readonly IEnumMasterDataRepository _enumMasterDataRepository;

        public EnumService(IEnumMasterDataRepository enumMasterDataRepository)
        {
            _enumMasterDataRepository = enumMasterDataRepository;
        }

        public int GetEnumIdByValue(string value, string enumType)
        {
            var enumVar = _enumMasterDataRepository.Entities.Where(x => x.Value.Equals(value) && x.EnumType.Equals(enumType))
                                                            .FirstOrDefault();                 
            if(enumVar == null)             
                throw new Exception($"Database missing '{value}' value for {enumType.ToLower()}!");

            return enumVar.Id;
        }

        public bool CheckEnumExistsById(int id, string enumType)
        {
            var enumVar = _enumMasterDataRepository.Entities.Where(x => x.Id == id && x.EnumType.Equals(enumType))
                                                            .FirstOrDefault();                 
            if(enumVar == null)
                return false;

            return true;
        }
    }
}