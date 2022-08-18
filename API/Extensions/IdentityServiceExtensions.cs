using API.Authorization.Handlers;
using API.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CreateRolePolicy", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return (context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "CREATE ROLE")) || context.User.IsInRole("Super Admin");
                    });
                }); 

                options.AddPolicy("EditRolePolicy", policy =>
                {
                    policy.RequireAssertion(context => 
                    {
                        return (context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "EDIT ROLE")) || context.User.IsInRole("Super Admin");
                    });
                });

                options.AddPolicy("DeleteRolePolicy", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return (context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "DELETE ROLE")) || context.User.IsInRole("Super Admin");
                    });
                });

                options.AddPolicy("AssignRolePolicy", policy =>
                {
                    policy.AddRequirements(new AssignRolesRequirement());
                });
            });


            // Registering the Handlers
            services.AddScoped<IAuthorizationHandler, CanAssignRolesToOnlyOtherUsersHandler>();
            services.AddScoped<IAuthorizationHandler, IsSuperAdminHandler>();


            return services;
        }
    }
}
