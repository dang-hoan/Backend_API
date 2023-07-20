using Application.Features.WorkShift.Command.AddWorkShift;
using AutoMapper;

namespace Application.Mappings.Service
{
    public class WorkShiftMappings : Profile
    {
        public WorkShiftMappings()
        {
            CreateMap<AddWorkShiftCommand, Domain.Entities.WorkShift.WorkShift>().ReverseMap();
        }
    }
}
