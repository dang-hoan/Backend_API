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

namespace Application.Features.Feedback.Command.EditFeedback
{
    public class EditFeedbackCommand : IRequest<Result<EditFeedbackCommand>>
    {
        public long Id { get; set; }
        public int Rating { get; set; } = default!;
        public long BookingDetailId { get; set; } = default!;
        public string? StaffContent { get; set; }
        public string? ServiceContent { get; set; }
        public List<ImageRequest>? FeedbackImageRequests { get; set; } // Hình ảnh mô tả (File)
    }

    internal class EditFeedbackCommandHandler : IRequestHandler<EditFeedbackCommand, Result<EditFeedbackCommand>>
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
        private readonly IUploadService _uploadService;

        public EditFeedbackCommandHandler
        (
            IMapper mapper, IFeedbackRepository feedbackRepository,
            IFeedbackFileUploadRepository feedbackFileUploadRepository,
            IBookingDetailRepository bookingDetailRepository,
            IBookingRepository bookingRepository,
            IEnumService enumService,
            IUnitOfWork<long> unitOfWork,
            ICurrentUserService currentUserService,
            UserManager<AppUser> userManager,
            IUploadService uploadService
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
            _uploadService = uploadService;
        }

        public async Task<Result<EditFeedbackCommand>> Handle(EditFeedbackCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var isExistFeedback = await _feedbackRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
                if(isExistFeedback == null) return await Result<EditFeedbackCommand>.FailAsync(StaticVariable.NOT_FOUND_FEEDBACK);
                var isExistInBookingDetail = await _bookingDetailRepository.FindAsync(x => x.Id == request.BookingDetailId && !x.IsDeleted);
                if(isExistInBookingDetail == null) return await Result<EditFeedbackCommand>.FailAsync(StaticVariable.NOT_FOUND_BOOKING_DETAIL);
                // var isExistBookingFeedback = await _feedbackRepository.FindAsync(x => x.BookingDetailId == request.BookingDetailId && !x.IsDeleted);
                if (request.Rating < (int)Rating.OneStar || request.Rating > (int)Rating.FiveStars)
                {
                    return await Result<EditFeedbackCommand>.FailAsync("Rating must be from 1 to 5 star");
                }
                // if (isExistBookingFeedback != null)
                // {
                //     return await Result<EditFeedbackCommand>.FailAsync("You rated this feedback");
                // }
                var editFeedback = await _feedbackRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
                _mapper.Map(request, editFeedback);

                long customerId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                var isCustomerHasBooking = _bookingRepository.Entities.Where(x => x.CustomerId == customerId && x.Id == isExistInBookingDetail.BookingId && x.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)).Select(x => x.Id);

                var isWaitingBooking = _bookingRepository.Entities.Where(x => x.CustomerId == customerId &&
                                        x.Id == isExistInBookingDetail.BookingId &&
                                        x.Status == _enumService.GetEnumIdByValue(StaticVariable.WAITING, StaticVariable.BOOKING_STATUS_ENUM));

                if (isWaitingBooking.Any())
                {
                    return await Result<EditFeedbackCommand>.FailAsync("You cannot leave feedback for waiting booking");
                }

                var isInProgressBooking = _bookingRepository.Entities.Where(x => x.CustomerId == customerId &&
                    x.Id == isExistInBookingDetail.BookingId &&
                    x.Status == _enumService.GetEnumIdByValue(StaticVariable.INPROGRESSING, StaticVariable.BOOKING_STATUS_ENUM));

                if (isInProgressBooking.Any())
                {
                    return await Result<EditFeedbackCommand>.FailAsync("You cannot leave feedback for in progress booking");
                }

                if (!isCustomerHasBooking.Any())
                    return await Result<EditFeedbackCommand>.FailAsync("You cannot leave feedback of other customer");

                // Check Image is valid
                editFeedback.CustomerId = customerId;
                editFeedback.ServiceId = isExistInBookingDetail.ServiceId;
                await _feedbackRepository.UpdateAsync(editFeedback);
                await _unitOfWork.Commit(cancellationToken);
                if (request.FeedbackImageRequests != null)
                {
                    List<string?> listNewFiles = request.FeedbackImageRequests.Select(x => x.NameFile).ToList();
                    //Remove and Add List Request Image
                    var requestImages =  _feedbackFileUploadRepository.Entities.Where(x => x.FeedbackId == editFeedback.Id && !x.IsDeleted).ToList();
                    if (requestImages.Any())
                    {
                        foreach (var item in requestImages)
                        {
                            if(!listNewFiles.Contains(item.NameFile))
                                await _uploadService.DeleteAsync(item.NameFile);
                        }
                        await _feedbackFileUploadRepository.RemoveRangeAsync(requestImages);
                        await _unitOfWork.Commit(cancellationToken);
                    }
                    var image = _mapper.Map<List<FeedbackFileUpload>>(request.FeedbackImageRequests);
                    var requestImage = image.Select(x =>
                    {
                        x.Id = 0;
                        x.FeedbackId = editFeedback.Id;
                        return x;
                    }).ToList();
                    await _feedbackFileUploadRepository.AddRangeAsync(requestImage);
                }

                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return await Result<EditFeedbackCommand>.SuccessAsync(request);
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