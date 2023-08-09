using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Feedback;
using Application.Interfaces.FeedbackFileUpload;
using Application.Interfaces.Reply;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Feedback.Command.DeleteFeedback
{
    public class DeleteFeedbackCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }
    }

    internal class DeleteFeedbackCommandHandler : IRequestHandler<DeleteFeedbackCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly IFeedbackFileUploadRepository _feedbackFileUploadRepository;
        private readonly IUploadService _uploadService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public DeleteFeedbackCommandHandler(IUnitOfWork<long> unitOfWork, IFeedbackRepository feedbackRepository, IReplyRepository replyRepository,IFeedbackFileUploadRepository feedbackFileUploadRepository,
            IUploadService uploadService, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _feedbackRepository = feedbackRepository;
            _replyRepository = replyRepository;
            _feedbackFileUploadRepository = feedbackFileUploadRepository;
            _uploadService = uploadService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<long>> Handle(DeleteFeedbackCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var deleteFeedback = await _feedbackRepository.FindAsync(_ => _.Id == request.Id && !_.IsDeleted) ?? throw new ApiException(StaticVariable.NOT_FOUND_MSG);
                
                if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
                {
                    long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                    if (userId != deleteFeedback.CustomerId)
                        return await Result<long>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
                }

                var deleteReply = await _replyRepository.FindAsync(_ => _.Id == deleteFeedback.ReplyId && !_.IsDeleted);
                if (deleteReply != null)
                {
                    await _replyRepository.DeleteAsync(deleteReply);
                }
                var feedbackFiles = await _feedbackFileUploadRepository.GetByCondition(_ => _.FeedbackId == request.Id);
                if (feedbackFiles != null)
                {
                    foreach(var file in feedbackFiles)
                    {
                        await _uploadService.DeleteAsync(file.NameFile);
                    }
                }
                await _feedbackRepository.DeleteAsync(deleteFeedback);
                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync();
                return await Result<long>.SuccessAsync(request.Id, $"Delete feedback by id successfully!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ApiException(ex.Message);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}