using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace SCTimeSheet_DAL.Models
{
    [Table("[T_Mst_ResearchAreas]")]
    public class ResearchModel 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 RsID { get; set; }

        [Required]
        public string RsCode { get; set; }

        [Display(Name = "RsDesc", ResourceType = typeof(ResourceDisplay))]
        public string RsDesc { get; set; }

        public Int64 Parent { get; set; }

        public bool IsActive { get; set; }


    }


}





