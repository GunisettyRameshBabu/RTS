using SCTimeSheet_UTIL;
using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_TS_EmpTimesheet")]

    public class NewEntryModel:EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 TsID { get; set; }

        public string RefNo { get; set; }

        public int SequenceNo { get; set; }

        public Int64 EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public Int64 EntryRole { get; set; }

        [Display(Name = "EmpRemarks", ResourceType = typeof(ResourceDisplay))]
        public string EmpRemarks { get; set; }

        public Int64 Status { get; set; }

        public DateTime? ApproveRejectDate { get; set; }

        public string ApproveRejectComments { get; set; }

        public Int64? ApproveRejectUser { get; set; }

        public string ApproveRejectStatus { get; set; }

        public string Quart { get; set; }

        [Required]
        [Display(Name = "InvolveMonth", ResourceType = typeof(ResourceDisplay))]

        public DateTime InvolveMonth { get; set; }
        [Required]
        [Display(Name = "EmpId", ResourceType = typeof(ResourceDisplay))]
        public Int64 EmpId { get; set; }
        [Required]
        [Display(Name = "ProjectID", ResourceType = typeof(ResourceDisplay))]
        public Int64 ProjectID { get; set; }
        [Required]
        [Display(Name = "InvolveDaysCount", ResourceType = typeof(ResourceDisplay))]
        public decimal DaysCount { get; set; }
        [Required]
        [Display(Name = "InvolveDaysEditCount", ResourceType = typeof(ResourceDisplay))]
        public decimal DaysEditCount { get; set; }
        [Required]
        [Display(Name = "InvolvePercent", ResourceType = typeof(ResourceDisplay))]
        public decimal InvolvePercent { get; set; }


       

        public void ValidateModel(ModelStateDictionary modelState,Int64 empid )
        {
            try
            {
                ApplicationDBContext db = new ApplicationDBContext();

                if (this.TsID == 0)
                {
                    var roleNameCount = db.EmpTimeSheet.Where(x => x.InvolveMonth.Month == InvolveMonth.Month && x.InvolveMonth.Year == InvolveMonth.Year && x.ProjectID == ProjectID && x.EmpId==empid).Count();
                    if (roleNameCount != 0)
                        modelState.AddModelError("ProjectID", ResourceMessage.InvolveMonthValid);
                }
                else
                {
                    var roleNameCount = db.EmpTimeSheet.Where(x => x.InvolveMonth.Month == InvolveMonth.Month && x.InvolveMonth.Year == InvolveMonth.Year && x.ProjectID == ProjectID && x.EmpId==empid && x.TsID != TsID).Count();
                    if (roleNameCount != 0)
                        modelState.AddModelError("ProjectID", ResourceMessage.InvolveMonthValid);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
