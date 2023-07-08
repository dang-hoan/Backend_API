using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Exceptions;
using Domain.Constants;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private string Function { get; }
        private string Action { get; }

        public CustomAuthorizeAttribute()
        {
        }

        public CustomAuthorizeAttribute(string function, string action)
        {
            Function = function;
            Action = action;
        }

        /// <summary>
        /// Get EmployeeNo, FullName, RoleName from token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<(string EmployeeNo, string RoleName, string FullName, long Expires)> GetInfoFromToken(AuthorizationFilterContext context)
        {
            (string EmployeeNo, string RoleName, string FullName, long Expires) result = (string.Empty, string.Empty, string.Empty, 0);
            var tokenString = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(tokenString))
            {
                var jwtEncodedString = tokenString.Replace("Bearer ", "");
                var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                result.EmployeeNo = token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                result.RoleName = token.Claims.First(c => c.Type == ClaimTypes.Role).Value;
                result.FullName = token.Claims.First(c => c.Type == ClaimTypes.Name).Value;
                result.Expires = long.Parse(token.Claims.First(c => c.Type == "exp").Value);
            }

            return await Task.FromResult(result);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Get EmployeeNo, FullName, RoleName from token
            var (employeeNo, roleName, fullName, expires) = await GetInfoFromToken(context);

            var identityContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            if (string.IsNullOrEmpty(employeeNo) || string.IsNullOrEmpty(roleName))
                throw new CustomUnauthorizedAccessException();

            if (!string.IsNullOrEmpty(Function) && !string.IsNullOrEmpty(Action))
            {
                // Check active of employeeNo and Role in sys_table
                var checkUser = await identityContext.Users.FirstOrDefaultAsync(x => x.UserName == employeeNo && !x.IsDeleted);
                var checkRole = await identityContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName && !x.IsDeleted);

                if (checkUser == null && checkRole == null)
                    throw new CustomUnauthorizedAccessException();

                if (((DateTimeOffset)(DateTime.Now)).ToUnixTimeSeconds() < expires)
                {
                    //if (!string.IsNullOrEmpty(roleName))
                    //{
                    //    var menuUrl = await identityContext.SysMenu.FirstOrDefaultAsync(x => x.Url == Function && !x.IsDeleted);
                    //    if (menuUrl != null && menuUrl.ParentId != 0)
                    //    {
                    //        var menuParent = await identityContext.SysMenu.FirstOrDefaultAsync(x => x.Id == menuUrl.ParentId && !x.IsDeleted);
                    //        var permissionParent = await identityContext.SysPermission.FirstOrDefaultAsync(x => x.MenuId == menuParent.Id && x.RoleId == checkRole.Id && !x.IsDeleted);

                    //        if (permissionParent != null && !permissionParent.CanAccess)
                    //            throw new CustomUnauthorizedAccessException();
                    //    }

                    //    if (!await CheckPermission(identityContext, roleName))
                    //        throw new CustomUnauthorizedAccessException();
                    //}
                }
                else throw new CustomExpiresAccessException();
            }
            else throw new CustomUnauthorizedAccessException();
        }

        //private async Task<bool> CheckPermission(ApplicationDbContext identityContext, string roleName)
        //{
        //    var query = await (from permission in identityContext.SysPermission
        //                       join appRole in identityContext.Roles
        //                    on permission.RoleId equals appRole.Id
        //                       join menu in identityContext.SysMenu
        //                           on permission.MenuId equals menu.Id
        //                       where appRole.Name == roleName && menu.Url == Function && !appRole.IsDeleted && !menu.IsDeleted
        //                       && ((permission.CanAccess && Action == StaticVariable.ACCESS)
        //                       || (permission.CanAdd && Action == StaticVariable.ADD)
        //                       || (permission.CanEdit && Action == StaticVariable.EDIT)
        //                       || (permission.CanDelete && Action == StaticVariable.DELETE))
        //                       select permission).AnyAsync();
        //    return query;
        //}
    }
}