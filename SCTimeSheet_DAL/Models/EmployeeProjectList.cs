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

        [UIHint("IsManager")]
        [Display(Name = "Project Manager")]
        public bool IsManager { get; set; }

        [UIHint("ProjectMemberInvPercentage")]
        [Display(Name = "% of Project Involvement")]
        public decimal InvPercentage { get; set; }

        [UIHint("ProjectMemberStartDate")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [UIHint("ProjectMemberEndDate")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [UIHint("RoleID")]
        [Display(Name = "Roles")]
        public Int64 EmpRole { get; set; }

        public int IsRDProject { get; set; }

        public bool CheckRole { get; set; }

        public long ProjectId { get; set; }

    }
    public class EditEmployeeProjectList
    {
        public List<EmployeeProjectList> Items { get; set; }
    }
}
