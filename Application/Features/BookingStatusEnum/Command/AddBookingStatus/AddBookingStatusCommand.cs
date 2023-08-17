using Application.Exceptions;
using Application.Interfaces.EnumMasterData;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.BookingStatusEnum.Command.AddBookingStatus
{
    public class AddBookingStatusCommand : IRequest<Result<AddBookingStatusCommand>>
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    internal class AddBookingStatusCommandHandler : IRequestHandler<AddBookingStatusCommand, Result<AddBookingStatusCommand>>
    {
        private readonly IEnumMasterDataRepository _enumMasterDataRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public AddBookingStatusCommandHandler(IEnumMasterDataRepository enumMasterDataRepository, IUnitOfWork<long> unitOfWork)
        {
            _enumMasterDataRepository = enumMasterDataRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddBookingStatusCommand>> Handle(AddBookingStatusCommand request, CancellationToken cancellationToken)
        {
            //open transaction
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var bookingStatus = new Domain.Entities.EnumMasterData.EnumMasterData() { 
                    Value = request.Value,
                    EnumType = StaticVariable.BOOKING_STATUS_ENUM
                };

                await _enumMasterDataRepository.AddAsync(bookingStatus);
                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                request.Id = bookingStatus.Id;

                return await Result<AddBookingStatusCommand>.SuccessAsync(request);
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