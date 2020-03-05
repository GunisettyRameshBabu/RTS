using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
  public class EmployeeProjectList
    {
        [Key]
        public Int64 EmployeeID { get; set; }

        [Display(Name ="Project Member")]
        public string EmpName { get; set; }

        [Display(Name = "Project Manager")]
        public string IsManager { get; set; }

        [Display(Name = "% of Project Involvement")]
        public decimal InvPercentage { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Roles")]
        public Int64 RoleID { get; set; }

        public int IsRDProject { get; set; }

        public bool CheckRole { get; set; }

        public long ProjectId { get; set; }

    }
    public class EditEmployeeProjectList
    {
        public List<EmployeeProjectList> Items { get; set; }
    }
}
