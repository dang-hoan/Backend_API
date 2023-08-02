using Application.Dtos.Requests.Feedback;
using Application.Dtos.Responses.FeedbackFileUpload;
using AutoMapper;
using Domain.Entities.FeedbackFileUpload;

namespace Application.Mappings.FeedbackFileUploads
{
    public class FeedbackFileUploadMapping : Profile
    {
        public FeedbackFileUploadMapping()
        {
            CreateMap<Domain.Entities.FeedbackFileUpload.FeedbackFileUpload, FeedbackFileUploadResponse>().ReverseMap();
            CreateMap<ImageRequest, FeedbackFileUpload>().ReverseMap();
        }
    }
}