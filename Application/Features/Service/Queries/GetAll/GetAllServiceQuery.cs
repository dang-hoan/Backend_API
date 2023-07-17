using System.Linq;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Service;
using Application.Interfaces.Feedback;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Service.Queries.GetAll
{
    public class GetAllServiceQuery : GetAllServiceParameter, IRequest<PaginatedResult<GetAllServiceResponse>>
    {
    }
    internal class GetAllServiceHandler : IRequestHandler<GetAllServiceQuery, PaginatedResult<GetAllServiceResponse>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        public GetAllServiceHandler(
            IServiceRepository serviceRepository,
            IFeedbackRepository feedbackRepository
        )
        {
            _serviceRepository = serviceRepository;
            _feedbackRepository = feedbackRepository;
        }
        public async Task<PaginatedResult<GetAllServiceResponse>> Handle(GetAllServiceQuery request, CancellationToken cancellationToken)
        {
            var avgRating = from s in _serviceRepository.Entities
                    join f in _feedbackRepository.Entities on s.Id equals f.ServiceId into g
                    from fb in g.DefaultIfEmpty()
                    group fb by s.Id
                        into grp
                    select new
                    {
                        ServiceId = grp.Key,
                        AvgRating = grp.Average(query => (decimal)query.Rating == null ? 0 : (decimal)query.Rating)
                    };

            var query = from s in _serviceRepository.Entities
                        join r in avgRating on s.Id equals r.ServiceId
                        where !s.IsDeleted 
                        && (string.IsNullOrEmpty(request.Keyword) || s.Name.Contains(request.Keyword) || s.Id.ToString().Contains(request.Keyword))
                        && (!request.Time.HasValue || s.ServiceTime == request.Time.Value)
                        && (request.Review == null || (int) Math.Round(r.AvgRating) == request.Review.Value)
                        select new GetAllServiceResponse
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Time = s.ServiceTime,
                            Price = s.Price,
                            Description = s.Description,
                            CreatedOn = s.CreatedOn,
                            Review = (int) Math.Round(r.AvgRating)
                        };

            var data = query.OrderBy(request.OrderBy);
            var totalRecord = query.Count();
            List<GetAllServiceResponse> result;

            //Pagination
            if (!request.IsExport)
                result = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken: cancellationToken);
            else
                result = await data.ToListAsync(cancellationToken);
            return PaginatedResult<GetAllServiceResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

    }
}
