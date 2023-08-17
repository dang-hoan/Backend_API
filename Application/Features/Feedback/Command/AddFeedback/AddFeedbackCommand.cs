using Application.Dtos.Requests.Feedback;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Feedback;
using Application.Interfaces.FeedbackFileUpload;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Entities;
using Domain.Entities.FeedbackFileUpload;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Feedback.Command.AddFeedback
{
    public class AddFeedbackCommand : IRequest<Result<AddFeedbackCommand>>
    {
        public int Rating { get; set; } = default!;
        public long BookingDetailId { get; set; } = default!;
        public string? StaffContent { get; set; }
        public string? ServiceContent { get; set; }
        public List<ImageRequest>? FeedbackImageRequests { get; set; } // Hình ảnh mô tả (File)
    }

    internal class AddFeedbackCommandHandler : IRequestHandler<AddFeedbackCommand, Result<AddFeedbackCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IFeedbackFileUploadRepository _feedbackFileUploadRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public AddFeedbackCommandHandler(
            IMapper mapper, IFeedbackRepository feedbackRepository,
            IFeedbackFileUploadRepository feedbackFileUploadRepository,
            IBookingDetailRepository bookingDetailRepository,
            IBookingRepository bookingRepository,
            IEnumService enumService,
            IUnitOfWork<long> unitOfWork,
            ICurrentUserService currentUserService,
            UserManager<AppUser> userManager
         )
        {
            _mapper = mapper;
            _feedbackRepository = feedbackRepository;
            _feedbackFileUploadRepository = feedbackFileUploadRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _enumService = enumService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<AddFeedbackCommand>> Handle(AddFeedbackCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var isExistInBookingDetail = await _bookingDetailRepository.FindAsync(x => x.Id == request.BookingDetailId && !x.IsDeleted);

                if(isExistInBookingDetail == null) return await Result<AddFeedbackCommand>.FailAsync(StaticVariable.NOT_FOUND_BOOKING_DETAIL);
                var isExistBookingFeedback = await _feedbackRepository.FindAsync(x => x.BookingDetailId == request.BookingDetailId && !x.IsDeleted);
                if (request.Rating < (int)Rating.OneStar || request.Rating > (int)Rating.FiveStars)
                {
                    return await Result<AddFeedbackCommand>.FailAsync("Rating must be from 1 to 5 star");
                }
                if (isExistBookingFeedback != null)
                {
                    return await Result<AddFeedbackCommand>.FailAsync("You rated this feedback");
                }
                var addFeedback = _mapper.Map<Domain.Entities.Feedback.Feedback>(request);

                long customerId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                var isCustomerHasBooking = _bookingRepository.Entities.Where(x => x.CustomerId == customerId && x.Id == isExistInBookingDetail.BookingId && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)).Select(x => x.Id);

                var isWaitingBooking = _bookingRepository.Entities.Where(x => x.CustomerId == customerId &&
                                        x.Id == isExistInBookingDetail.BookingId &&
                                        x.Status == _enumService.GetEnumIdByValue(StaticVariable.WAITING, StaticVariable.BOOKING_STATUS_ENUM));

                if (isWaitingBooking.Any())
                {
                    return await Result<AddFeedbackCommand>.FailAsync("You cannot leave feedback for waiting booking");
                }

                var isInProgressBooking = _bookingRepository.Entities.Where(x => x.CustomerId == customerId &&
                    x.Id == isExistInBookingDetail.BookingId &&
                    x.Status == _enumService.GetEnumIdByValue(StaticVariable.INPROGRESSING, StaticVariable.BOOKING_STATUS_ENUM));

                if (isInProgressBooking.Any())
                {
                    return await Result<AddFeedbackCommand>.FailAsync("You cannot leave feedback for in progress booking");
                }

                if (!isCustomerHasBooking.Any())
                    return await Result<AddFeedbackCommand>.FailAsync("You cannot leave feedback of other customer");

                // Check Image is valid
                addFeedback.CustomerId = customerId;
                addFeedback.ServiceId = isExistInBookingDetail.ServiceId;
                await _feedbackRepository.AddAsync(addFeedback);
                await _unitOfWork.Commit(cancellationToken);
                if (request.FeedbackImageRequests != null)
                {
                    var addImage = _mapper.Map<List<FeedbackFileUpload>>(request.FeedbackImageRequests);
                    addImage.ForEach(x => x.FeedbackId = addFeedback.Id);
                    await _feedbackFileUploadRepository.AddRangeAsync(addImage);
                }
                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return await Result<AddFeedbackCommand>.SuccessAsync(request);
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