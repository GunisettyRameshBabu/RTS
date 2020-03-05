using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("T_Mst_PageMapping")]
    public class PageMappingModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 PageMappingID { get; set; }

        [Required]
        [Display(Name = "PageID", ResourceType = typeof(ResourceDisplay))]
        public Int64 PageID { get; set; }

        [Required]
        [Display(Name = "RoleID", ResourceType = typeof(ResourceDisplay))]
        public Int64 RoleID { get; set; }

        [DefaultValue(true)]
        [Display(Name = "IsActive", ResourceType = typeof(ResourceDisplay))]
        public bool IsActive { get; set; }
    }
}
