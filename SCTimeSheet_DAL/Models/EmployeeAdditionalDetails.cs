using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_TS_EmployeeAdditionalDetails")]
    public class EmployeeAdditionalDetails : EntityBase
    {
        [Key]
        public string EmpCode { get; set; }

        public string EmpCostCentre { get; set; }

        public string EmpCostCentreDescription { get; set; }

        public DateTime? JoinDate { get; set; }

        public DateTime? LeavingDate { get; set; }
    }
}
