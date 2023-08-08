using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Booking.Queries.GetAll
{
    public class GetAllBookingQuery : GetAllBookingParameter, IRequest<PaginatedResult<GetAllBookingResponse>>
    {
    }

    internal class GetAllBookingHandler : IRequestHandler<GetAllBookingQuery, PaginatedResult<GetAllBookingResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;

        public GetAllBookingHandler(IBookingRepository bookingRepository, ICustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
        }

        public async Task<PaginatedResult<GetAllBookingResponse>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = from booking in _bookingRepository.Entities.AsEnumerable()
                        join customer in _customerRepository.Entities.AsEnumerable() on booking.CustomerId equals customer.Id
                        where !booking.IsDeleted
                                && (string.IsNullOrEmpty(request.Keyword)
                                || StringHelper.Contains(customer.CustomerName, request.Keyword)
                                || booking.Id.ToString().Contains(request.Keyword)
                                || customer.PhoneNumber.Contains(request.Keyword))
                                && (!request.BookingDate.HasValue || booking.BookingDate.Equals(request.BookingDate))
                                && (!request.FromTime.HasValue || booking.FromTime >= request.FromTime)
                                && (!request.ToTime.HasValue || booking.ToTime <= request.ToTime)
                                && (!request.Status.HasValue || booking.Status.Equals(request.Status))
                        select new GetAllBookingResponse
                        {
                            Id = booking.Id,
                            CustomerName = customer.CustomerName,
                            PhoneNumber = customer.PhoneNumber,
                            BookingDate = booking.BookingDate,
                            FromTime = booking.FromTime,
                            ToTime = booking.ToTime,
                            Status = booking.Status,
                            CreatedOn = booking.CreatedOn,
                            LastModifiedOn = booking.LastModifiedOn,
                        };

            var data = query.AsQueryable().OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllBookingResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllBookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}