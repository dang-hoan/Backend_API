using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var query = from booking in _bookingRepository.Entities
                        join customer in _customerRepository.Entities on booking.CustomerId equals customer.Id
                        where !booking.IsDeleted 
                        && (string.IsNullOrEmpty(request.Keyword) 
                        || customer.CustomerName.Contains(request.Keyword) 
                        || booking.Id.ToString().Contains(request.Keyword)
                        || customer.PhoneNumber.Contains(request.Keyword))
                        && (!request.BookingDate.HasValue || booking.BookingDate.Equals(request.BookingDate))
                        && (!request.FromTime.HasValue || booking.FromTime >= request.FromTime)
                        && (!request.Totime.HasValue || booking.Totime <= request.Totime)
                        && (!request.Status.HasValue || booking.Status.Equals(request.Status))
                        select new GetAllBookingResponse
                        {
                            Id = booking.Id,                            
                            CustomerName = customer.CustomerName,
                            PhoneNumber = customer.PhoneNumber,
                            BookingDate = booking.BookingDate,
                            FromTime = booking.FromTime,
                            Totime = booking.Totime,
                            Status = booking.Status,
                            CreatedOn = booking.CreatedOn,
                            LastModifiedOn = booking.LastModifiedOn,
                        };


            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllBookingResponse> result;

            //Pagination
            if (!request.IsExport)
                result = await data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            else
                result = await data.ToListAsync(cancellationToken);
            return PaginatedResult<GetAllBookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}
