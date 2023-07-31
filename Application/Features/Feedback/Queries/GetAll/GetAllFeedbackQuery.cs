using Application.Interfaces.Feedback;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;
using Application.Interfaces.Customer;
using Application.Interfaces.Service;
using Domain.Helpers;

namespace Application.Features.Feedback.Queries.GetAll
{
    public class GetAllFeedbackQuery : GetAllFeedbackParameter, IRequest<PaginatedResult<GetAllFeedbackResponse>>
    {
    }

    internal class GetAllFeedbackHandler : IRequestHandler<GetAllFeedbackQuery, PaginatedResult<GetAllFeedbackResponse>>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServiceRepository _serviceRepository;
        public GetAllFeedbackHandler
        (
            IFeedbackRepository feedbackRepository,
            ICustomerRepository customerRepository,
            IServiceRepository serviceRepository
        )
        {
            _feedbackRepository = feedbackRepository;
            _customerRepository = customerRepository;
            _serviceRepository = serviceRepository;
        }
        public async Task<PaginatedResult<GetAllFeedbackResponse>> Handle(GetAllFeedbackQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            if (request.ServiceName != null)
                request.ServiceName = request.ServiceName.Trim();

            var query = from fb in _feedbackRepository.Entities.AsEnumerable()
                        join c in _customerRepository.Entities.AsEnumerable() on fb.CustomerId equals c.Id
                        join s in _serviceRepository.Entities.AsEnumerable() on fb.ServiceId equals s.Id
                        where !c.IsDeleted && !fb.IsDeleted && !s.IsDeleted
                        && (string.IsNullOrEmpty(request.Keyword)
                            || StringHelper.Contains(s.Name, request.Keyword)
                            || c.PhoneNumber.Contains(request.Keyword)
                            || c.Id.ToString().Contains(request.Keyword)
                            || StringHelper.Contains(c.CustomerName, request.Keyword)
                           )
                        && (string.IsNullOrEmpty(request.ServiceName) || StringHelper.Contains(s.Name, request.ServiceName))
                        && (request.Rating == null || (int?)fb.Rating == request.Rating)
                        select new GetAllFeedbackResponse
                        {
                            Id = fb.Id,
                            CustomerName = c.CustomerName,
                            Title = fb.Title,
                            Phone = c.PhoneNumber,
                            Rating = fb.Rating,
                            ServiceName = s.Name,
                            CreatedOn = fb.CreatedOn
                        };

            var data = query.AsQueryable().OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllFeedbackResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllFeedbackResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}