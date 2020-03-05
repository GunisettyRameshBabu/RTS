using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("[T_Mst_Grant]")]
    public class ProjectGrantModel//(NotUse)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Int64 GID { get; set; }

        public string GType { get; set; }

        public Int64 GCode { get; set; }

        public string GName { get; set; }

        public Int64 GOrder { get; set; }

    }

}
