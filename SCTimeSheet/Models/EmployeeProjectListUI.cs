using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCTimeSheet.Models
{
    public class EmployeeProjectListUI
    {
        [Key]
        public Int64 EmployeeID { get; set; }

        [Display(Name = "Project Member")]
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

        
        [Display(Name = "Roles")]
        public Int64 RoleID { get; set; }

        public int IsRDProject { get; set; }

        [UIHint("ProjectMemberRole")]
        [Display(Name ="Roles")]
        public long? EmpRole { get; set; }

        public bool IsNew { get; set; }

        public DateTime? JoinDate { get; set; }
    }
}