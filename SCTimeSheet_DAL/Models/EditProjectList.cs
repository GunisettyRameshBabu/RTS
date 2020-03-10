using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
   public class ProjectListEdit
    {

        public ProjectListEdit()
        {
            
        }
        [Key]
        public Int64 ProjectID { get; set; }

        public string ProjectCode { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDesc { get; set; }

        public string InternalOrder { get; set; }

        public string CostCentre { get; set; }

        public int ProjectGrant { get; set; }

        public int? Theme { get; set; }

        public Int64? ResearchArea { get; set; }

        public Int64? TypeofResearch { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int IsRDProject { get; set; }

        //[Required]
        [Display(Name = "ProjectGrant")]
        public Int64? MemberProjectGrant { get; set; }

        public string EmpSearchText { get; set; }

       public string EmployeeProjectList { get; set; }
    }

    public class EditProjectList
    {
        public List<ProjectListEdit> Items { get; set; }
    }


}
