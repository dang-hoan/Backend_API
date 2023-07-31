using Application.Interfaces.Repositories;

namespace Application.Interfaces.Reply
{
    public interface IReplyRepository : IRepositoryAsync<Domain.Entities.Reply.Reply,long>
    {
    }
}
