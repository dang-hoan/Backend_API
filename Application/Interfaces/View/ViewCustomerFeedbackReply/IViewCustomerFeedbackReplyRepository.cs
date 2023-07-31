using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.View.ViewCustomerFeedbackReply
{
    public interface IViewCustomerFeedbackReplyRepository : IRepositoryAsync<Domain.Entities.View.ViewCustomerFeedbackReply.ViewCustomerFeedbackReply, long>
    {
    }
}
