using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_AutoGen")]
    public class AutoGenModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int RefID { get; set; }
        
        public DateTime Date { get; set; }
    }
}
