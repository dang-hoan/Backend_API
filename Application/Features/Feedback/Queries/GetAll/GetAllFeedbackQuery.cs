using Application.Interfaces.Feedback;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Customer;
using Application.Interfaces.Service;

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
            var query = from fb in _feedbackRepository.Entities
                        join c in _customerRepository.Entities on fb.CustomerId equals c.Id
                        join s in _serviceRepository.Entities on fb.ServiceId equals s.Id
                        where !c.IsDeleted && !fb.IsDeleted && !s.IsDeleted
                        && (string.IsNullOrEmpty(request.Keyword)
                            || s.Name.Contains(request.Keyword)
                            || c.PhoneNumber.Contains(request.Keyword)
                            || c.Id.ToString().Contains(request.Keyword)
                            || c.CustomerName.Contains(request.Keyword)
                           )
                        && (string.IsNullOrEmpty(request.ServiceName) || s.Name == request.ServiceName)
                        && (request.Rating == null || (int)fb.Rating == request.Rating)
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

            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllFeedbackResponse> result;

            //Pagination
            if (!request.IsExport)
                result = await data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            else
                result = await data.ToListAsync(cancellationToken);
            return PaginatedResult<GetAllFeedbackResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}