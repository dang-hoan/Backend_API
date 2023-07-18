﻿using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Application.Interfaces.Service;
using Application.Interfaces.BookingDetail;
using Domain.Constants;
using Domain.Constants.Enum;

namespace Application.Features.Booking.Command.AddBooking
{
    public class AddBookingCommand : IRequest<Result<AddBookingCommand>>
    {
        public long? Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime Totime { get; set; }
        public string? Note { get; set; }
        public List<long> ServiceId { get; set; }
    }
    internal class AddBookingCommandHandler : IRequestHandler<AddBookingCommand, Result<AddBookingCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IBookingDetailRepository _bookingDetailService;


        public AddBookingCommandHandler(IMapper mapper, IBookingRepository bookingRepository,ICustomerRepository customerRepository, IServiceRepository serviceRepository, IUnitOfWork<long> unitOfWork, IBookingDetailRepository bookingDetailService)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _serviceRepository = serviceRepository;
            _bookingDetailService = bookingDetailService;
    }

        public async Task<Result<AddBookingCommand>> Handle(AddBookingCommand request, CancellationToken cancellationToken)
        {
            var isExistCustomer = await _customerRepository.FindAsync(_ => _.Id == request.CustomerId && _.IsDeleted == false) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_CUSTOMER);
            if(request.Totime.CompareTo(request.FromTime) < 0)
            {
                return await Result<AddBookingCommand>.FailAsync("ToTime must be greater than FromTime");
            }
            foreach (long i in request.ServiceId)
            {
                bool isExistedService = await IsExistedIdService(i);
                if (!isExistedService)
                {
                    throw new KeyNotFoundException(StaticVariable.NOT_FOUND_SERVICE);
                }
            }
            var booking = _mapper.Map<Domain.Entities.Booking.Booking>(request);
            booking.Status = BookingStatus.Waiting;
            await _bookingRepository.AddAsync(booking);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = booking.Id;
            foreach(long i in request.ServiceId)
            {
                await _bookingDetailService.AddAsync(new Domain.Entities.BookingDetail.BookingDetail
                {
                    BookingId = booking.Id,
                    ServiceId = i,
                    Note = booking.Note
                });
            }
            await _unitOfWork.Commit(cancellationToken);
            return await Result<AddBookingCommand>.SuccessAsync(request);
        }
        public async Task<bool> IsExistedIdService(long id)
        {
            var isExistedId = await _serviceRepository.FindAsync(_ => _.Id == id && _.IsDeleted == false);
            return isExistedId != null;
        }
    }
}