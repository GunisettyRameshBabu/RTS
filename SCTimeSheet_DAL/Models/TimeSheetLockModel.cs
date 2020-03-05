using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("t_Mst_TimeSheetLock")]
    public class TimeSheetLockModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Status { get; set; } = true;

        public long updatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        [MaxLength(7)]
        public string Quarter
        {
            get;set;
            //get
            //{
            //    string q = "";
            //    switch (StartDate.Month)
            //    {
            //        case 01:
            //        case 02:
            //        case 03:
            //            q = "Q1";
            //            break;
            //        case 04:
            //        case 05:
            //        case 06:
            //            q = "Q2";
            //            break;
            //        case 07:
            //        case 08:
            //        case 09:
            //            q = "Q3";
            //            break;
            //        case 10:
            //        case 11:
            //        case 12:
            //            q = "Q4";
            //            break;
            //    }
            //    return q;

            //}
           
            //set { }
        }
    }

    public class TimeSheetLockUIModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; }

        public string updatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        [MaxLength(7)]
        public string Quarter
        {
            get;set;
        }
    }
}
