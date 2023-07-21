using Application.Interfaces.WorkShift;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.WorkShift.Queries.GetById
{
    public class GetWorkshiftByIdQuery : IRequest<Result<GetWorkshiftByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetWorkshiftByIdQueryHandler : IRequestHandler<GetWorkshiftByIdQuery, Result<GetWorkshiftByIdResponse>>
    {
        private readonly IWorkShiftRepository _workshiftRepository;
        private readonly IMapper _mapper;

        public GetWorkshiftByIdQueryHandler(IWorkShiftRepository workshiftRepository, IMapper mapper)
        {
            _workshiftRepository = workshiftRepository;
            _mapper = mapper;
        }

        public async Task<Result<GetWorkshiftByIdResponse>> Handle(GetWorkshiftByIdQuery request, CancellationToken cancellationToken)
        {
            var service = _workshiftRepository.Entities
                .Where(_ => _.Id == request.Id && _.IsDeleted == false).Select(s => new GetWorkshiftByIdResponse()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    FromTime = s.WorkingFromTime.ToString(@"hh\:mm"),
                    ToTime = s.WorkingToTime.ToString(@"hh\:mm"),
                    TimeWord = (s.WorkingToTime - s.WorkingFromTime).TotalHours,
                    IsDefault = s.IsDefault,
                    WorkDays = ConvertStringToList(s.WorkDays)
                }).FirstOrDefault();
            if (service == null) throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);
            return await Result<GetWorkshiftByIdResponse>.SuccessAsync(service);
        }
        public static List<int>? ConvertStringToList(string value)
        {
            try
            {
                if (value == null || value == "") return null;
                return new List<int>(Array.ConvertAll(value.Split(','), int.Parse));
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
