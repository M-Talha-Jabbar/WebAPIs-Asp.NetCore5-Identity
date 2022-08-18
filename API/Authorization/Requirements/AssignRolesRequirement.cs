using Microsoft.AspNetCore.Authorization;

namespace API.Authorization.Requirements
{
    public class AssignRolesRequirement : IAuthorizationRequirement
    {
        // Requirement: An admin can assign role(s) to other admins & normal users but cannot assign role(s) to himself.

        // Here we cannot satisfy our authorization requirement using RequireAssertion() because here we need to access the body of the request.
        // Also as authorization requirements get complex, you may need access to other services via dependency injection.
        // So for these two purposes, we need to create Custom Authorization Requirement and Handler(s) for it.
    }
}
