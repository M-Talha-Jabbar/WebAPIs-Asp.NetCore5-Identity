using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.ViewModels;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm]RegisterRequest model)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            // Store user data in AspNetUsers database table
            var result = await userManager.CreateAsync(user, model.Password);

            // If user is successfully created, sign-in the user using SignInManager
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                // isPersistent is used to specify whether we want to create session cookie or persistent cookie.
                // Here we are creating a session cookie (A session cookie is immediately lost when we close the browser window (Note: When all windows of a browser will be closed)).

                return Ok("Registered & Logged In");
            }
            
            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm]LoginRequest model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, false);

            if (result.Succeeded)
            {
                return Ok("Logged In");
            }

            return BadRequest("Invalid Login Attempt");
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // sign out the user by removing the session cookie from the user's browser

            return Ok("You have been Logged Out!!");
        }
    }
}
