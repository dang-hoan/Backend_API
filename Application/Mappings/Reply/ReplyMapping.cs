using Application.Features.Reply.Command.AddReplyAtFeeback;
using Application.Features.Reply.Command.EditReply;
using AutoMapper;

namespace Application.Mappings.Reply
{
    public class ReplyMapping : Profile
    {
       public ReplyMapping() {
            CreateMap<AddReplyAtFeedbackCommand, Domain.Entities.Reply.Reply>().ReverseMap();
            CreateMap<EditReplyCommand, Domain.Entities.Reply.Reply>()
                .ForMember(dest => dest.FeedbackId, otp => otp.Ignore())
                .ReverseMap();
        }
    }
}
