using API.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository.Identity;
using Service.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IAuthorizationService authorizationService;

        public AdministratorController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        [HttpGet("Role")]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles; // Roles property of RoleManager class returns the list of all IdentityRole objects.

            return Ok(roles);
        }

        [Authorize(Policy = "CreateRolePolicy")]
        [HttpPost("Role")]
        public async Task<IActionResult> CreateRole([FromForm]CreateDeleteRoleViewModel model)
        {
            var role = new IdentityRole()
            {
                Name = model.RoleName
            };

            var result = await roleManager.CreateAsync(role); // You get a validation error, if you try to create a role with the same name that already exists.

            if (result.Succeeded)
            {
                return Ok($"Role {role.Name} Created");
            }

            return BadRequest(result.Errors);
        }

        [Authorize(Policy = "EditRolePolicy")]
        [HttpPatch("Role")]
        public async Task<IActionResult> EditRole([FromForm]EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                return BadRequest($"Role with Id = {model.RoleId} cannot be found");
            }

            role.Name = model.NewRoleName;

            var result = await roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            return BadRequest(result.Errors);
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        [HttpDelete("Role")]
        public async Task<IActionResult> DeleteRole([Required][FromForm]CreateDeleteRoleViewModel model)
        {
            var role = await roleManager.FindByNameAsync(model.RoleName);

            if (role != null)
            {
                var result = await roleManager.DeleteAsync(role); // Cascade Delete is the default behaviour here.

                if (result.Succeeded)
                {
                    return Ok($"Role {model.RoleName} deleted");
                }

                return BadRequest(result.Errors);
            }

            return BadRequest($"Role {model.RoleName} doesn't exist");
        }

        [HttpGet("Role/Users")]
        public async Task<IActionResult> GetUsersInARole([FromQuery]string roleName)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                return Ok($"{roleName} doesn't exist");
            }

            var users = await userManager.GetUsersInRoleAsync(roleName);

            if (users.Count == 0)
            {
                return Ok($"There are no users with the role {roleName}");
            }

            return Ok(users);
        }

        //[Authorize(Policy = "AssignRolePolicy")]
        [Authorize]
        [HttpPost("User/Role")]
        public async Task<IActionResult> AssignRoleToAUser([FromForm]UserRoleViewModel model)
        {
            var requirement = new AssignRolesRequirement();
            var resource = model;
            var authorizationResult = await authorizationService.AuthorizeAsync(User, resource, requirement);

            if (authorizationResult.Succeeded)
            {

                var roleExists = await roleManager.RoleExistsAsync(model.RoleName);

                if (!roleExists)
                {
                    return Ok($"Role {model.RoleName} doesn't exist");
                }

                var user = await userManager.FindByNameAsync(model.Username);

                if (user != null)
                {
                    var result = await userManager.AddToRoleAsync(user, model.RoleName);

                    if (result.Succeeded)
                    {
                        return Ok($"User {model.Username} added to role {model.RoleName}");
                    }

                    return BadRequest(result.Errors);
                }

                return BadRequest($"User {model.Username} doesn't exist");
            }

            return Forbid();
        }

        [HttpDelete("User/Roles")]
        public async Task<IActionResult> RemoveAllRolesFromUser([FromForm]UserRoleViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);

            if(user != null)
            {
                var roles = await userManager.GetRolesAsync(user);

                if(roles.Count > 0)
                {
                    var result = await userManager.RemoveFromRolesAsync(user, roles);

                    if (result.Succeeded)
                    {
                        return Ok($"User {model.Username} have no roles now");
                    }

                    return BadRequest(result.Errors);
                }

                return Ok($"User {model.Username} has no roles");
            }

            return BadRequest($"User {model.Username} doesn't exist");
        }

        [HttpPost("User/Claims")]
        public async Task<IActionResult> AddUserClaims([FromForm]UserClaimsViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);

            if(user != null)
            {
                var userClaims = await userManager.GetClaimsAsync(user);

                foreach(var claimType in model.claims)
                {
                    if(userClaims.Any(c => c.Type == claimType.ToUpper()))
                    {
                        return BadRequest($"Claim {claimType.ToUpper()} is already assigned to user {model.UserName}");
                    }
                }

                var result = await userManager.AddClaimsAsync(user, model.claims.Select(c => new Claim(c.ToUpper(), c.ToUpper())).ToList());

                if (result.Succeeded)
                {
                    return Ok("Claims added");
                }

                return BadRequest(result.Errors);
            }

            return BadRequest($"User {user.UserName} doesn't exist");
        }

        [HttpGet("User")]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;

            return Ok(users);
        }

        [HttpDelete("User")]
        public async Task<IActionResult> DeleteUser([Required][FromForm]string userName)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user != null)
            {
                var result = await userManager.DeleteAsync(user); // Cascade Delete is the default behaviour here.
                // Cascade Delete means if a record in the parent table is deleted, then the corresponding records in the child table(s) will automatically be deleted.

                if (result.Succeeded)
                {
                    return Ok($"User {userName} deleted");
                }

                return BadRequest(result.Errors);
            }

            return BadRequest($"User {userName} doesn't exist");
        }

        [AllowAnonymous]
        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return BadRequest("Access Denied");
        }
    }
}
