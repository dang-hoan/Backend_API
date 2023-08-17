using Application.Interfaces.EnumMasterData;
using Application.Parameters;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Application.Features.BookingStatusEnum.Queries.GetAll
{
    public class GetAllBookingStatusQuery : RequestParameter, IRequest<PaginatedResult<GetAllBookingStatusResponse>>
    {
    }

    internal class GetAllBookingStatusHandler : IRequestHandler<GetAllBookingStatusQuery, PaginatedResult<GetAllBookingStatusResponse>>
    {
        private readonly IEnumMasterDataRepository _enumMasterDataRepository;

        public GetAllBookingStatusHandler(IEnumMasterDataRepository enumMasterDataRepository)
        {
            _enumMasterDataRepository = enumMasterDataRepository;
        }

        public async Task<PaginatedResult<GetAllBookingStatusResponse>> Handle(GetAllBookingStatusQuery request, CancellationToken cancellationToken)
        {
            var query = _enumMasterDataRepository.Entities
                        .Where(x => !x.IsDeleted)
                        .Select(x => new GetAllBookingStatusResponse
                        {
                            Id = x.Id,
                            Value = x.Value,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn
                        });

            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            //var totalRecord = query.Count();
            //var data = query;
            List<GetAllBookingStatusResponse> result;

            //Pagination
            if (!request.IsExport)
                result = await data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            else
                result = await data.ToListAsync();
            return PaginatedResult<GetAllBookingStatusResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}