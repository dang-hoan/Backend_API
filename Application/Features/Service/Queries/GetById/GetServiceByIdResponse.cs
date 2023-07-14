using Application.Dtos.Responses.ServiceImage;
using Domain.Entities.ServiceImage;

namespace Application.Features.Service.Queries.GetById
{
    public class GetServiceByIdResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int ServiceTime { get; set; }
        public List<ServiceImageResponse> Images { get; set; }
    }
}
