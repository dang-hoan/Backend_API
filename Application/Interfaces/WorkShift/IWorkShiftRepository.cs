using Application.Interfaces.Repositories;

namespace Application.Interfaces.WorkShift
{
    public interface IWorkShiftRepository : IRepositoryAsync<Domain.Entities.WorkShift.WorkShift, long>
    {
    }
}
