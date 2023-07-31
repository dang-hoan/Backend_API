using Application.Dtos.Responses.FeedbackFileUpload;
using AutoMapper;

namespace Application.Mappings.FeedbackFileUpload
{
    public class FeedbackFileUploadMapping : Profile
    {
        public FeedbackFileUploadMapping() {
            CreateMap<Domain.Entities.FeedbackFileUpload.FeedbackFileUpload,FeedbackFileUploadResponse>().ReverseMap();
        }
    }
}
