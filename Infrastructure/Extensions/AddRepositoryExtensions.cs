using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Employee;
using Application.Interfaces.Feedback;
using Application.Interfaces.Reply;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using Application.Interfaces.ViewCustomerBookingHistory;
using Infrastructure.Repositories.Booking;
using Infrastructure.Repositories.BookingDetail;
using Infrastructure.Repositories.Customer;
using Infrastructure.Repositories.Employee;
using Infrastructure.Repositories.Feedback;
using Infrastructure.Repositories.Reply;
using Infrastructure.Repositories.Service;
using Infrastructure.Repositories.ServiceImage;
using Infrastructure.Repositories.ViewCustomerBookingHistory;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class AddRepositoryExtensions
    {
        public static void AddEmployeeRepository(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }
        public static void AddServiceRepository(this IServiceCollection services)
        {
            services.AddScoped<IServiceRepository, ServiceRepository>();
        }
        public static void AddServiceImageRepository(this IServiceCollection services)
        {
            services.AddScoped<IServiceImageRepository, ServiceImageRepository>();
        }
        public static void AddCustomerRepository(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }
        public static void AddBookingRepository(this IServiceCollection services)
        {
            services.AddScoped<IBookingRepository,BookingRepository>();
        }
        public static void AddBookingDetailRepository(this IServiceCollection services)
        {
            services.AddScoped<IBookingDetailRepository, BookingDetailRepository>();
        }
        public static void AddViewCustomerBookingHistoryRepository(this IServiceCollection services)
        {
            services.AddScoped<IViewCustomerBookingHistoryRepository,ViewCustomerBookingHistoryRepository>();
        }
        public static void AddFeedbackRepository(this IServiceCollection services)
        {
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        }
        public static void AddReplyRepository(this IServiceCollection services)
        {
            services.AddScoped<IReplyRepository,ReplyRepository>();
        }
    }
}
