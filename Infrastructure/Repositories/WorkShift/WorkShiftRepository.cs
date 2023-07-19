using Application.Interfaces.Customer;
using Application.Interfaces.WorkShift;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.WorkShift
{
    public class WorkShiftRepository : RepositoryAsync<Domain.Entities.WorkShift.WorkShift, long>, IWorkShiftRepository
    {
        public WorkShiftRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
