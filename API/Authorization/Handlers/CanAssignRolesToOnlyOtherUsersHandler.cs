using API.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Service.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Authorization.Handlers
{
    public class CanAssignRolesToOnlyOtherUsersHandler : AuthorizationHandler<AssignRolesRequirement, UserRoleViewModel> // The generic parameter <T> is the type of the requirement.
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AssignRolesRequirement requirement, UserRoleViewModel model)
        {
            //var authFilterContext = context.Resource as AuthorizationFilterContext;

            var loggedInAdminUserName = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

            //var userNameInBodyOfRequest = authFilterContext.HttpContext.Request.Form.ToArray(); // Username of person to which we are assigning the role

            if(context.User.IsInRole("Admin") && loggedInAdminUserName != model.Username)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
