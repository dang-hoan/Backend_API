using Application.Parameters;
using Domain.Constants.Enum;
using Domain.Entities.Feedback;

namespace Application.Features.Service.Queries.GetAll
{
    public class GetAllServiceParameter: RequestParameter
    {
        public int? Time { get; set; }
        public int? Review { get; set; }
    }
}