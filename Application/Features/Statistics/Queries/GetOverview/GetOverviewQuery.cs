using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Feedback;
using Application.Interfaces.Service;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Features.Statistics.Queries.GetOverview
{
    public class GetOverviewQuery : IRequest<Result<List<GetOverviewResponse>>>
    {
        public StatisticsTime statisticsTime { get; set; }
    }
    internal class GetOverviewQueryHandler : IRequestHandler<GetOverviewQuery, Result<List<GetOverviewResponse>>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IServiceRepository _serviceRepository;

        public GetOverviewQueryHandler(ICustomerRepository customerRepository, IBookingRepository bookingRepository, IFeedbackRepository feedbackRepository, IBookingDetailRepository bookingDetailRepository, IServiceRepository serviceRepository)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _feedbackRepository = feedbackRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<Result<List<GetOverviewResponse>>> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
        {
            List<GetOverviewResponse> response = new List<GetOverviewResponse>();
            if (request.statisticsTime == StatisticsTime.LastDayOrWeek)
            {
                DateTime currentDate = DateTime.Now;
                DateTime firstDayOfLastWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek - 6);
                for (int i = 0; i < 7; i++)
                {
                    DateTime dayOfLastWeek = firstDayOfLastWeek.AddDays(i);
                    GetOverviewResponse overviewOfDay = new GetOverviewResponse();
                    overviewOfDay.Revenue = 0m;
                    var customers = await _customerRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date == dayOfLastWeek.Date);
                    var feedbacks = await _feedbackRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date == dayOfLastWeek.Date);
                    var bookings = await _bookingRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date == dayOfLastWeek.Date);
                    overviewOfDay.Reach = customers.ToList().Count + feedbacks.ToList().Count + bookings.ToList().Count;
                    foreach (var booking in bookings)
                    {
                        var bookingDetails = await _bookingDetailRepository.GetByCondition(_ => _.BookingId == booking.Id && _.IsDeleted == false);
                        foreach (var bookingDetail in bookingDetails)
                        {
                            var service = await _serviceRepository.GetByIdAsync(bookingDetail.ServiceId);
                            if (service != null)
                            {
                                overviewOfDay.Revenue += service.Price;
                            }
                        }
                    }
                    overviewOfDay.Date = dayOfLastWeek;
                    response.Add(overviewOfDay);
                }
            } else if (request.statisticsTime == StatisticsTime.LastMonth)
            {
                DateTime currentDate = DateTime.Now;
                DateTime firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);
                DateTime lastDayOfLastMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddDays(-1);
                DateTime dayOfLastMonth = firstDayOfLastMonth;
                while (dayOfLastMonth < lastDayOfLastMonth)
                {
                    GetOverviewResponse overviewOfDay = await getOverviewInPeriod(dayOfLastMonth,6);
                    response.Add(overviewOfDay);
                    dayOfLastMonth = dayOfLastMonth.AddDays(7);
                }
                int lastPeriod = lastDayOfLastMonth.Day - dayOfLastMonth.Day;
                GetOverviewResponse overviewOfLastDay = await getOverviewInPeriod(dayOfLastMonth,lastPeriod);
                response.Add(overviewOfLastDay);
            } else
            {
                DateTime currentDate = DateTime.Now;
                int lastYear = currentDate.Year - 1;
                DateTime firstDayOfLastYear = new DateTime(lastYear, 1, 1);
                for(int i = 0; i < 12; i++)
                {
                    DateTime dayOfYear = firstDayOfLastYear.AddMonths(i);
                    GetOverviewResponse overviewOfDay = new GetOverviewResponse();
                    overviewOfDay.Revenue = 0m;
                    var customers = await _customerRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date >= firstDayOfLastYear.AddMonths(i).Date && _.CreatedOn.Date <= firstDayOfLastYear.AddMonths(i + 1).AddDays(-1));
                    var feedbacks = await _feedbackRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date >= firstDayOfLastYear.AddMonths(i).Date && _.CreatedOn.Date <= firstDayOfLastYear.AddMonths(i + 1).AddDays(-1));
                    var bookings = await _bookingRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date >= firstDayOfLastYear.AddMonths(i).Date && _.CreatedOn.Date <= firstDayOfLastYear.AddMonths(i + 1).AddDays(-1));
                    overviewOfDay.Reach = customers.ToList().Count + feedbacks.ToList().Count + bookings.ToList().Count;
                    foreach (var booking in bookings)
                    {
                        var bookingDetails = await _bookingDetailRepository.GetByCondition(_ => _.BookingId == booking.Id && _.IsDeleted == false);
                        foreach (var bookingDetail in bookingDetails)
                        {
                            var service = await _serviceRepository.GetByIdAsync(bookingDetail.ServiceId);
                            if (service != null)
                            {
                                overviewOfDay.Revenue += service.Price;
                            }
                        }
                    }
                    overviewOfDay.Date = dayOfYear;
                    response.Add(overviewOfDay);
                }
            }
            return await Result<List<GetOverviewResponse>>.SuccessAsync(response);
        }
        private async Task<GetOverviewResponse> getOverviewInPeriod(DateTime firstDateOfWeek, int period)
        {
            GetOverviewResponse overviewOfDay = new GetOverviewResponse();
            overviewOfDay.Revenue = 0m;
            var customers = await _customerRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date >= firstDateOfWeek.Date && _.CreatedOn.Date <= firstDateOfWeek.AddDays(period).Date);
            var feedbacks = await _feedbackRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date >= firstDateOfWeek.Date && _.CreatedOn.Date <= firstDateOfWeek.AddDays(period).Date);
            var bookings = await _bookingRepository.GetByCondition(_ => _.IsDeleted == false && _.CreatedOn.Date >= firstDateOfWeek.Date && _.CreatedOn.Date <= firstDateOfWeek.AddDays(period).Date);
            overviewOfDay.Reach = customers.ToList().Count + feedbacks.ToList().Count + bookings.ToList().Count;
            foreach (var booking in bookings)
            {
                var bookingDetails = await _bookingDetailRepository.GetByCondition(_ => _.BookingId == booking.Id && _.IsDeleted == false);
                foreach (var bookingDetail in bookingDetails)
                {
                    var service = await _serviceRepository.GetByIdAsync(bookingDetail.ServiceId);
                    if (service != null)
                    {
                        overviewOfDay.Revenue += service.Price;
                    }
                }
            }
            overviewOfDay.Date = firstDateOfWeek;
            return overviewOfDay;
        }
    } 
}
