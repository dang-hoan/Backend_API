using Application.Parameters;

namespace Application.Features.Service.Queries.GetAll
{
    public class GetAllServiceParameter: RequestParameter
    {
        public int? Time { get; set; }
        public int? Review { get; set; }
    }
}