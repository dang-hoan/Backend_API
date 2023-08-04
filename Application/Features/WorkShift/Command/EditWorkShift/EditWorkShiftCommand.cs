using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.WorkShift;
using Domain.Constants;
using System.Text.Json.Serialization;
using Application.Interfaces;

namespace Application.Features.WorkShift.Command.EditWorkShift
{
    public class EditWorkShiftCommand : IRequest<Result<EditWorkShiftCommand>>
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string? FromTime { get; set; }

        public string? ToTime { get; set; }

        [JsonIgnore]
        public TimeSpan WorkingFromTime { get; set; }

        [JsonIgnore]
        public TimeSpan WorkingToTime { get; set; }

        public bool? IsDefault { get; set; }

        public string? Description { get; set; }

        public List<int> WorkDays { get; set; }
    }

    internal class EditWorkShiftCommandHandler : IRequestHandler<EditWorkShiftCommand, Result<EditWorkShiftCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTimeService;
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IUnitOfWork<long> _unitOfWork;


        public EditWorkShiftCommandHandler(IMapper mapper, IDateTimeService dateTimeService, IWorkShiftRepository workShiftRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _dateTimeService = dateTimeService;
            _workShiftRepository = workShiftRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<EditWorkShiftCommand>> Handle(EditWorkShiftCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditWorkShiftCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            var editWorkShift = await _workShiftRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (editWorkShift == null) return await Result<EditWorkShiftCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            if (request.IsDefault != null && request.IsDefault == true)
            {
                request.WorkingFromTime = new TimeSpan(hours: 8, minutes: 0, seconds: 0);
                request.WorkingToTime = new TimeSpan(hours: 17, minutes: 0, seconds: 0);
            }
            else
            {
                if (request.FromTime == null || request.ToTime == null)
                    return await Result<EditWorkShiftCommand>.FailAsync(StaticVariable.NOT_LOGIC_WORKING_TIME);

                //check logic time
                DateTime fromTime;
                DateTime toTime;
                if (!_dateTimeService.IsCorrectFormat(request.FromTime, "HH:mm", out fromTime) || !_dateTimeService.IsCorrectFormat(request.ToTime, "HH:mm", out toTime))
                {
                    return await Result<EditWorkShiftCommand>.FailAsync(StaticVariable.NOT_LOGIC_WORKING_TIME_FORMAT);
                }
                request.WorkingFromTime = new TimeSpan(hours: fromTime.Hour, minutes: fromTime.Minute, seconds: 0);
                request.WorkingToTime = new TimeSpan(hours: toTime.Hour, minutes: toTime.Minute, seconds: 0);
            }

            if (request.WorkingToTime < request.WorkingFromTime)
            {
                return await Result<EditWorkShiftCommand>.FailAsync(StaticVariable.NOT_LOGIC_DATE_ORDER);
            }
            
            var workShift = _mapper.Map(request, editWorkShift);

            request.WorkDays = request.WorkDays.Distinct().ToList();
            foreach (int i in request.WorkDays)
            {
                if (i < 2 || i > 8)
                {
                    return await Result<EditWorkShiftCommand>.FailAsync(StaticVariable.NOT_LOGIC_WORKDAY_VALUE);
                }
            }
            string workDays = string.Join(",", request.WorkDays);
            workShift.WorkDays = workDays;

            await _workShiftRepository.UpdateAsync(workShift);
            await _unitOfWork.Commit(cancellationToken);

            request.FromTime = request.WorkingFromTime.ToString(@"hh\:mm");
            request.ToTime = request.WorkingToTime.ToString(@"hh\:mm");

            return await Result<EditWorkShiftCommand>.SuccessAsync(request);
        }
    }
}
