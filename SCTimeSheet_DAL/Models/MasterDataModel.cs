using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCTimeSheet_DAL.Models
{
    [Table("[T_Mst_MasterData]")]
    public class MasterDataModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long MstID { get; set; }

        public long MstTypeID { get; set; }
        [Required(ErrorMessage = "Please enter Grant Code")]
        public string MstCode { get; set; }
        [Required(ErrorMessage = "Please enter Grant Type")]
        public string MstName { get; set; }

        public int? MstOrder { get; set; }

        public bool? IsActive { get; set; }
    }
}

