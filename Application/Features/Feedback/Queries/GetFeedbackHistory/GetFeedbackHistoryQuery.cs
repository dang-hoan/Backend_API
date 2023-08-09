using Application.Dtos.Responses.FeedbackFileUpload;
using Application.Dtos.Responses.ServiceImage;
using Application.Interfaces;
using Application.Interfaces.FeedbackFileUpload;
using Application.Interfaces.Reply;
using Application.Interfaces.ServiceImage;
using Application.Interfaces.View.ViewCustomerReviewHistory;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Application.Features.Feedback.Queries.GetHistoryFeedback
{
    public class GetFeedbackHistoryQuery : IRequest<Result<List<GetFeebackHistoryResponse>>>
    {
        public long BookingId { get; set; }
    }

    internal class GetFeedbackHistoryQueryHandler : IRequestHandler<GetFeedbackHistoryQuery, Result<List<GetFeebackHistoryResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IViewCustomerReviewHisotyRepository _viewCustomerReviewHisotyRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly IFeedbackFileUploadRepository _feedbackFileUploadRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly IUploadService _uploadService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public GetFeedbackHistoryQueryHandler(IMapper mapper, IViewCustomerReviewHisotyRepository viewCustomerReviewHisotyRepository,
            IReplyRepository replyRepository, IFeedbackFileUploadRepository feedbackFileUploadRepository,
            IServiceImageRepository serviceImageRepository, IUploadService uploadService,
            ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _viewCustomerReviewHisotyRepository = viewCustomerReviewHisotyRepository;
            _replyRepository = replyRepository;
            _feedbackFileUploadRepository = feedbackFileUploadRepository;
            _serviceImageRepository = serviceImageRepository;
            _uploadService = uploadService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<List<GetFeebackHistoryResponse>>> Handle(GetFeedbackHistoryQuery request, CancellationToken cancellationToken)
        {
            var customerReviewHistories = await _viewCustomerReviewHisotyRepository.Entities.Where(_ => _.BookingId == request.BookingId)
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
            List<GetFeebackHistoryResponse> response = new List<GetFeebackHistoryResponse>();
            if (customerReviewHistories != null)
            {
                long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                if (userId != customerReviewHistories.First().CustomerId)
                    return await Result<List<GetFeebackHistoryResponse>>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);

                foreach (var history in customerReviewHistories)
                {
                    var customerHistoryResponse = new GetFeebackHistoryResponse()
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
                        ServiceImages = _mapper.Map<List<ServiceImageResponse>>(_serviceImageRepository.Entities.Where(x => x.ServiceId == history.ServiceId && !x.IsDeleted).ToList()),
                        FeedbackFileUploads = _mapper.Map<List<FeedbackFileUploadResponse>>(_feedbackFileUploadRepository.Entities.Where(x => x.FeedbackId == history.FeedbackId && !x.IsDeleted).ToList())
                    };
                    foreach (ServiceImageResponse serviceImageResponse in customerHistoryResponse.ServiceImages)
                    {
                        serviceImageResponse.NameFile = _uploadService.GetFullUrl(serviceImageResponse.NameFile);
                    }
                    foreach (FeedbackFileUploadResponse feedbackFileUploadResponse in customerHistoryResponse.FeedbackFileUploads)
                    {
                        feedbackFileUploadResponse.NameFile = _uploadService.GetFullUrl(feedbackFileUploadResponse.NameFile);
                    }

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
                    if (reply != null)
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
                    response.Add(customerHistoryResponse);
                }
            }
            return await Result<List<GetFeebackHistoryResponse>>.SuccessAsync(response);
        }
    }
}