using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_Status")]
    public class StatusModel
    {
        [Key]
        public Int64 StatusID { get; set; }

        public string StatusCode { get; set; }

        public string StatusDesc { get; set; }
    }
}
