using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SCTimeSheet.Common;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class EmployeeDashboardController : BaseController
    {
        // GET: EmployeeDashboard
        [NoCache]
        public ActionResult Index()
        {
            GetDropdownList();
            return View();
        }

        [HttpGet]
        public ActionResult EmployeeProjectDetails(int? ProjectId)
        {
            //Session["ProjectId"] = ProjectId;
            ViewBag.projectID = ProjectId;
            var empId = Convert.ToInt64(Session[Constants.SessionEmpID]);
            var projectName = (from x in DB.ProjectMaster
                               join y in DB.ProjectEmployee on x.ProjectID equals y.ProjectID
                               where x.ProjectID == ProjectId && y.EmployeeID == empId
                               select new { x.ProjectName, y.CheckRole }).FirstOrDefault();
            ViewBag.projectName = projectName.ProjectName;
            return View(projectName.CheckRole);
        }

        public JsonResult GetGrantTypes(long? projectId, string text)
        {
            var empId = (Int64)Session[Constants.SessionEmpID];
            IEnumerable<MasterLookUp> projects = projectId.HasValue ? DropdownList.GetGrantListByProjectId(projectId.Value,empId) : DropdownList.GrantList(empId);

            if (!string.IsNullOrEmpty(text))
            {
                projects = projects.Where(p => p.MstCode.ToLower().Contains(text.ToLower()));
            }

            return Json(projects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjects(long? grantID, string text)
        {
            IEnumerable<ProjectLookUp> projects = grantID.HasValue ? DropdownList.GetProjectListByGrantId(Convert.ToInt64(Session[Constants.SessionEmpID]), Convert.ToInt64(Session[Constants.SessionRoleID]), grantID.Value) : DropdownList.ProjectList(Convert.ToInt64(Session[Constants.SessionEmpID]), Convert.ToInt64(Session[Constants.SessionRoleID]));



            if (!string.IsNullOrEmpty(text))
            {
                projects = projects.Where(p => p.ProjectName.ToLower().Contains(text.ToLower()));
            }

            return Json(projects, JsonRequestBehavior.AllowGet);
        }
       
       
        [HttpPost]
        public JsonResult Search([DataSourceRequest]DataSourceRequest request, Int64 projectid,int grantId)
        {
            GetDropdownList();
            try
            {
                //projectid = projectid == 0 ? 0 : projectid;
                //grantId = grantId == 0 ? 0 : grantId;
                //Int64 empid = (Int64)Session[Constants.SessionEmpID];
                //var GetDetails = DB.Database.SqlQuery<SearchList>(
                //        @"exec " + Constants.P_GetEmpTimesheetBy_Grant + " @EmpID,@ProjectID,@GradeId",
                //        new object[] {
                //        new SqlParameter("@EmpID", empid),
                //        new SqlParameter("@ProjectID", projectid),
                //        new SqlParameter("@GradeId", grantId)
                //        }).ToList();
                long empid = (long)Session[Constants.SessionEmpID];
                List<SearchList> result = GetProjects(projectid, grantId, empid);
                return Json(result.ToDataSourceResult(request));
                //return Json(new
                //{
                //    data = result
                //});

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        private List<SearchList> GetProjects(long projectid, int grantId, long empid)
        {
            List<SearchList> result = new List<SearchList>();

            var res = (from x in DB.ProjectMaster
                       join y in DB.ProjectEmployee
                       on x.ProjectID equals y.ProjectID
                       join z in DB.MasterData on x.ProjectGrant equals z.MstID
                       where x.ProjectID == (projectid == 0 ? x.ProjectID : projectid) && x.ProjectGrant == (grantId == 0 ? x.ProjectGrant : grantId) && y.EmployeeID == empid && z.MstTypeID == 3
                       select new { x, y, z }).ToList();

            foreach (var item in res)
            {
                result.Add(new SearchList()
                {
                    ProjectId = item.x.ProjectID,
                    isPM = item.y.CheckRole ? "YES" : "NO",
                    EmpEndDate = getString(item.y.EndDate),
                    EmpStartDate = getString(item.y.StartDate),
                    InvPercentage = item.y.InvPercentage.Value,
                    prgEndDate = getString(item.x.EndDate),
                    prgStartDate = getString(item.x.StartDate),
                    ProjectCode = item.x.ProjectCode,
                    ProjectGrant = item.z.MstName,
                    ProjectName = item.x.ProjectName
                });
            }

            return result;
        }

        private string getString(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd MMM yyyy") : "";
        }

        public void GetDropdownList()
        {
            ViewBag.ProjectList = DropdownList.ProjectList(Convert.ToInt64(Session[Constants.SessionEmpID]), Convert.ToInt64(Session[Constants.SessionRoleID]));
            ViewBag.StatusList = DropdownList.StatusList();
            ViewBag.GradeList = DropdownList.GrantList();
        }
        [HttpPost]
        public JsonResult GetProjectDetails([DataSourceRequest]DataSourceRequest request, Int64[] projectId)
        {
            try
            {
                //   projectId = projectId == 0 ? 0 : projectId;
                if (projectId == null)
                {
                    projectId = new long[0];
                }
                var empId = (long)Session[Constants.SessionEmpID];
                List<MonthList2> lstResult = SCTimeSheet.Models.Common.GetTimeSheet(projectId, empId);

                DataSourceResult result = lstResult.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult GetSelectedProject([DataSourceRequest]DataSourceRequest request, int projectid,bool isManager)
        {
            try
            {
               
                var GetDetails = Models.Common.GetProjectDetails(projectid,isManager);
                DataSourceResult result = GetDetails.ToDataSourceResult(request);
                return Json(result);
                //return Json(GetDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }


    }
}