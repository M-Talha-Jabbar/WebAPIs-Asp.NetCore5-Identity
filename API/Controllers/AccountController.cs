using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository.Identity;
using Service.Contracts;
using Service.ViewModels;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSenderService emailSenderService;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSenderService emailSenderService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSenderService = emailSenderService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm]RegisterViewModel model)
        {
            var user = new ApplicationUser()
            {
                UserName = model.UserName, // In AspNetUsers table Username is UNIQUE.
                Email = model.Email, // In AspNetUsers table Email is NOT Unique.
                City = model.City,
            };

            // Store user data in AspNetUsers database table
            var result = await userManager.CreateAsync(user, model.Password);

            // If user is successfully created, sign-in the user using SignInManager
            if (result.Succeeded)
            {
                var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, emailConfirmationToken = emailConfirmationToken }, Request.Scheme);
                // The last parameter Request.Scheme returns the request protocol such as Http or Https. This parameter is required to generate the full absolute URL. If this parameter is not specified, a relative URL will be generated.

                string emailStatus = await emailSenderService.SendEmailAsync(user.Email, user.UserName, confirmationLink);

                // But If the user is signed in and in the Admin role, then it is the Admin user that is creating a new user. So redirecting the Admin user to ListUsers action method in Administrator controller.
                // 'User' contains ClaimsPrincipal of the current user of the application.
                if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                {
                    return RedirectToAction("ListUsers", "Administrator");
                }

                return Ok("Registered successfully & now Confirm your Email to Login");
            }
            
            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm]LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, false);

            if (result.Succeeded)
            {
                return Ok("Logged In");
            }

            if (result.IsNotAllowed)
            {
                return BadRequest("Email not confirmed yet");
            }

            return BadRequest("Invalid Login Attempt");
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // sign out the user by removing the session cookie from the user's browser

            return Ok("You have been Logged Out!!");
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string emailConfirmationToken)
        {
            if(userId == null || emailConfirmationToken == null)
            {
                return BadRequest("Something went wrong");
            }

            var user = await userManager.FindByIdAsync(userId);

            if(user == null)
            {
                return BadRequest($"The User ID {userId} is invalid");
            }

            var result = await userManager.ConfirmEmailAsync(user, emailConfirmationToken); // This method sets EmailConfirmed column to True in AspNetUsers table.

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                // isPersistent is used to specify whether we want to create session cookie or persistent cookie.
                // Here we are creating a session cookie (A session cookie is immediately lost when we close the browser window (Note: When all windows of a browser will be closed)).

                return Ok("Your Email is confirmed & you have been Logged In");
            }

            return BadRequest(result.Errors);
        }
    }
}
