using Application.Features.Customer.Queries.GetById;
using Application.Features.Feedback.Queries.GetById;
using AutoMapper;

namespace Application.Mappings.ViewCustomerFeedbackReply
{
    public class ViewCustomerFeedbackReplyMapping : Profile
    {
        public ViewCustomerFeedbackReplyMapping() {
            CreateMap<Domain.Entities.View.ViewCustomerFeedbackReply.ViewCustomerFeedbackReply, GetFeedbackByIdResponse>().ReverseMap();
        }
    }
}
