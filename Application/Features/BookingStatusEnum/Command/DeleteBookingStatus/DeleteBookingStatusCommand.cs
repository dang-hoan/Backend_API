using Application.Exceptions;
using Application.Interfaces.EnumMasterData;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.BookingStatusEnum.Command.DeleteBookingStatus
{
    public class DeleteBookingStatusCommand : IRequest<Result<long>>
    {
        public int Id { get; set; }
    }

    internal class DeleteBookingStatusCommandHandler : IRequestHandler<DeleteBookingStatusCommand, Result<long>>
    {
        private readonly IEnumMasterDataRepository _enumMasterDataRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public DeleteBookingStatusCommandHandler(IEnumMasterDataRepository enumMasterDataRepository, IUnitOfWork<long> unitOfWork)
        {
            _enumMasterDataRepository = enumMasterDataRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(DeleteBookingStatusCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var bookingStatus = await _enumMasterDataRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
                if (bookingStatus == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);

                await _enumMasterDataRepository.DeleteAsync(bookingStatus);
                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return await Result<long>.SuccessAsync(request.Id, $"Delete booking status by id {request.Id} successfully!");
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