using Application.Features.Booking.Command.AddBooking;
using AutoMapper;

namespace Application.Mappings.Booking
{
    internal class BookingMappings: Profile
    {
        public BookingMappings()
        {
            CreateMap<AddBookingCommand, Domain.Entities.Booking.Booking>().ReverseMap();
        }
    }
}
