using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_UserDetails")]
    public class UserModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 UserID { get; set; }

        [Required]
        [Display(Name = "Email", ResourceType = typeof(ResourceDisplay))]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password", ResourceType = typeof(ResourceDisplay))]
        public string Password { get; set; }

        [DefaultValue(0)]
        [Display(Name = "RoleID", ResourceType = typeof(ResourceDisplay))]
        public Int64 RoleID { get; set; }

        [DefaultValue(true)]
        [Display(Name = "IsActive", ResourceType = typeof(ResourceDisplay))]
        public bool IsActive { get; set; }
    }
}
