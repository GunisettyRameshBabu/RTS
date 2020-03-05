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
    public class AdminDashboardController : BaseController
    {
        // GET: AdminDashboard
        [NoCache]
        public ActionResult Index()
        {
            //GetDropdownList();
            return View();
        }
        [HttpGet]
        public ActionResult ProjectDetails(int? ProjectId)
        {
           // Session["ProjectId"] = ProjectId;
            ViewBag.projectID = ProjectId;
            long empId = Convert.ToInt64(Session[Constants.SessionEmpID]);
            var projectName = (from x in DB.ProjectMaster
                               //join y in DB.ProjectEmployee on x.ProjectID equals y.ProjectID
                               where x.ProjectID == ProjectId 
                               select new { x.ProjectName }).FirstOrDefault();
            ViewBag.projectName = projectName.ProjectName;
            return View(true);
        }

        [HttpPost]
        public JsonResult Search([DataSourceRequest]DataSourceRequest request, long projectid, int grantId)
        {
            GetDropdownList();
            try
            {
                long empid = (long)Session[Constants.SessionEmpID];
                //long role = (long)Session[Constants.SessionRoleID];
                //long userid = (long)Session[Constants.SessionUserID];
                //if (role == 1)
                //{
                    List<ProjectMasterModel> projectList = DB.ProjectMaster.Where(x=> x.ProjectID == (projectid == 0 ? x.ProjectID : projectid) && x.ProjectGrant == (grantId == 0 ? x.ProjectGrant : grantId)) .ToList();
                    var projectEmpolyees = DB.ProjectEmployee.Where(x => x.EmployeeID == empid).ToList();
                    var projectGrants = DB.MasterData.Where(x => x.MstTypeID == 3).ToList();
                    List<SearchList> allProjectS = new List<SearchList>();
                    foreach (ProjectMasterModel item in projectList)
                    {
                        ProjectEmployeesModel project = projectEmpolyees.Where(x => x.ProjectID == item.ProjectID && x.EmployeeID == empid).FirstOrDefault();
                        string projectGrant = projectGrants.Where(x => x.MstID == item.ProjectGrant).FirstOrDefault()?.MstName;
                        if (project != null)
                        {
                            allProjectS.Add(new SearchList()
                            {
                                ProjectId = project.ProjectID,
                                isPM = project.CheckRole ? "YES" : "NO",
                                EmpEndDate = project.EndDate.Value.ToString("dd MMM yyyy"),
                                EmpStartDate = project.StartDate.Value.ToString("dd MMM yyyy"),
                                InvPercentage = project.InvPercentage.Value,
                                prgEndDate = item.EndDate.Value.ToString("dd MMM yyyy"),
                                prgStartDate = item.StartDate.Value.ToString("dd MMM yyyy"),
                                ProjectCode = item.ProjectCode,
                                ProjectGrant = projectGrant,
                                ProjectName = item.ProjectName
                            });
                        }
                        else
                        {
                            allProjectS.Add(new SearchList()
                            {
                                ProjectId = item.ProjectID,
                                isPM = "",
                                EmpEndDate = "",
                                EmpStartDate = "",
                                InvPercentage = null,
                                prgEndDate = item.EndDate.Value.ToString("dd MMM yyyy"),
                                prgStartDate = item.StartDate.Value.ToString("dd MMM yyyy"),
                                ProjectCode = item.ProjectCode,
                                ProjectGrant = projectGrant,
                                ProjectName = item.ProjectName
                            });
                        }
                    }

                return Json(allProjectS.ToDataSourceResult(request));

                  

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }

            //  return Json(new { data = "" });
        }

        public void GetDropdownList()
        {
            ViewBag.ProjectList = DropdownList.ProjectList(Convert.ToInt64(Session[Constants.SessionEmpID]), Convert.ToInt64(Session[Constants.SessionRoleID]));
            ViewBag.StatusList = DropdownList.StatusList();
            ViewBag.GradeList = DropdownList.GrantList();
        }

        public JsonResult GetGrantTypes(long? projectId, string text)
        {
            IEnumerable<MasterLookUp> projects = projectId.HasValue ? DropdownList.GetGrantListByProjectId(projectId.Value) : DropdownList.GrantList();



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
        public JsonResult GetSelectedProject([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            try
            {
                List<TestClass> GetDetails = SCTimeSheet.Models.Common.GetProjectDetails(projectId,true);
                
                DataSourceResult result = GetDetails.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetProjectDetails([DataSourceRequest]DataSourceRequest request, long[] projectId)
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

      
    }
}