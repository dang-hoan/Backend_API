using Application.Features.Feedback.Command.AddFeedback;
using AutoMapper;

namespace Application.Mappings.Feeback
{
    public class FeedbackMappings : Profile
    {
        public FeedbackMappings() {
            CreateMap<AddFeedbackCommand, Domain.Entities.Feedback.Feedback>().ReverseMap();
        }
    }
}
