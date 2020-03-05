using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_Settings")]
    public class SettingsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 SetID { get; set; }

        public string SetCode { get; set; }

        [Required]
        [Display(Name = "SetValue", ResourceType = typeof(ResourceDisplay))]
        public Int64 SetValue { get; set; }
    }
}
