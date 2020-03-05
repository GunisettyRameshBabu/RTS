using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_TS_AuditTimeSheetInfo")]
   public class AuditTimeSheetInfo : EntityBase
    {
        [Key]
        public int AuditId { get; set; }

        public int tablePK { get; set; }

        public DateTime startOldValue { get; set; }

        public DateTime startNewValue { get; set; }

        public DateTime endOldValue { get; set; }

        public DateTime endNewValue { get; set; }

        public string tableName { get; set; }

        public string oldValue { get; set; }

        public string newValue { get; set; }

        public DateTime AuditDate { get; set; }

        public string LastUpdatedUser { get; set; }
    }
}
