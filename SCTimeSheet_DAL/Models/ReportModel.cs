using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_Reportlist")]
    public class ReportModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        [Display(Name = "ReportID", ResourceType = typeof(ResourceDisplay))]
        public Int64 ReportID { get; set; }

        public string ReportName { get; set; }

        [DefaultValue(true)]
        [Display(Name = "IsActive", ResourceType = typeof(ResourceDisplay))]
        public bool IsActive { get; set; }

        //[Display(Name = "Start Date")]
        //public DateTime? StartDate { get; set; }


        //[Display(Name = "End Date")]
        //public DateTime? EndDate { get; set; }
    }


}
