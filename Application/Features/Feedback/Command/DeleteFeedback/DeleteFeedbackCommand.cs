using Application.Exceptions;
using Application.Interfaces.Feedback;
using Application.Interfaces.Reply;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

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
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var deleteFeedback = await _feedbackRepository.FindAsync(_ => _.Id == request.Id && !_.IsDeleted) ?? throw new ApiException(StaticVariable.NOT_FOUND_MSG);
                var deleteReply = await _replyRepository.FindAsync(_ => _.Id == deleteFeedback.ReplyId && !_.IsDeleted);
                if (deleteReply != null)
                {
                    await _replyRepository.DeleteAsync(deleteReply);
                }
                await _feedbackRepository.DeleteAsync(deleteFeedback);

                await transaction.CommitAsync();
                return await Result<long>.SuccessAsync(request.Id, $"Delete feedback by id successfully!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ApiException(ex.Message);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}