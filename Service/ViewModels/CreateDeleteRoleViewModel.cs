using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels
{
    public class CreateDeleteRoleViewModel
    {
        [Required] public string RoleName { get; set; }
    }
}
