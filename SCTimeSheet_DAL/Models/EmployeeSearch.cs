using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
   public class EmployeeSearch
    {
        public Int64 EmployeeID { get; set; }

        [Display(Name ="Name")]
        public string EmpName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Company")]
        public string Company { get; set; }

        public string Department { get; set; }

        public DateTime? JoinDate { get; set; }

        public DateTime? LeavingDate { get; set; }

        public long? RoleID { get; set; }
    }
}
