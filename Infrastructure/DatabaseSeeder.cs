using Application.Interfaces;
using Application.Interfaces.EnumMasterData;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Entities.EnumMasterData;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IEnumMasterDataRepository _enumMasterDataRepository;
        private readonly IDateTimeService _dateTimeService;

        public DatabaseSeeder(
            ILogger<DatabaseSeeder> logger, 
            ApplicationDbContext context, 
            UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager,
            IEnumMasterDataRepository enumMasterDataRepository,
            IDateTimeService dateTimeService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _enumMasterDataRepository = enumMasterDataRepository;
            _dateTimeService = dateTimeService;
        }

        public void Initialize()
        {
            AddAdministrator();
            _context.SaveChanges();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                var adminRole = new AppRole()
                {
                    Name = RoleConstants.AdministratorRole,
                    Description = "Administrator role with full permission"
                };
                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                    _logger.LogInformation("Seeded Administrator Role.");
                }

                var employeeRole = new AppRole()
                {
                    Name = RoleConstants.EmployeeRole,
                    Description = "Employee role with custom permission"
                };
                var employeeRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.EmployeeRole);
                if (employeeRoleInDb == null)
                {
                    await _roleManager.CreateAsync(employeeRole);
                    _logger.LogInformation("Seeded Employee Role.");
                }

                var customerRole = new AppRole()
                {
                    Name = RoleConstants.CustomerRole,
                    Description = "Customer role with custom permission"
                };
                var customerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.CustomerRole);
                if (customerRoleInDb == null)
                {
                    await _roleManager.CreateAsync(customerRole);
                    _logger.LogInformation("Seeded Customer Role.");
                }

                //Check if User Exists
                var superUser = new AppUser()
                {
                    FullName = "Nguyen Phuoc Le Hieu",
                    Email = "lehieu.qrt@gmail.com",
                    UserName = "superadmin",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                var superUserInDb = await _userManager.FindByNameAsync(superUser.UserName);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Seeded Default SuperAdmin User.");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                }

                var existWaitingEnum = _enumMasterDataRepository.Entities
                                        .Where(x => x.Value.Equals(StaticVariable.WAITING) && !x.IsDeleted).FirstOrDefault() != null;
                var existInprogressingEnum = _enumMasterDataRepository.Entities
                                        .Where(x => x.Value.Equals(StaticVariable.INPROGRESSING) && !x.IsDeleted).FirstOrDefault() != null;
                var existDoneEnum = _enumMasterDataRepository.Entities
                                        .Where(x => x.Value.Equals(StaticVariable.DONE) && !x.IsDeleted).FirstOrDefault() != null;

                if(!existWaitingEnum)
                    await _enumMasterDataRepository.AddAsync(
                        new EnumMasterData
                        {
                            Value = StaticVariable.WAITING,
                            EnumType = StaticVariable.BOOKING_STATUS_ENUM,
                            CreatedBy = "System",
                            CreatedOn = _dateTimeService.NowUtc
                        });

                if(!existInprogressingEnum)
                    await _enumMasterDataRepository.AddAsync(
                        new EnumMasterData
                        {
                            Value = StaticVariable.INPROGRESSING,
                            EnumType = StaticVariable.BOOKING_STATUS_ENUM,
                            CreatedBy = "System",
                            CreatedOn = _dateTimeService.NowUtc
                        });

                if(!existDoneEnum)
                    await _enumMasterDataRepository.AddAsync(
                        new EnumMasterData
                        {
                            Value = StaticVariable.DONE,
                            EnumType = StaticVariable.BOOKING_STATUS_ENUM,
                            CreatedBy = "System",
                            CreatedOn = _dateTimeService.NowUtc
                        });

                if(!(existWaitingEnum && existInprogressingEnum && existDoneEnum))
                    _logger.LogInformation("Seeded Booking Status basic enum.");

            }).GetAwaiter().GetResult();
        }
    }
}