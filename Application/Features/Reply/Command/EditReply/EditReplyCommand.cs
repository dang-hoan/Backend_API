using Application.Features.Booking.Command.AddBooking;
using Application.Interfaces;
using Application.Interfaces.Reply;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Reply.Command.EditReply
{
    public class EditReplyCommand : IRequest<Result<EditReplyCommand>>
    {
        public long? Id { get; set; }
        public long? FeedbackId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
    }
    internal class EditReplyCommandHandler : IRequestHandler<EditReplyCommand, Result<EditReplyCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IReplyRepository _replyRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public EditReplyCommandHandler(IMapper mapper, IReplyRepository replyRepository, IUnitOfWork<long> unitOfWork, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _replyRepository = replyRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<Result<EditReplyCommand>> Handle(EditReplyCommand request, CancellationToken cancellationToken)
        {
            var editReply = await _replyRepository.FindAsync(_ => _.Id ==  request.Id && !_.IsDeleted);
            if (editReply == null) return await Result<EditReplyCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            if (_currentUserService.RoleName.Equals(RoleConstants.EmployeeRole))
            {
                if (!_currentUserService.UserName.Equals(editReply.CreatedBy))
                    return await Result<EditReplyCommand>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
            }

            editReply.Title = request.Title;
            editReply.Content = request.Content;
            await _replyRepository.UpdateAsync(editReply);
            await _unitOfWork.Commit(cancellationToken);
            var replyResponse = _mapper.Map<EditReplyCommand>(editReply);
            return await Result<EditReplyCommand>.SuccessAsync(replyResponse);
        }
    }
}
