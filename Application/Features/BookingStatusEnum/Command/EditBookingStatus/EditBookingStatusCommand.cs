using Application.Exceptions;
using Application.Interfaces.EnumMasterData;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.BookingStatusEnum.Command.EditBookingStatus
{
    public class EditBookingStatusCommand : IRequest<Result<EditBookingStatusCommand>>
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    internal class EditBookingStatusCommandHandler : IRequestHandler<EditBookingStatusCommand, Result<EditBookingStatusCommand>>
    {
        private readonly IEnumMasterDataRepository _enumMasterDataRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public EditBookingStatusCommandHandler(IEnumMasterDataRepository enumMasterDataRepository, IUnitOfWork<long> unitOfWork)
        {
            _enumMasterDataRepository = enumMasterDataRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<EditBookingStatusCommand>> Handle(EditBookingStatusCommand request, CancellationToken cancellationToken)
        {
            var bookingStatus = await _enumMasterDataRepository.FindAsync(_ => _.Id == request.Id && _.IsDeleted == false);
            if (bookingStatus == null) return await Result<EditBookingStatusCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            //open transaction
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                bookingStatus.Value = request.Value;
                bookingStatus.EnumType = StaticVariable.BOOKING_STATUS_ENUM;

                await _enumMasterDataRepository.UpdateAsync(bookingStatus);
                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return await Result<EditBookingStatusCommand>.SuccessAsync(request);
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