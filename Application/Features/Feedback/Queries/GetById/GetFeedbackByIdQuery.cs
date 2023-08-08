using Application.Exceptions;
using Application.Interfaces.View.ViewCustomerFeedbackReply;
using AutoMapper;
using Domain.Constants;
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
        public GetFeedbackByIdQueryHandler(IViewCustomerFeedbackReplyRepository viewCustomerFeedbackReplyRepository)
        {
            _viewCustomerFeedbackReplyRepository = viewCustomerFeedbackReplyRepository;
        }

        public async Task<Result<GetFeedbackByIdResponse>> Handle(GetFeedbackByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await (from s in _viewCustomerFeedbackReplyRepository.Entities
                                where s.FeedbackId == request.Id
                                select new GetFeedbackByIdResponse()
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
                                }).FirstOrDefaultAsync();
            if (result == null) throw new ApiException(StaticVariable.NOT_FOUND_MSG);
            return await Result<GetFeedbackByIdResponse>.SuccessAsync(result);
        }
    }
}