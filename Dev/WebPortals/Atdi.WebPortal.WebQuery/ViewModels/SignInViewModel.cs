using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.ViewModels
{
    public class SignInViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
