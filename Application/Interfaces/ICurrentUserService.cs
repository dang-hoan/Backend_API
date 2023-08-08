namespace Application.Interfaces
{
    public interface ICurrentUserService
    {
        string UserName { get; }
        string HostServerName { get; }
        string OriginRequest { get; }
        string RoleName { get; }
    }
}