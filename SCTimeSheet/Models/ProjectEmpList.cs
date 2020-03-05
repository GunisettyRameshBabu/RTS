using System;
using System.ComponentModel.DataAnnotations;

namespace SCTimeSheet.Models
{
    public class ProjectEmpList
    {

        [Key]
        public long EmployeeID { get; set; }

        [Display(Name = "Project Member")]
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
        public long RoleID { get; set; }

        public int IsRDProject { get; set; }

        public string ProjectName { get; set; }

        public DateTime? ProjectEndDate { get; set; }

        public bool CheckRole { get; set; }

        public long ProjectID { get; set; }


    }
}