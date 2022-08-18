using API.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Service.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Authorization.Handlers
{
    public class CanAssignRolesToOnlyOtherUsersHandler : AuthorizationHandler<AssignRolesRequirement> // The generic parameter <T> is the type of the requirement.
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AssignRolesRequirement requirement)
        {
            var resource = context.Resource as HttpContext;
            // The use of the Resource property is framework specific. Using information in the Resource property will limit your authorization policies to particular frameworks.
            // For example MVC passes an instance of Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext in the resource property which is used to access HttpContext, RouteData and everything else MVC provides.
            // Similarly with Web API we cast this Resource property as Microsoft.AspNetCore.Http.HttpContext.
            // Resource property of AuthorizationHandlerContext is of type 'object' so thats why we can cast it to any other framework specific context which framework itself pass in the Resource property. So Framework is doing boxing(context type -> object type) while we are doing Un-boxing(object type -> context type(framework specific)).

            if (resource == null)
            {
                return Task.CompletedTask;
            }

            var userNameInBodyOfRequest = resource.Request.Form.FirstOrDefault(f => f.Key == "Username").Value;

            var loggedInAdminUserName = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

            if (context.User.IsInRole("Admin") && loggedInAdminUserName != userNameInBodyOfRequest)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
