using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    public class ProjectInvolvementPercentageModel
    {
        public Int64 ItemNo { get; set; }
        public Int64 EmployeeID { get; set; }
        public Int64 ProjectId { get; set; }
        public string ProjectName { get; set; }
        [Required(ErrorMessage ="Please select start date")]
        [UIHint("StartDateEditor")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Please select end date")]
        [UIHint("EndDateEditor")]
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "Please Enter Involvement Percent ")]
        [UIHint("NumericEditor")]
        public decimal InvolvePercent { get; set; }
        [Display(Name = "Project Start Date")]
        public DateTime ProjectStartDate { get; set; }
        [Display(Name = "Project End Date")]
        public DateTime ProjectEndDate { get; set; }

        [Display(Name = "Last Modified Date")]
        public DateTime? AuditDate { get; set; }

        public string OriginalStartDate { get { return  StartDate.HasValue ? StartDate.Value.ToString("dd MMM yyyy") : ""; } }

        public string OriginalEndDate { get { return EndDate.HasValue ? EndDate.Value.ToString("dd MMM yyyy") : ""; } }

        public decimal OldInvolvePercent { get { return InvolvePercent; } }
        //public int EmpLimit { get; set; }
    }
}
