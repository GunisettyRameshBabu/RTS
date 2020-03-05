using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_Roles")]
    public class RoleModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 RoleID { get; set; }

        [Required]
        [Display(Name = "RoleName", ResourceType = typeof(ResourceDisplay))]
        public string RoleName { get; set; }

        [DefaultValue(true)]
        [Display(Name = "IsActive", ResourceType = typeof(ResourceDisplay))]
        public bool IsActive { get; set; }
    }
}
