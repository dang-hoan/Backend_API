using Application.Dtos.Responses.Audit;
using Domain.Wrappers;

namespace Application.Interfaces.Services
{
    public interface IAuditService
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId);
    }
}