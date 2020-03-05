using SCTimeSheet_UTIL.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("[T_TS_QuarterList]")]
    public  class QuarterListModel
   {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "QID", ResourceType = typeof(ResourceDisplay))]
        public Int64 QID { get; set; }

        public string Quarter { get; set; }

    }
}
