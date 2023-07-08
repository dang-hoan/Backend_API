//using Application.Interfaces;
//using Application.Interfaces.Services.Permission;
//using Domain.Entities;
//using Domain.Wrappers;
//using Infrastructure.Contexts;
//using Microsoft.AspNetCore.Identity;

//namespace Infrastructure.Services.Permission
//{
//    public class PermissionAccessService : IPermissionAccessService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly RoleManager<AppRole> _roleManager;
//        private readonly ICurrentUserService _currentUserService;

//        public PermissionAccessService(ApplicationDbContext context, RoleManager<AppRole> roleManager, ICurrentUserService currentUserService)
//        {
//            _context = context;
//            _roleManager = roleManager;
//            _currentUserService = currentUserService;
//        }

//        public async Task<Result<List<PermissionByRoleResponse>>> GetPermissionByRole()
//        {
//            var role = await _roleManager.FindByNameAsync(_currentUserService.RoleName);
//            if (role == null) return await Result<List<PermissionByRoleResponse>>.FailAsync(UserErrorCode.USR_006);
//            var menuByRole = await (from menu in _context.SysMenu
//                                    join permission in _context.SysPermission
//                                        on menu.Id equals permission.Id
//                                    where permission.RoleId.Equals(role.Id)
//                                          && !menu.IsDeleted && !role.IsDeleted
//                                    select new PermissionByRoleResponse()
//                                    {
//                                        Id = menu.Id,
//                                        DisplayOrder = menu.DisplayOrder,
//                                        Icon = menu.Icon,
//                                        NameEn = menu.NameEn,
//                                        NameJa = menu.NameJa,
//                                        NameVi = menu.NameVi,
//                                        ParentId = menu.ParentId,
//                                        Url = menu.Url
//                                    }).ToListAsync();
//            return await Result<List<PermissionByRoleResponse>>.SuccessAsync(menuByRole);
//        }
//    }
//}