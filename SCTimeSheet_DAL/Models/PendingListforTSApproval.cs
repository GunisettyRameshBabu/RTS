using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    public class PendingListforTSApproval
    {
        [Key]
        public Int64 TsID { get; set; }
        public string RefNo { get; set; }
        public string SubmittedBy { get; set; }
        public string ProjectName { get; set; }
        public string Period { get; set; }
        public string SubmittedOn { get; set; }
        public Int64 Status { get; set; }
        public string ApproveRejectComments { get; set; }
        public string Quart { get; set; }
        public string InvolvementDays1 { get; set; }
        public string InvolvementDays2 { get; set; }
        public string InvolvementDays3 { get; set; }
        public string InvolvementEditDays1 { get; set; }
        public string InvolvementEditDays2 { get; set; }
        public string InvolvementEditDays3 { get; set; }
    }
}
