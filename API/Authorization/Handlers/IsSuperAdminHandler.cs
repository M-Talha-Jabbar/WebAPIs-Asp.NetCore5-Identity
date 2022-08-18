using API.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace API.Authorization.Handlers
{
    public class IsSuperAdminHandler : AuthorizationHandler<AssignRolesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AssignRolesRequirement requirement)
        {
            if(context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
