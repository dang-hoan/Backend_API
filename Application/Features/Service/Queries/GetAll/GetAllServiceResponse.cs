using Domain.Constants.Enum;
using Domain.Entities.Feedback;

namespace Application.Features.Service.Queries.GetAll
{
    public class GetAllServiceResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Time { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int Review { get; set;}
    }
}