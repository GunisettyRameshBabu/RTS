using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("[T_Mst_EmailTemplates]")]
    public class EmailTemplates
    {
        [Key]
        [Column(Order = 0)]
        public int EmailTemplateID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(255)]
        public string EmailTemplateCode { get; set; }

        [Key]
        [Column(Order = 2)]
        public string EmailSubject { get; set; }

        [Key]
        [Column(Order = 3)]
        public string EmailBody { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime CreatedDate { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(10)]
        public string ModifiedBy { get; set; }
    }
}
