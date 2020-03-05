using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_Country")]
    public class Country : EntityBase
    {
        [Key]
        public long CountryID { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string CountryOrder { get; set; }

        public bool IsActive { get; set; }

        public string Cotinent { get; set; }
    }
}
