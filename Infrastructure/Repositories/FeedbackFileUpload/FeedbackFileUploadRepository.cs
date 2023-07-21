using Application.Interfaces.FeedbackFileUpload;
using Application.Interfaces.Repositories;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.FeedbackFileUpload
{
    public class FeedbackFileUploadRepository : RepositoryAsync<Domain.Entities.FeebackFileUpload.FeedbackFileUpload, long>, IFeedbackFileUploadRepository
    {
        public FeedbackFileUploadRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
