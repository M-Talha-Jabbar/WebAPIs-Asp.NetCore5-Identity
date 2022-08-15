using Service.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels
{
    public class RegisterViewModel
    {
        [Required] [MaxLength(20)] public string UserName { get; set; }
        [Required] [EmailAddress] [ValidEmailDomain(allowedDomain: "google.com", ErrorMessage = "Email domain must be google.com")] public string Email { get; set; }
        [Required][Compare("ConfirmPassword", ErrorMessage = "Passwords don't match")] public string Password { get; set; }
        [Required] public string ConfirmPassword { get; set; }
        public string City { get; set; }
    }
}
