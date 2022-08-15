using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.ViewModels;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public AdministratorController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpPost("Role")]
        public async Task<IActionResult> CreateRole([FromForm]CreateRoleViewModel model)
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

        [HttpGet("Role")]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles; // Roles property of RoleManager class returns the list of all IdentityRole objects.

            return Ok(roles);
        }
    }
}
