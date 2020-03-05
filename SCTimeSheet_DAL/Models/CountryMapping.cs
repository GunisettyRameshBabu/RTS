using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_CountryMapping")]
    public class CountryMapping 
    {
        [Key]
        public long CountryMapId { get; set; }

        public string CountrySet { get; set; }

        public long CountryId { get; set; }

        public int MapOrder { get; set; }
    }
}
