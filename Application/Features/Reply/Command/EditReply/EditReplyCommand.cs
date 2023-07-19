using Application.Interfaces.Reply;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public EditReplyCommandHandler(IMapper mapper, IReplyRepository replyRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _replyRepository = replyRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<EditReplyCommand>> Handle(EditReplyCommand request, CancellationToken cancellationToken)
        {
            var editReply = await _replyRepository.FindAsync(_ => _.Id ==  request.Id && !_.IsDeleted) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);
            editReply.Title = request.Title;
            editReply.Content = request.Content;
            await _replyRepository.UpdateAsync(editReply);
            await _unitOfWork.Commit(cancellationToken);
            var replyResponse = _mapper.Map<EditReplyCommand>(editReply);
            return await Result<EditReplyCommand>.SuccessAsync(replyResponse);
        }
    }
}
