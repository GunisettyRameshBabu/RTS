using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
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
    public class ManagerDashboardController : BaseController
    {
        // GET: ManagerDashboard
        [NoCache]
        public ActionResult Index()
        {
            GetDropdownList();
            return View();
        }

        [HttpGet]
        public ActionResult ManagerProjectDetails(int? ProjectId)
        {
            //Session["ProjectId"] = ProjectId;
            ViewBag.projectID = ProjectId;
            long empId = Convert.ToInt64(Session[Constants.SessionEmpID]);
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

        [NoCache]
        public ActionResult Timesheet()
        {
            ViewBag.ProjectId = Session["ProjectId"];
            return View();
        }

        public void GetDropdownList()
        {
            ViewBag.ProjectList = DropdownList.ProjectList(Convert.ToInt64(Session[Constants.SessionEmpID]), Convert.ToInt64(Session[Constants.SessionRoleID]));
            ViewBag.StatusList = DropdownList.StatusList();
            ViewBag.EmployeeList = DropdownList.EmployeeList();
            ViewBag.GradeList = DropdownList.GrantList();
        }



        [HttpPost]
        public JsonResult DetailPopup(int Id)
        {
            try
            {
                List<NewEntryList> GetDetails = DB.Database.SqlQuery<NewEntryList>(
                              @"exec " + Constants.P_GetEmpTimesheet_NewEntryPOPUP + " @TsID",
                              new object[] {
                        new SqlParameter("@TsID", Id)
                              }).ToList();
                //var NewEnryModel = DB.NewEntry.Join(DB.ProjectMaster,x=>x.ProjectID,y=>y.ProjectID,(x,y)=>new { x.InvolveMonth,x.InvolvePercent}) .OrderBy(x => x.ProjectID).ToList();

                //var list = (from x in DB.NewEntry
                //            join y in DB.Status on x.Status equals y.StatusID
                //            join p in DB.ProjectMaster on x.ProjectID equals p.ProjectID
                //            where x.TsID == x.TsID
                //            select new  { x.RefNo, p.ProjectName , x.InvolveMonth,x.DaysCount , x.TsID , x.InvolvePercent , x.ApproveRejectComments , y.StatusDesc , Quard = x.Quart  });

                //List<NewEntryList> GetDetails = new List<NewEntryList>();
                //foreach (var item in list)
                //{
                //    GetDetails.Add(new NewEntryList() { RefNo = item.RefNo , ProjectName = item.ProjectName , InvolveMonth = item.InvolveMonth.ToString("MMM yyyy") , DaysCount = item.DaysCount , TsID = item.TsID , InvolvePercent = item.InvolvePercent, ApproveRejectComments = item.ApproveRejectComments ,
                //    StatusDesc = item.StatusDesc , Quard = item.Quard});
                //}

                return Json(new { data = GetDetails });
            }
            catch (Exception ex)
            {

                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }



        [HttpPost]
        public JsonResult TimesheetApprove(string[] DATA)
        {

            try
            {
                foreach (string i in DATA)
                {

                    ActionItems res = JsonConvert.DeserializeObject<ActionItems>(i);
                    NewEntryModel existing = DB.EmpTimeSheet.Find(res.TsID);
                    if (existing != null)
                    {
                        existing.InvolvePercent = 0;
                        existing.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));
                        existing.ApproveRejectComments = "";
                        existing.ApproveRejectStatus = "A";
                        existing.ApproveRejectUser = (long)Session[Constants.SessionEmpID];
                        existing.ApproveRejectDate = DateTime.Now;
                        DB.EmpTimeSheet.Attach(existing);
                        DB.Entry(existing).State = System.Data.Entity.EntityState.Modified;
                        DB.SaveChanges();

                    }

                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
                LogHelper.ErrorLog(ex);
            }
            return Json(new { data = true });
        }




        [HttpPost]
        public JsonResult TimesheetReject(string[] DATA)
        {

            try
            {
                foreach (string i in DATA)
                {

                    ActionItems res = JsonConvert.DeserializeObject<ActionItems>(i);
                    NewEntryModel existing = DB.EmpTimeSheet.Find(res.TsID);
                    if (existing != null)
                    {
                        existing.Status = Convert.ToInt64(ReadConfig.GetValue("StatusRejected"));
                        existing.ApproveRejectComments = "";
                        existing.ApproveRejectStatus = "R";
                        existing.ApproveRejectUser = (long)Session[Constants.SessionEmpID];
                        existing.ApproveRejectDate = DateTime.Now;
                        DB.EmpTimeSheet.Attach(existing);
                        DB.Entry(existing).State = System.Data.Entity.EntityState.Modified;
                        DB.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
                LogHelper.ErrorLog(ex);
            }
            return Json(new { data = true });
        }

       
        [HttpPost]
        public JsonResult GetSelectedProject([DataSourceRequest]DataSourceRequest request, int projectid)
        {
            try
            {
               
                var GetDetails = Models.Common.GetProjectDetails(projectid,true);
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

        
       

        [HttpPost]
        public ActionResult GetTestvalues(string projectid)
        {
            try
            {
                Session["ProjectId"] = projectid;
                return Json(new { redirectUrl = "ManagerDashboard/Timesheet" });
                //return RedirectToAction("Timesheet", "ManagerDashboard",pd);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult Search([DataSourceRequest] DataSourceRequest request, long projectid, int grantId)
        {
            GetDropdownList();
            try
            {
               

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

        public class ActionItems
        {

            public long TsID { get; set; }


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