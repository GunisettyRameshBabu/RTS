using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SCTimeSheet_UTIL.Resource;

namespace SCTimeSheet_DAL.Models
{
    public class SearchModel
    {
        [Display(Name = "ProjectID", ResourceType = typeof(ResourceDisplay))]
        public Int64? ProjectID { get; set; }

        //[Required(ErrorMessage = "This field is required")]
        [Display(Name = "InvolveMonth", ResourceType = typeof(ResourceDisplay))]
        public string InvolveMonth { get; set; }

        [Display(Name = "Status", ResourceType = typeof(ResourceDisplay))]
        public Int64? Status { get; set; }

        [Display(Name = "IsSumbitByMe", ResourceType = typeof(ResourceDisplay))]
        public bool? IsSumbitByMe { get; set; }


        public int GrantTypeId { get; set; }

        public int GrantTypeName { get; set; }


        public List<TimeSheetList> Result { get; set; }
    }
}
