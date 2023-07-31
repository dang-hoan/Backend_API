using Application.Features.Booking.Command.AddBooking;
using Application.Features.Booking.Command.EditBooking;
using Application.Features.Booking.Queries.GetById;
using AutoMapper;

namespace Application.Mappings.Booking
{
    public class BookingMappings : Profile
    {
        public BookingMappings() {
            CreateMap<Domain.Entities.Booking.Booking, GetBookingByIdResponse>().ReverseMap();
            CreateMap<AddBookingCommand, Domain.Entities.Booking.Booking>().ReverseMap();
            CreateMap<EditBookingCommand, Domain.Entities.Booking.Booking>()
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore()).ReverseMap();
        }
    }
}
