using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using Microsoft.VisualBasic;

namespace Application.Features.Service.Command.DeleteService
{
    public class DeleteServiceCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }
    }

    internal class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;

        public DeleteServiceCommandHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork<long> unitOfWork,
            IBookingRepository bookingRepository,
            IBookingDetailRepository bookingDetailRepository)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
        }

        public async Task<Result<long>> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {

            var query = from bd in _bookingDetailRepository.Entities
                        join s in _serviceRepository.Entities on bd.ServiceId equals s.Id
                        join b in _bookingRepository.Entities on bd.BookingId equals b.Id
                        where !s.IsDeleted && !b.IsDeleted && s.Id == request.Id
                        select new
                        {
                            Status = b.Status,
                            ServiceIsDeleted = s.IsDeleted,
                            BookingIsDeleted = b.IsDeleted,
                            BoookingDetailsDeleted = bd.IsDeleted
                        };
            var service = await _serviceRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (service == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            try
            {
                bool canDelete = true;
                foreach (var q in query)
                {
                    if (q.Status == BookingStatus.Inprogress)
                    {
                        canDelete = false;
                        break;
                    }
                }

                if (canDelete)
                {
                    var bookingDetail = await _bookingDetailRepository.GetByCondition(x => x.ServiceId == request.Id && !x.IsDeleted);
                    if (bookingDetail == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);

                    await _serviceRepository.DeleteAsync(service);

                    await _bookingDetailRepository.DeleteRange(bookingDetail.ToList());

                    await _unitOfWork.Commit(cancellationToken);

                    return await Result<long>.SuccessAsync(request.Id, $"Delete service and booking detail by serivce id {request.Id}  successfully!");
                }

                return await Result<long>.FailAsync("Service is using in other booking");
            }
            catch (System.Exception e)
            {
                return await Result<long>.FailAsync(e.Message);
            }
        }
    }
}