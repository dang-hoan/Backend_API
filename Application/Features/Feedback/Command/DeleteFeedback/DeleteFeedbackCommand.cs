using Application.Interfaces.Feedback;
using Application.Interfaces.Reply;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Feedback.Command.DeleteFeedback
{
    public class DeleteFeedbackCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }
    }
    internal class DeleteFeedbackCommandHandler : IRequestHandler<DeleteFeedbackCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IReplyRepository _replyRepository;
        
        public DeleteFeedbackCommandHandler(IUnitOfWork<long> unitOfWork, IFeedbackRepository feedbackRepository, IReplyRepository replyRepository)
        {
            _unitOfWork = unitOfWork;
            _feedbackRepository = feedbackRepository;
            _replyRepository = replyRepository;
        }
        public async Task<Result<long>> Handle(DeleteFeedbackCommand request, CancellationToken cancellationToken)
        {
            var deleteFeedback = await _feedbackRepository.FindAsync(_ => _.Id == request.Id && !_.IsDeleted);
            if (deleteFeedback == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            var deleteReply = await _replyRepository.FindAsync(_ => _.Id == deleteFeedback.ReplyId && !_.IsDeleted);
            if (deleteReply != null)
            {
                await _replyRepository.DeleteAsync(deleteReply);
                await _unitOfWork.Commit(cancellationToken);
            }
            await _feedbackRepository.DeleteAsync(deleteFeedback);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<long>.SuccessAsync(request.Id, $"Delete feedback by id successfully!");
        }
    }
}
