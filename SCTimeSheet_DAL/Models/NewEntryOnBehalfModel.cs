using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    //[Table("T_TS_EmpTimesheet")]
    public class NewEntryOnBehalfModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 TsID { get; set; }

        public string RefNo { get; set; }

        public Int64 EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public int EntryRole { get; set; }

        [Required]
        [Display(Name = "InvolveDate", ResourceType = typeof(ResourceDisplay))]
        public DateTime InvolveStartDate { get; set; }
        [Required]
        public DateTime InvolveEndDate { get; set; }
        [Required]
        [Display(Name = "InvolveMonth", ResourceType = typeof(ResourceDisplay))]
        public int InvolveMonth { get; set; }
        [Required]
        public Int64 EmpId { get; set; }
        [Required]
        public Int64 ProjectID { get; set; }
        [Required]
        public int DaysCount { get; set; }
        [Required]
        [Display(Name = "InvolvePercent", ResourceType = typeof(ResourceDisplay))]
        public int InvolvePercent { get; set; }

        public string EmpRemarks { get; set; }

        public int Status { get; set; }

        public DateTime ApproveRejectDate { get; set; }

        public string ApproveRejectComments { get; set; }

        public Int64 ApproveRejectUser { get; set; }

        //public string ApproveRejectStatus { get; set; }
        //public string ModifiedBy { get; set; }
        //public DateTime ModifiedDate { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }


    }
}
