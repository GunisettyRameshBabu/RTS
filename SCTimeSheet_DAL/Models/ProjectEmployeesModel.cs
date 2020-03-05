using SCTimeSheet_UTIL.Resource;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace SCTimeSheet_DAL.Models
{
    [Table("[T_TS_ProjectEmployees]")]
    public class ProjectEmployeesModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Int64 ProjectEmpID { get; set; }

        [Required]
        [Display(Name = "ProjectID", ResourceType = typeof(ResourceDisplay))]
        public Int64 ProjectID { get; set; }

        [Required]
        [Display(Name = "EmployeeID", ResourceType = typeof(ResourceDisplay))]
        public Int64 EmployeeID { get; set; }


        public Int64 RefRole { get; set; }


        [Required]
        [Display(Name = "CheckRole", ResourceType = typeof(ResourceDisplay))]
        public bool CheckRole { get; set; }


        public DateTime? StartDate { get; set; }


        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "InvPercentage", ResourceType = typeof(ResourceDisplay))]
        public decimal? InvPercentage { get; set; }

        [Required]
        [Display(Name = "ProjectGrant", ResourceType = typeof(ResourceDisplay))]
        public Int64? ProjectGrant { get; set; }

        public bool IsActive { get; set; }

        public bool? IsAutoActive { get; set; }


        public void ValidateModel(ModelStateDictionary modelState, Int64 empid, Int64 projid)
        {
            try
            {
                ApplicationDBContext db = new ApplicationDBContext();

                if (this.ProjectEmpID == 0)
                {
                    var roleNameCount = db.ProjectEmployee.Where(x => x.ProjectID == ProjectID && x.EmployeeID == empid).Count();
                    if (roleNameCount != 0)
                        modelState.AddModelError("ProjectID", ResourceMessage.Alreadyexist);
                }
                else
                {
                    var roleNameCount = db.ProjectEmployee.Where(x => x.ProjectID == ProjectID && x.EmployeeID == empid && x.ProjectEmpID != ProjectEmpID).Count();
                    if (roleNameCount != 0)
                        modelState.AddModelError("ProjectID", ResourceMessage.Alreadyexist);
                }
                if (this.EndDate != null)
                {

                    var list = (from pm in db.ProjectMaster
                                where pm.ProjectID == projid
                                select new
                                {

                                    pm.EndDate,


                                }).FirstOrDefault();

                    if (this.EndDate > list.EndDate)
                    {
                        modelState.AddModelError("EndDate", ResourceMessage.Enddateexceed);
                    }

                }
                if (this.StartDate != null)
                {
                    if (this.StartDate > this.EndDate)
                    {

                        modelState.AddModelError("StartDate", ResourceMessage.Startdate);
                    }
                }



                if (this.ProjectEmpID == 0)
                {

                    var sum = 0;
                    var inputedinvolvmentpercentage = this.InvPercentage;

                    var list = (from pe in db.ProjectEmployee.AsEnumerable()
                                where pe.EmployeeID == empid
                                select new
                                {

                                    pe.ProjectID,
                                    pe.InvPercentage,
                                    StartDate = (pe.StartDate == null) ? DateTime.Now.Date : (pe.StartDate),
                                    EndDate = (pe.EndDate == null) ? DateTime.Now.Date : (pe.EndDate)
                                    //StartDate = (pe.StartDate == null) ? null : (pe.StartDate),
                                    //EndDate = (pe.EndDate == null) ? null : (pe.EndDate)

                                }).AsEnumerable().Where(x => x.StartDate <= DateTime.Now.Date && x.EndDate >= (x.EndDate)).ToList();


                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {

                            sum = sum + (int)list[i].InvPercentage + (int)inputedinvolvmentpercentage;
                        }
                        if (sum > 100)
                        {
                            modelState.AddModelError("InvPercentage", ResourceMessage.Invpercexceed);
                        }
                    }
                }
                else
                {

                    var sum = 0;
                    var inputedinvolvmentpercentage = this.InvPercentage;

                    var list = (from pe in db.ProjectEmployee.AsEnumerable()
                                where pe.EmployeeID == empid
                                select new
                                {
                                    pe.ProjectEmpID,
                                    pe.ProjectID,
                                    pe.InvPercentage,
                                    StartDate = (pe.StartDate == null) ? DateTime.Now : (pe.StartDate),
                                    EndDate = (pe.EndDate == null) ? DateTime.Now : (pe.EndDate)

                                }).AsEnumerable().Where(x => x.StartDate <= DateTime.Now.Date && x.EndDate >= (x.EndDate) && x.ProjectEmpID != ProjectEmpID).ToList();


                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {

                            sum = sum + (int)list[i].InvPercentage + (int)inputedinvolvmentpercentage;
                        }
                        if (sum > 100)
                        {
                            modelState.AddModelError("InvPercentage", ResourceMessage.Invpercexceed);
                        }
                    }
                }


            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}





