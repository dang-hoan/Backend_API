using Application.Features.Reply.Command.AddReplyAtFeeback;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.Feeback
{
    public class FeedbackMappings : Profile
    {
        public FeedbackMappings() {
            CreateMap<AddReplyAtFeedbackCommand, Domain.Entities.Reply.Reply>().ReverseMap();
        }
    }
}
