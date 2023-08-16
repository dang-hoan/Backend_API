using Application.Interfaces.EnumMasterData;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BookingStatusEnum.Queries.GetById
{
    public class GetBookingStatusByIdQuery : IRequest<Result<GetBookingStatusByIdResponse>>
    {
        public int Id { get; set; }
    }
    internal class GetBookingStatusByIdQueryHandler : IRequestHandler<GetBookingStatusByIdQuery, Result<GetBookingStatusByIdResponse>>
    {
        private readonly IEnumMasterDataRepository _enumMasterDataRepository;

        public GetBookingStatusByIdQueryHandler(IEnumMasterDataRepository enumMasterDataRepository)
        {
            _enumMasterDataRepository = enumMasterDataRepository;
        }
        public async Task<Result<GetBookingStatusByIdResponse>> Handle(GetBookingStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var bookingStatus = await _enumMasterDataRepository.Entities
                .Where(_ => _.Id == request.Id && !_.IsDeleted)
                .Select(s => new GetBookingStatusByIdResponse
                {
                    Id = s.Id,
                    Value = s.Value
                }).FirstOrDefaultAsync();
            if(bookingStatus == null)
            {
                return await Result<GetBookingStatusByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            return await Result<GetBookingStatusByIdResponse>.SuccessAsync(bookingStatus);
        }
    }
}
