using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels
{
    public class RegisterRequest
    {
        [Required] [MaxLength(20)] public string UserName { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
        [Required][Compare("ConfirmPassword", ErrorMessage = "Passwords don't match")] public string Password { get; set; }
        [Required] public string ConfirmPassword { get; set; }
    }
}
