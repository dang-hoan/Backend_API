using Application.Parameters;
using Domain.Constants.Enum;

namespace Application.Features.Feedback.Queries.GetAll
{
    public class GetAllFeedbackParameter : RequestParameter
    {
        public string? ServiceName { get; set; }
        public int? Rating { get; set; }
    }
}