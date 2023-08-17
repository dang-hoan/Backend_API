using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Feedback;
using Application.Interfaces.Service;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.Statistics.Queries.GetInsightMetrics
{
    public class GetInsightMetricsQuery : IRequest<Result<GetInsightMetricsResponse>>
    {
        public StatisticsTime statisticsTime { get; set; }
    }
    internal class GetInsightMetricsQueryHandler : IRequestHandler<GetInsightMetricsQuery, Result<GetInsightMetricsResponse>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IEnumService _enumService;
        public GetInsightMetricsQueryHandler(ICustomerRepository customerRepository, IBookingRepository bookingRepository,
            IBookingDetailRepository bookingDetailRepository, IServiceRepository serviceRepository, IFeedbackRepository feedbackRepository,
            IEnumService enumService)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _serviceRepository = serviceRepository;
            _feedbackRepository = feedbackRepository;
            _enumService = enumService;
        }
        public async Task<Result<GetInsightMetricsResponse>> Handle(GetInsightMetricsQuery request, CancellationToken cancellationToken)
        {
            GetInsightMetricsResponse response = new GetInsightMetricsResponse();
            response.Sales = 12.6m;

            Expression<Func<Domain.Entities.Customer.Customer, bool>> conditionCustomer;
            Expression<Func<Domain.Entities.Booking.Booking, bool>> conditionBooking;
            Expression<Func<Domain.Entities.Feedback.Feedback, bool>> conditionFeedback;
            if (request.statisticsTime == StatisticsTime.LastMonth)
            {
                DateTime currentDate = DateTime.Now;
                DateTime firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);
                conditionCustomer = _ => _.CreatedOn.Date >= firstDayOfLastMonth && _.CreatedOn.Date < firstDayOfCurrentMonth && _.IsDeleted == false;
                conditionBooking = _ => _.CreatedOn.Date >= firstDayOfLastMonth && _.CreatedOn.Date < firstDayOfCurrentMonth && _.IsDeleted == false && _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM);
                conditionFeedback = _ => _.CreatedOn.Date >= firstDayOfLastMonth && _.CreatedOn.Date < firstDayOfCurrentMonth && _.IsDeleted == false;
            }
            else if (request.statisticsTime == StatisticsTime.LastDayOrWeek)
            {
                DateTime currentDate = DateTime.Now;
                DateTime lastDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                conditionCustomer = _ => _.CreatedOn == lastDayOfCurrentMonth && _.IsDeleted == false;
                conditionBooking = _ => _.CreatedOn == lastDayOfCurrentMonth && _.IsDeleted == false && _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM);
                conditionFeedback = _ => _.CreatedOn == lastDayOfCurrentMonth && _.IsDeleted == false;
            }
            else
            {
                DateTime currentDate = DateTime.Now;
                int lastYear = currentDate.Year - 1;
                DateTime firstDayOfLastYear = new DateTime(lastYear, 1, 1);
                DateTime lastDayOfLastYear = new DateTime(lastYear, 12, 31);
                conditionCustomer = _ => _.CreatedOn.Date >= firstDayOfLastYear && _.CreatedOn.Date < firstDayOfLastYear && _.IsDeleted == false;
                conditionBooking = _ => _.CreatedOn.Date >= firstDayOfLastYear && _.CreatedOn.Date < firstDayOfLastYear && _.IsDeleted == false && _.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM);
                conditionFeedback = _ => _.CreatedOn.Date >= firstDayOfLastYear && _.CreatedOn.Date < firstDayOfLastYear && _.IsDeleted == false;
            }
            var customer = await _customerRepository.GetByCondition(conditionCustomer);
            response.Subscription = customer.ToList().Count;

            var bookings = await _bookingRepository.GetByCondition(conditionBooking);
            var revenue = 0m;
            foreach (var booking in bookings)
            {
                var bookingDetails = await _bookingDetailRepository.GetByCondition(_ => _.BookingId == booking.Id && _.IsDeleted == false);
                foreach(var bookingDetail in bookingDetails)
                {
                    var service = await _serviceRepository.GetByIdAsync(bookingDetail.ServiceId);
                    if(service != null)
                    {
                        revenue += service.Price;
                    }
                }
            }
            response.Revenue = revenue;
            var feedback = await _feedbackRepository.GetByCondition(conditionFeedback);
            response.Feedback = feedback.ToList().Count;
            return await Result<GetInsightMetricsResponse>.SuccessAsync(response);
        }
    }
}
