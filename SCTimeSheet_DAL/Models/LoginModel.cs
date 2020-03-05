using SCTimeSheet_UTIL.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Email", ResourceType = typeof(ResourceDisplay))]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password", ResourceType = typeof(ResourceDisplay))]
        public string Password { get; set; }
    }
}
