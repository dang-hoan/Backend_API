using Application.Interfaces.View.ViewCustomerFeedbackReply;
using AutoMapper;
using Domain.Constants;
using Domain.Entities.View.ViewCustomerFeedbackReply;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Feedback.Queries.GetById
{
    public class GetFeedbackByIdQuery : IRequest<Result<GetFeedbackByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetFeedbackByIdQueryHandler : IRequestHandler<GetFeedbackByIdQuery, Result<GetFeedbackByIdResponse>>
    {
        private readonly IViewCustomerFeedbackReplyRepository _viewCustomerFeedbackReplyRepository;
        private readonly IMapper _mapper;

        public GetFeedbackByIdQueryHandler(IViewCustomerFeedbackReplyRepository viewCustomerFeedbackReplyRepository, IMapper mapper)
        {
            _viewCustomerFeedbackReplyRepository = viewCustomerFeedbackReplyRepository;
            _mapper = mapper;
        }

        public async Task<Result<GetFeedbackByIdResponse>> Handle(GetFeedbackByIdQuery request, CancellationToken cancellationToken)
        {
            var feedback = await _viewCustomerFeedbackReplyRepository.Entities.Where(_ => _.FeedbackId == request.Id)
                .Select(s => new ViewCustomerFeedbackReply
                {
                    FeedbackId = s.FeedbackId,
                    CustomerId = s.CustomerId,
                    CustomerName = s.CustomerName,
                    PhoneNumber = s.PhoneNumber,
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    FeedbackTitle = s.FeedbackTitle,
                    FeedbackServiceContent = s.FeedbackServiceContent,
                    FeedbackStaffContent = s.FeedbackStaffContent,
                    ReplyId = s.ReplyId,
                    ReplyTitle = s.ReplyTitle,
                    ReplyContent = s.ReplyContent,
                    Rating = s.Rating  
                })
                .FirstOrDefaultAsync();
            if (feedback == null)
            {
                return await Result<GetFeedbackByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }
            var feedbackResponse = _mapper.Map<GetFeedbackByIdResponse>(feedback);
            return await Result<GetFeedbackByIdResponse>.SuccessAsync(feedbackResponse);
        }
    }
}
