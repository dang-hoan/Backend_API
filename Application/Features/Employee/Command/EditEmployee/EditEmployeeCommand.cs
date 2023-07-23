using Application.Interfaces;
using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Identity;
using Application.Interfaces.WorkShift;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Employee.Command.EditEmployee
{
    public class EditEmployeeCommand : IRequest<Result<EditEmployeeCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? Gender { get; set; }
        public IFormFile? ImageFile { get; set; }
        public long WorkShiftId { get; set; }
    }

    internal class EditEmployeeCommandHandler : IRequestHandler<EditEmployeeCommand, Result<EditEmployeeCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IUserService _userService;
        private readonly IUploadService _uploadService;
        private readonly IRemoveImageService _removeImageService;
        private readonly ICheckFileType _checkFileType;
        private readonly IWorkShiftRepository _workshiftRepository;


        public EditEmployeeCommandHandler(IMapper mapper, IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, IUserService userService,
                                        IUploadService uploadService, IRemoveImageService removeImageService, ICheckFileType checkFileType, IWorkShiftRepository workshiftRepository)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _uploadService = uploadService;
            _removeImageService = removeImageService;
            _checkFileType = checkFileType;
            _workshiftRepository = workshiftRepository;
        }

        public async Task<Result<EditEmployeeCommand>> Handle(EditEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }
            var editEmployee = await _employeeRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);
            if(request.Email != editEmployee.Email)
            {
                var existEmail = _employeeRepository.Entities.FirstOrDefault(_ => _.Email == request.Email && _.IsDeleted == false);
                if (existEmail != null)
                {
                    return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.EMAIL_EXISTS_MSG);
                }
            }
            if (request.PhoneNumber.Length < 8 || request.PhoneNumber.Length > 10)
            {
                return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.PHONE_ERROR_MSG);
            }
            var isExistedWorkshift = await _workshiftRepository.FindAsync(_ => _.IsDeleted == false && _.Id == request.WorkShiftId) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_WORK_SHIFT);
            _mapper.Map(request, editEmployee);


            if (request.ImageFile != null)
            {
                var msgCheck = _checkFileType.CheckFilesIsImage(new Dtos.Requests.CheckImagesTypeRequest
                {
                    Files = new List<IFormFile>() { request.ImageFile }
                });

                if (msgCheck != "")
                    return await Result<EditEmployeeCommand>.FailAsync(msgCheck);

                if (editEmployee.Image != null && !_removeImageService.RemoveImage(new Dtos.Requests.RemoveImageRequest
                {
                    FilePath = editEmployee.Image
                }))
                    return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.SERVER_ERROR_MSG);

                var filePath = _uploadService.UploadAsync(new Dtos.Requests.UploadRequest
                {
                    FileName = request.ImageFile.FileName,
                    Extension = Path.GetExtension(request.ImageFile.FileName),
                    Data = _mapper.Map<byte[]>(request.ImageFile)
                });
                if (string.IsNullOrEmpty(filePath))
                {
                    return await Result<EditEmployeeCommand>.FailAsync($"File {request.ImageFile.FileName} isn't not upload!");
                }
                editEmployee.Image = filePath;
            }
            await _employeeRepository.UpdateAsync(editEmployee);
            await _unitOfWork.Commit(cancellationToken);
            await _userService.EditUser(new Dtos.Requests.Identity.EditUserRequest
            {
                Id = request.Id,
                TypeFlag = TypeFlagEnum.Employee,
                FullName = request.Name,
                Email = request.Email,
                Phone = request.PhoneNumber
            });
            return await Result<EditEmployeeCommand>.SuccessAsync(request);
        }
    }
}
