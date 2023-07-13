﻿using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employee.Command.ResetPasswordEmployee
{
    public class ResetPasswordEmployeeCommand : IRequest<Result<ResetPasswordEmployeeCommand>>
    {
        public string Username { get; set; }
    }
    internal class ResetPasswordEmployeeCommandHandler : IRequestHandler<ResetPasswordEmployeeCommand, Result<ResetPasswordEmployeeCommand>>
    {
        private readonly UserManager<AppUser> _userManager;
        public ResetPasswordEmployeeCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<ResetPasswordEmployeeCommand>> Handle(ResetPasswordEmployeeCommand request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(request.Username))
            {
                return await Result<ResetPasswordEmployeeCommand>.FailAsync("This employee doesn't exist in the database");
            }
            var user = await _userManager.FindByNameAsync(request.Username);
            if(user == null)
            {
                return await Result<ResetPasswordEmployeeCommand>.FailAsync("This employee doesn't exist in the database");
            }
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            var result = _userManager.ResetPasswordAsync(user, resetToken, "Abc123!@#");
            if (!result.Result.Succeeded)
            {
                return await Result<ResetPasswordEmployeeCommand>.FailAsync("Resetting the password of the employee was unsuccessful.");
            }
            return await Result<ResetPasswordEmployeeCommand>.SuccessAsync("Resetting the password of the employee was successful.");
        }
    }
}