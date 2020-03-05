using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SCTimeSheet.Models
{
    public class ReportUIModel
    {


        [Display(Name = "Report ID")]
        public long ReportID { get; set; }

        public string ReportName { get; set; }

        [DefaultValue(true)]
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

    }

   public class FTEReport
    {
        public decimal? Sum { get; set; }
        public long? EmpId { get; set; }
        public string Qualification { get; set; }
        public string ResearchType { get; set; }
        public long? EmpRole { get; set; }
    }
}