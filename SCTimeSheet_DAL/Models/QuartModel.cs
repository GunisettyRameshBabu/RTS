using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{

    [Table("[T_Mst_Quarter]")]

    public class QuartModel 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Int64 QID { get; set; }

        public string Month { get; set; }

        public string Quarter { get; set; }

    }
}
