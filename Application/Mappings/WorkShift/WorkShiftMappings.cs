using Application.Features.WorkShift.Command.AddWorkShift;
using Application.Features.WorkShift.Command.EditWorkShift;
using AutoMapper;

namespace Application.Mappings.Service
{
    public class WorkShiftMappings : Profile
    {
        public WorkShiftMappings()
        {
            CreateMap<AddWorkShiftCommand, Domain.Entities.WorkShift.WorkShift>().ReverseMap();
            CreateMap<EditWorkShiftCommand, Domain.Entities.WorkShift.WorkShift>().ReverseMap();
        }
    }
}
