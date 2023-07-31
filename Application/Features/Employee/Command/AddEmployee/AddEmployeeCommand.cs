using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Employee;
using Domain.Constants.Enum;
using System.ComponentModel.DataAnnotations;
using Domain.Constants;
using Microsoft.AspNetCore.Http;
using Application.Interfaces;
using Application.Features.Service.Command.AddService;
using Application.Interfaces.WorkShift;

namespace Application.Features.Employee.Command.AddEmployee
{
    public class AddEmployeeCommand : IRequest<Result<AddEmployeeCommand>>
    {
        public long? Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? Gender { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public long WorkShiftId { get; set; }

    }

    internal class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, Result<AddEmployeeCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IUploadService _uploadService;
        private readonly ICheckFileType _checkFileType;
        private readonly ICheckSizeFile _checkSizeFile;
        private readonly IWorkShiftRepository _workshiftRepository;


        public AddEmployeeCommandHandler(IMapper mapper, IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, IAccountService accountService,
                                                IUploadService uploadService, ICheckFileType checkFileType, ICheckSizeFile checkSizeFile, IWorkShiftRepository workshiftRepository)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _uploadService = uploadService;
            _checkFileType = checkFileType;
            _checkSizeFile = checkSizeFile;
            _workshiftRepository = workshiftRepository;
        }

        public async Task<Result<AddEmployeeCommand>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            request.Id = null;
            bool isUsernameExists = await _accountService.IsExistUsername(request.Username);
            if (isUsernameExists)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.USERNAME_EXISTS_MSG);
            }
            var existEmail = _employeeRepository.Entities.FirstOrDefault(_ => _.Email == request.Email && _.IsDeleted == false);
            if (existEmail != null)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.EMAIL_EXISTS_MSG);
            }
            if (request.Password.Length < 8)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.INVALID_PASSWORD);
            }
            if (request.PhoneNumber.Length < 8 || request.PhoneNumber.Length > 10)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.PHONE_ERROR_MSG);
            }
            var isExistedWorkshift = await _workshiftRepository.FindAsync(_ => _.IsDeleted == false && _.Id == request.WorkShiftId);
            if (isExistedWorkshift == null) return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_WORK_SHIFT);
            var addEmployee = _mapper.Map<Domain.Entities.Employee.Employee>(request);

            if (request.ImageFile != null)
            {
                List<IFormFile> listImages = new List<IFormFile>();
                listImages.Add(request.ImageFile);
                var msgCheck = _checkFileType.CheckFilesIsImage(new Dtos.Requests.CheckImagesTypeRequest
                {
                    Files = listImages
                });

                var imageCheckMaxSize = _checkSizeFile.CheckImageSize(new Dtos.Requests.CheckImageSizeRequest
                {
                    Files = listImages
                });

                if (msgCheck != "")
                    return await Result<AddEmployeeCommand>.FailAsync(msgCheck);

                if (imageCheckMaxSize != "")
                    return await Result<AddEmployeeCommand>.FailAsync(imageCheckMaxSize);
                    
                var filePath = _uploadService.UploadAsync(new Dtos.Requests.UploadRequest
                {
                    FileName = request.ImageFile.FileName,
                    Extension = Path.GetExtension(request.ImageFile.FileName),
                    Data = _mapper.Map<byte[]>(request.ImageFile)
                });

                if (string.IsNullOrEmpty(filePath))
                {
                    return await Result<AddEmployeeCommand>.FailAsync($"File {request.ImageFile.FileName} isn't not upload!");
                }
                addEmployee.Image = filePath;
            }

            await _employeeRepository.AddAsync(addEmployee);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addEmployee.Id;
            var user = new AppUser()
            {
                FullName = request.Name,
                Email = request.Email,
                UserName = request.Username,
                EmailConfirmed = true,
                PhoneNumber = request.PhoneNumber,
                PhoneNumberConfirmed = true,
                CreatedOn = DateTime.Now,
                IsActive = true,
                TypeFlag = TypeFlagEnum.Employee,
                UserId = (long)request.Id,
            };
            bool result = await _accountService.AddAcount(user, request.Password, RoleConstants.EmployeeRole);
            if (result == false)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.ERROR_ADD_USER);
            }
            return await Result<AddEmployeeCommand>.SuccessAsync(request);
        }
    }
}
