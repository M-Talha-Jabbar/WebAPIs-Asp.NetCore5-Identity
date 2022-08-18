using Service.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels
{
    public class UserClaimsViewModel
    {
        [Required] public string UserName { get; set; }
        [Required] [EnsureClaimsAssignedAttribute(ErrorMessage = "One of your Claim doesn't exist in ClaimStore")] public List<string> claims { get; set; }
    }
}
