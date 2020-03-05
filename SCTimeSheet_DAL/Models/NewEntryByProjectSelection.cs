using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    public class NewEntryByProjectSelection
    {
        [Key]
        public Int64 ProjectID { get; set; }
        //public string ProID { get; set; }
        public Int64 EmployeeID { get; set; }
        [Display(Name = "Employee Name")]
        public string emp_name { get; set; }
        public string ProjectName { get; set; }
        [Display(Name = "Sys Calc")]
        public decimal InvolvementDays1 { get; set; }
        [Display(Name = "Sys Calc")]
        public decimal InvolvementDays2 { get; set; }
        [Display(Name = "Sys Calc")]
        public decimal InvolvementDays3 { get; set; }
        [Display(Name = "TS Entry")]
        [UIHint("InvolvementEditDays1")]
        public decimal InvolvementEditDays1 { get; set; }
        [Display(Name = "TS Entry")]
        [UIHint("InvolvementEditDays2")]
        public decimal InvolvementEditDays2 { get; set; }
        [Display(Name = "TS Entry")]
        [UIHint("InvolvementEditDays3")]
        public decimal InvolvementEditDays3 { get; set; }
        public string Quarter { get; set; }

        public string Month1 { get; set; } 
        public string Month2{ get; set; }
        public string Month3 { get; set; }

        public int Status1 { get; set; }        
        public int Status2 { get; set; }
        public int Status3 { get; set; }

        public bool IsEdit1 { get; set; }
        public bool IsEdit2 { get; set; }
        public bool IsEdit3 { get; set; }

       
        public decimal Month1MaxLimit { get; set; }
        
        public decimal Month2MaxLimit { get; set; }
       
        public decimal Month3MaxLimit { get; set; }

        public decimal? OldInvolvementEditDays1 { get; set; }
        public decimal? OldInvolvementEditDays2 { get; set; }
        public decimal? OldInvolvementEditDays3 { get; set; }

        [Display(Name = "Member Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Member End Date")]
        public DateTime EndDate { get; set; }

        public bool ShowWarning { get; set; } = false;
    }
    //public class OnBehalfList
    //{
    //    public List<NewEntryByProjectSelection> Items { get; set; }
    //}
}
