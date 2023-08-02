namespace Application.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string HostServerName { get; }
        string OriginRequest { get; }
        string RoleName { get; }
    }
}