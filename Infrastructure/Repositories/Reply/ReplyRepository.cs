using Application.Interfaces.Reply;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.Reply
{
    public class ReplyRepository : RepositoryAsync<Domain.Entities.Reply.Reply, long>, IReplyRepository
    {
        public ReplyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}