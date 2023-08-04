using Application.Interfaces.WorkShift;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.WorkShift
{
    public class WorkShiftRepository : RepositoryAsync<Domain.Entities.WorkShift.WorkShift, long>, IWorkShiftRepository
    {
        public WorkShiftRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}