namespace Application.Interfaces
{
    public interface IEnumService
    {
        int GetEnumIdByValue(string value, string enumType);
        bool CheckEnumExistsById(int id, string enumType);
    }
}