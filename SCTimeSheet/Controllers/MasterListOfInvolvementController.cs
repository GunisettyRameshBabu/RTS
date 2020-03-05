using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SCTimeSheet.Common;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class MasterListOfInvolvementController : BaseController
    {
        // GET: MasterListOfInvolvement
        public ActionResult Index()
        {
           // ViewBag.EmployeeList = DropdownList.EmployeeList();
            return View();
        }

        [HttpPost]
        public JsonResult GetTotalInvolvement(int empId)
        {
            var GetEmpTotalInvolvement = (from x in DB.Employee

                                          where x.EmployeeID == empId
                                          select new { x.TotalInvolvement, x.IsAutoActive }).FirstOrDefault();
            return Json(GetEmpTotalInvolvement);
        }

        [HttpPost]
        public ActionResult GetProjectDetails([DataSourceRequest]DataSourceRequest request, ProjectInvolvementPercentageModel model, int employeeId)
        {
           // ViewBag.EmployeeList = DropdownList.EmployeeList();
            try
            {
                List<ProjectInvolvementPercentageModel> GetDetails = GetEmployeeCurrentInvolvement(employeeId);
                DataSourceResult result = GetDetails.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        private List<ProjectInvolvementPercentageModel> GetEmployeeCurrentInvolvement(long employeeId)
        {
            //return DB.Database.SqlQuery<ProjectInvolvementPercentageModel>(
            //    @"exec " + Constants.P_GetMasterListOfTotalProjectInvolvement + " @empID",
            //    new object[] {
            //            new SqlParameter("@empID",employeeId)
            //    }).Distinct().ToList();
            int i = 1;
            List<ProjectInvolvementPercentageModel> list = (from x in DB.ProjectEmployee
                                                            join y in DB.ProjectMaster
                                                            on x.ProjectID equals y.ProjectID
                                                            where x.EmployeeID == employeeId && (x.EndDate == null || 
                                                            DbFunctions.TruncateTime(x.EndDate.Value) >= DbFunctions.TruncateTime(DateTime.Now))
                                                            select new ProjectInvolvementPercentageModel()
                                                            {
                                                                ProjectId = x.ProjectID,
                                                                EmployeeID = x.EmployeeID,
                                                                ProjectName = y.ProjectName,
                                                                StartDate = x.StartDate,
                                                                EndDate = x.EndDate,
                                                                AuditDate = x.ModifiedDate,
                                                                InvolvePercent = x.InvPercentage.Value,
                                                                ProjectStartDate = y.StartDate.Value , 
                                                                ProjectEndDate  = y.EndDate.Value

                                                            }
                                        ).ToList();

            foreach (ProjectInvolvementPercentageModel item in list)
            {
                item.ItemNo = i++;
            }

            return list;
        }

        [HttpPost]
        public ActionResult GetProjectHistoryDetails([DataSourceRequest]DataSourceRequest request, ProjectInvolvementPercentageModel model, int employeeId)
        {
           // ViewBag.EmployeeList = DropdownList.EmployeeList();
            try
            {
                var list = (from x in DB.ProjectEmployee
                            join y in DB.ProjectMaster on x.ProjectID equals y.ProjectID
                            join z in DB.AuditTimeSheetInfo on x.ProjectID equals z.tablePK
                            where x.EmployeeID == employeeId && z.tableName == "T_TS_ProjectEmployees"
                            && z.LastUpdatedUser == employeeId.ToString()
                            orderby z.AuditDate descending
                            select new
                            {
                                ProjectId = x.ProjectID,
                                x.EmployeeID,
                                y.ProjectName,
                                StartDate = z.startOldValue,
                                EndDate = z.endOldValue,
                                InvolvePercent = z.oldValue,
                                z.AuditDate
                            }).ToList();


                List<ProjectInvolvementPercentageModel> GetDetails = new List<ProjectInvolvementPercentageModel>();
                foreach (var item in list)
                {
                    GetDetails.Add(new ProjectInvolvementPercentageModel()
                    {
                        ProjectId = item.ProjectId,
                        EmployeeID = item.EmployeeID,
                        ProjectName = item.ProjectName,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        InvolvePercent = Convert.ToDecimal(item.InvolvePercent),
                        AuditDate = item.AuditDate
                    });
                }
                List<ProjectInvolvementPercentageModel> expireProjects = (from x in DB.ProjectEmployee
                                                                          join y in DB.ProjectMaster
                                                                          on x.ProjectID equals y.ProjectID
                                                                          where x.EmployeeID == employeeId && y.EndDate < DateTime.Now
                                                                          select new ProjectInvolvementPercentageModel()
                                                                          {
                                                                              ProjectId = x.ProjectID,
                                                                              EmployeeID = x.EmployeeID,
                                                                              ProjectName = y.ProjectName,
                                                                              StartDate = x.StartDate,
                                                                              EndDate = x.EndDate,
                                                                              AuditDate = x.ModifiedDate,
                                                                              InvolvePercent = x.InvPercentage.Value
                                                                          }
                                          ).ToList();

                GetDetails.Concat(expireProjects);


                for (int i = 1; i <= GetDetails.Count(); i++)
                {
                    GetDetails[i - 1].ItemNo = i;
                }
                DataSourceResult result = GetDetails.OrderBy(x => x.ItemNo).ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }



        //[AcceptVerbs("Post")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditEmpDetails([DataSourceRequest]DataSourceRequest request, IEnumerable<ProjectInvolvementPercentageModel> employees, ProjectInvolvementPercentageModel griditems)
        {
           // ViewBag.EmployeeList = DropdownList.EmployeeList();
            try
            {
                long empid = 0;
                foreach (ProjectInvolvementPercentageModel item in employees)
                {
                    empid = item.EmployeeID;
                    DateTime? startDate = null;
                    if (item.StartDate.HasValue)
                    {
                        startDate = Convert.ToDateTime(item.StartDate.Value);
                    }
                    DateTime? endDate = null;
                    if (item.EndDate.HasValue)
                    {
                        endDate = Convert.ToDateTime(item.EndDate.Value);
                    }
                    ProjectEmployeesModel existing = DB.ProjectEmployee.FirstOrDefault(x => x.EmployeeID == item.EmployeeID && x.ProjectID == item.ProjectId);//&& x.StartDate == startDate && x.EndDate == endDate
                    if (existing != null)
                    {
                        existing.StartDate = startDate;
                        existing.EndDate = endDate;
                        existing.InvPercentage = item.InvolvePercent;
                        DB.ProjectEmployee.Attach(existing);
                        DB.Entry(existing).State = System.Data.Entity.EntityState.Modified;
                        DB.SaveChanges();
                    }
                }
                List<ProjectInvolvementPercentageModel> GetDetails = DB.Database.SqlQuery<ProjectInvolvementPercentageModel>(
                    @"exec " + Constants.P_GetMasterListOfTotalProjectInvolvement + " @empID",
                    new object[] {
                        new SqlParameter("@empID",empid)
                    }).ToList();
                DataSourceResult result = employees.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        //[HttpPost]
        public JsonResult UpdateInvolvementLimit(string involvePercent, int EmdId)
        {
            EmployeeModel empDetaild = DB.Employee.FirstOrDefault(x => x.EmployeeID == EmdId);
            empDetaild.TotalInvolvement = Convert.ToInt32(involvePercent);
            DB.Employee.Attach(empDetaild);
            DB.Entry(empDetaild).State = System.Data.Entity.EntityState.Modified;
            DB.SaveChanges();
            return Json(new { data = true });
        }

        public JsonResult GetEmps()
        {
            return Json(DropdownList.EmployeeList(),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoAdjustPercentage(bool isAuto, long empId)
        {
            try
            {
                List<EmployeeModel> employees = (from x in DB.Employee
                                                where x.EmployeeID == empId
                                                select x).ToList();

                foreach (EmployeeModel item in employees)
                {
                    item.IsAutoActive = isAuto;
                    DB.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    DB.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json("");
        }


    }
}