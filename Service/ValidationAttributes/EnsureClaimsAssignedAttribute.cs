using Repository.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ValidationAttributes
{
    public class EnsureClaimsAssignedAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as List<string>;

            foreach(var item in list)
            {
                if(item == null || !ClaimStore.AllClaims.Any(c => c.Type.ToUpper() == item.ToUpper())) // we have make use of ToUpper() to make this claim checking case in-sensitive.
                {
                    return false;
                }
            }

            return true;
        }
    }
}
