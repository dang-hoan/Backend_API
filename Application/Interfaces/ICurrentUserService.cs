namespace Application.Interfaces
{
    public interface ICurrentUserService
    {
        string Username { get; }
        string RoleName { get; }
    }
}