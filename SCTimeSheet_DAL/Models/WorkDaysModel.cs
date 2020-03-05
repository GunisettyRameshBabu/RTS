using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_Emp_Workingdays")] 

    public  class WorkDaysModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 WorkID { get; set; }

        public Int64? EmpID { get; set; }

        public DateTime InvolveMonth { get; set; }

        public decimal DaysCount { get; set; }
    }
}
