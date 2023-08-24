using Application.Features.Feedback.Command.AddFeedback;
using Application.Features.Feedback.Command.EditFeedback;
using AutoMapper;

namespace Application.Mappings.Feeback
{
    public class FeedbackMappings : Profile
    {
        public FeedbackMappings() {
            CreateMap<AddFeedbackCommand, Domain.Entities.Feedback.Feedback>().ReverseMap();
            CreateMap<EditFeedbackCommand, Domain.Entities.Feedback.Feedback>().ReverseMap();
        }
    }
}
