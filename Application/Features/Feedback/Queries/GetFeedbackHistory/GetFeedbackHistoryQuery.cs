using Application.Dtos.Responses.FeedbackFileUpload;
using Application.Dtos.Responses.ServiceImage;
using Application.Interfaces.Customer;
using Application.Interfaces.Feedback;
using Application.Interfaces.FeedbackFileUpload;
using Application.Interfaces.Reply;
using Application.Interfaces.ServiceImage;
using Application.Interfaces.View.ViewCustomerReviewHistory;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Application.Features.Feedback.Queries.GetHistoryFeedback
{
    public class GetFeedbackHistoryQuery : IRequest<Result<GetFeebackHistoryResponse>>
    {
        public long BookingId { get; set; }
    }
    internal class GetFeedbackHistoryQueryHandler : IRequestHandler<GetFeedbackHistoryQuery, Result<GetFeebackHistoryResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IViewCustomerReviewHisotyRepository _viewCustomerReviewHisotyRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly IFeedbackFileUploadRepository _feedbackFileUploadRepository;
        private readonly IServiceImageRepository _serviceImageRepository;

        public GetFeedbackHistoryQueryHandler(IMapper mapper, IViewCustomerReviewHisotyRepository viewCustomerReviewHisotyRepository,
            IReplyRepository replyRepository, IFeedbackFileUploadRepository feedbackFileUploadRepository,
            IServiceImageRepository serviceImageRepository)
        {
            _mapper = mapper;
            _viewCustomerReviewHisotyRepository = viewCustomerReviewHisotyRepository;
            _replyRepository = replyRepository;
            _feedbackFileUploadRepository = feedbackFileUploadRepository;
            _serviceImageRepository = serviceImageRepository;
        }
        public async Task<Result<GetFeebackHistoryResponse>> Handle(GetFeedbackHistoryQuery request, CancellationToken cancellationToken)
        {
            var cusotmerReviewHistories = await _viewCustomerReviewHisotyRepository.Entities.Where(_ => _.BookingId == request.BookingId)
                 .Select(s => new Domain.Entities.View.ViewCustomerReviewHistory.ViewCustomerReviewHistory
                 {
                     BookingId = s.BookingId,
                     ServiceId = s.ServiceId,
                     ServiceName = s.ServiceName,
                     CustomerId = s.CustomerId,
                     CustomerName = s.CustomerName,
                     FeedbackId = s.FeedbackId,
                     FeedbackTitle = s.FeedbackTitle,
                     FeedbackServiceContent = s.FeedbackServiceContent,
                     FeedbackStaffContent = s.FeedbackStaffContent,
                     ReplyId = s.ReplyId,
                     CreateOnFeedback = s.CreateOnFeedback,
                 }).ToListAsync();
            GetFeebackHistoryResponse response = new GetFeebackHistoryResponse();
            if(cusotmerReviewHistories != null)
            {
                foreach (var history in cusotmerReviewHistories)
                {
                    var customerHistoryResponse = new FeebackHistoryResponse()
                    {
                        ServiceId = history.ServiceId,
                        ServiceName = history.ServiceName,
                        CustomerId = history.CustomerId,
                        CustomerName = history.CustomerName,
                        FeebackId = history.FeedbackId,
                        FeedbackTitle = history.FeedbackTitle,
                        FeedbackServiceContent = history.FeedbackServiceContent,
                        FeedbackStaffContent = history.FeedbackStaffContent,
                        Rating = history.Rating,
                        CreatedOnFeedback = history.CreateOnFeedback,
                        ServiceImages = _mapper.Map<List<ServiceImageResponse>>(_serviceImageRepository.Entities.Where(_ => _.ServiceId == history.ServiceId && _.IsDeleted == false).ToList()),
                        FeedbackFileUploads = _mapper.Map<List<FeedbackFileUploadResponse>>(_feedbackFileUploadRepository.Entities.Where(_ => _.FeedbackId == history.FeedbackId && _.IsDeleted == false).ToList())
                    };
                    var reply = _replyRepository.Entities.Where(_ => _.Id == history.ReplyId && !_.IsDeleted)
                        .Select(s => new Domain.Entities.Reply.Reply
                        {
                            Id = s.Id,
                            FeedbackId = s.FeedbackId,
                            Title = s.Title,
                            Content = s.Content,
                            CreatedOn = s.CreatedOn,
                            LastModifiedOn = s.LastModifiedOn,
                        }).FirstOrDefault();
                    if(reply != null)
                    {
                        customerHistoryResponse.Reply = new ReplyResponse
                        {
                            Id = reply.Id,
                            ReplyTitle = reply.Title,
                            ReplyContent = reply.Content,
                            CreateOn = reply.CreatedOn,
                            LastModifiedOn = reply.LastModifiedOn,
                        };
                    }
                    response.feebackHistoryResponses.Add(customerHistoryResponse);
                }
            }
            return await Result<GetFeebackHistoryResponse>.SuccessAsync(response);
        }
    }
}

