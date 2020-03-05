using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("[T_TS_EmployeeDetails]")]
    public class EmployeeModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 EmployeeID { get; set; }

        public string EmpCode { get; set; }

        public string EmpFirstName { get; set; }

        public string EmpLastName { get; set; }

        public string EmpMiddleName { get; set; }

        public string EmpGender { get; set; }

        public DateTime? EmpDOB { get; set; }

        public Int64? Nationality { get; set; }

        public Int64? Qualification { get; set; }

        public Int64? UserID { get; set; }

        public int? TotalInvolvement { get; set; }

        public bool? IsAutoActive { get; set; }

        public long? PermenantResidence { get; set; }

        public string Email { get; set; }

        public long? RoleID { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string Designation { get; set; }

        public long? Company { get; set; }

    }
}
