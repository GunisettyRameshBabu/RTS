using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_Pages")]
    public class PageModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 PageID { get; set; }

        public string PageName { get; set; }

        public string PageDesc { get; set; }

        public bool IsDefault { get; set; }
    }
}
