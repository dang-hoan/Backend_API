using Application.Interfaces.Service;
using Application.Interfaces.Feedback;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;
using Domain.Helpers;
using Application.Interfaces.ServiceImage;
using Application.Interfaces;

namespace Application.Features.Service.Queries.GetAll
{
    public class GetAllServiceQuery : GetAllServiceParameter, IRequest<PaginatedResult<GetAllServiceResponse>>
    {
    }
    internal class GetAllServiceHandler : IRequestHandler<GetAllServiceQuery, PaginatedResult<GetAllServiceResponse>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly IUploadService _uploadService;
        public GetAllServiceHandler(
            IServiceRepository serviceRepository,
            IFeedbackRepository feedbackRepository,
            IServiceImageRepository serviceImageRepository,
            IUploadService uploadService
        )
        {
            _serviceRepository = serviceRepository;
            _feedbackRepository = feedbackRepository;
            _serviceImageRepository = serviceImageRepository;
            _uploadService = uploadService;
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
                                AvgRating = grp.Average(query => (decimal?)query.Rating == null ? 0 : (decimal)query.Rating)
                            };

            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = from s in _serviceRepository.Entities.AsEnumerable()
                        join r in avgRating.AsEnumerable() on s.Id equals r.ServiceId
                        join si in _serviceImageRepository.Entities.AsEnumerable() on s.Id equals si.ServiceId
                        where !s.IsDeleted
                        && (string.IsNullOrEmpty(request.Keyword) || StringHelper.Contains(s.Name, request.Keyword) || s.Id.ToString().Contains(request.Keyword))
                        && (!request.Time.HasValue || s.ServiceTime == request.Time.Value)
                        && (request.Review == null || (int) Math.Round(r.AvgRating) == request.Review.Value)
                        group new {s,si,r} by new {s.Id, si.ServiceId} into grp
                        select new GetAllServiceResponse
                        {
                            Id = grp.First().s.Id,
                            Name = grp.First().s.Name,
                            Time = grp.First().s.ServiceTime,
                            Price = grp.First().s.Price,
                            Description = grp.First().s.Description,
                            CreatedOn = grp.First().s.CreatedOn,
                            Review = (int) Math.Round(grp.First().r.AvgRating),
                            Image = _uploadService.GetFullUrl(grp.First().si.NameFile)
        };
            var data = query.AsQueryable().OrderBy(request.OrderBy);
            var totalRecord = query.Count();
            List<GetAllServiceResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllServiceResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

    }
}
