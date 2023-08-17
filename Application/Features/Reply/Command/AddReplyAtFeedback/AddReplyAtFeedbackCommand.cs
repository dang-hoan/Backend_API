using Application.Interfaces.Feedback;
using Application.Interfaces.Reply;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Reply.Command.AddReplyAtFeeback
{
    public class AddReplyAtFeedbackCommand : IRequest<Result<AddReplyAtFeedbackCommand>>
    {
        public long? Id { get; set; }
        public long FeedbackId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
    }
    internal class AddReplyAtFeebackCommandHandler : IRequestHandler<AddReplyAtFeedbackCommand, Result<AddReplyAtFeedbackCommand>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly IMapper _mapper;

        public AddReplyAtFeebackCommandHandler(IUnitOfWork<long> unitOfWork ,IFeedbackRepository feedbackRepository,IReplyRepository replyRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _feedbackRepository = feedbackRepository;
            _replyRepository = replyRepository;
            _mapper = mapper;
        }
        public async Task<Result<AddReplyAtFeedbackCommand>> Handle(AddReplyAtFeedbackCommand request, CancellationToken cancellationToken)
        {
            request.Id = null;
            if(request.FeedbackId == 0)
            {
                return await Result<AddReplyAtFeedbackCommand>.FailAsync("The field FeedbackId is required.");
            }
            var feeback = await _feedbackRepository.FindAsync(_ => _.Id == request.FeedbackId && !_.IsDeleted);
            if (feeback == null) return await Result<AddReplyAtFeedbackCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            if(feeback.ReplyId != null)
            {
                return await Result<AddReplyAtFeedbackCommand>.FailAsync("This feedback has been replied to.");
            }
            var addReply = _mapper.Map<Domain.Entities.Reply.Reply>(request);
            await _replyRepository.AddAsync(addReply);
            await _unitOfWork.Commit(cancellationToken);
            feeback.ReplyId = addReply.Id;
            request.Id = addReply.Id;
            await _feedbackRepository.UpdateAsync(feeback);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<AddReplyAtFeedbackCommand>.SuccessAsync(request);
        }
    }
}
