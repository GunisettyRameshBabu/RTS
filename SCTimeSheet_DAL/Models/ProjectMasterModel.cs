using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace SCTimeSheet_DAL.Models
{


    public class AddProjectEmployee
    {
        public Int64 ProjectEmpID { get; set; }

        public Int64 ProjectID { get; set; }

        public Int64 EmployeeID { get; set; }


        public Int64 RefRole { get; set; }

        public string CheckRole { get; set; }


        public DateTime? StartDate { get; set; }


        public DateTime? EndDate { get; set; }


        public Int64? InvPercentage { get; set; }


        public bool IsActive { get; set; }

    }


    [Table("[T_Mst_ProjectMaster]")]
    public class ProjectMasterModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 ProjectID { get; set; }

        [Required]
        public string ProjectCode { get; set; }

        [Required]
        public string ProjectName { get; set; }

        [Display(Name = "ProjectDesc", ResourceType = typeof(ResourceDisplay))]
        public string ProjectDesc { get; set; }

        [Required]
        public string InternalOrder { get; set; }

        [Required]
        [Display(Name = "CostCentre", ResourceType = typeof(ResourceDisplay))]
        public string CostCentre { get; set; }

        [Required]
        [Display(Name = "ProjectGrant", ResourceType = typeof(ResourceDisplay))]
        public int ProjectGrant { get; set; }

       
        [Display(Name = "ResearchArea", ResourceType = typeof(ResourceDisplay))]
        public Int64? ResearchArea { get; set; }

       
        [Display(Name = "TypeofResearch", ResourceType = typeof(ResourceDisplay))]
        public Int64? TypeofResearch { get; set; }


        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ProjectDuration { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public string SearchText { get; set; }

        
        public int? IsRDProject { get; set; }

        public int? Theme { get; set; }

        public void ValidateModel(System.Web.Mvc.ModelStateDictionary modelState)
        {
            try
            {
                ApplicationDBContext db = new ApplicationDBContext();

                if (this.StartDate != null)
                {
                    if (this.StartDate > this.EndDate)
                    {

                        modelState.AddModelError("StartDate", ResourceMessage.Startdate);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [NotMapped]
        public AddProjectEmployee ProjectEmployeeModel { get; set; }

        
    }

    public class ProjectMasterModelNew
    {
       
        public Int64 ProjectID { get; set; }

        
        public string ProjectCode { get; set; }

        
        public string ProjectName { get; set; }

        [Display(Name = "ProjectDesc", ResourceType = typeof(ResourceDisplay))]
        public string ProjectDesc { get; set; }

        
        public string InternalOrder { get; set; }

        
        [Display(Name = "CostCentre", ResourceType = typeof(ResourceDisplay))]
        public string CostCentre { get; set; }

        
        [Display(Name = "ProjectGrant", ResourceType = typeof(ResourceDisplay))]
        public int ProjectGrant { get; set; }
        
        public int? Theme { get; set; }


        [Display(Name = "ResearchArea", ResourceType = typeof(ResourceDisplay))]
        public Int64? ResearchArea { get; set; }

        
        [Display(Name = "TypeofResearch", ResourceType = typeof(ResourceDisplay))]
        public Int64? TypeofResearch { get; set; }


        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ProjectDuration { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public string SearchText { get; set; }


        public int IsRDProject { get; set; } = 2;

        public void ValidateModel(System.Web.Mvc.ModelStateDictionary modelState)
        {
            try
            {
                ApplicationDBContext db = new ApplicationDBContext();

                if (this.StartDate != null)
                {
                    if (this.StartDate > this.EndDate)
                    {

                        modelState.AddModelError("StartDate", ResourceMessage.Startdate);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [NotMapped]
        public AddProjectEmployee ProjectEmployeeModel { get; set; }

        public string ProjectMembers { get; set; }

        
        [Display(Name = "CheckRole")]
        public bool CheckRole { get; set; }


        public DateTime? MemberStartDate { get; set; }


        public DateTime? MemberEndDate { get; set; }

        
        [Display(Name = "InvPercentage")]
        public Int64? InvPercentage { get; set; }

        
        [Display(Name = "ProjectGrant")]
        public Int64? MemberProjectGrant { get; set; }

        public Int64 RefRole { get; set; }

        public string ProjectMembersNames { get; set; } = "";

        public string EmpSearchText { get; set; }
    }

}





