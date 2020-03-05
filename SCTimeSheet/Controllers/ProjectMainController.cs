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
using System.Web;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class ProjectMainController : BaseController
    {
        
        public ActionResult Index(string message = null)
        {
            if (string.IsNullOrEmpty(message) && TempData.ContainsKey("ProjectCreated"))
            {
                message = TempData["ProjectCreated"].ToString();
                TempData.Remove("ProjectCreated");
            }
            ViewBag.Message = message ?? "";
            ViewBag.GrantList = DropdownList.GrantList();
            return View();
        }
        [HttpPost]
        public JsonResult Read([DataSourceRequest]DataSourceRequest request)
        {

            try
            {
                var empId = Convert.ToInt64(Session[Constants.SessionEmpID]);
                var roleId = Convert.ToInt32(Session[Constants.SessionRoleID]);
                var GetDetails = DB.Database.SqlQuery<ProjectMasterList>(
                           @"exec " + Constants.P_GetEmpTimesheet_ProjectMasterEntry + " @empId,@roleId",
                           new object[] {
                               new SqlParameter("@empId",empId),
                               new SqlParameter("@roleId",roleId)
                           }).ToList();

                //var NewEnryModel = DB.NewEntry.Join(DB.ProjectMaster,x=>x.ProjectID,y=>y.ProjectID,(x,y)=>new { x.InvolveMonth,x.InvolvePercent}) .OrderBy(x => x.ProjectID).ToList();

                //var GetDetails = new List<ProjectMasterList>();
                //var list = (from x in DB.ProjectMaster
                //            join m in DB.MasterData on x.TypeofResearch equals m.MstID into researches
                //            from m in researches.DefaultIfEmpty()
                //            join re in DB.ResearchMaster on x.ResearchArea equals re.RsID into researchAreas
                //            from re in researchAreas.DefaultIfEmpty()
                //            join md in DB.MasterData on x.ProjectGrant equals md.MstID into projectGrants
                //            from md in projectGrants.DefaultIfEmpty()
                //            join co in DB.MasterData on x.CostCentre equals co.MstCode
                //            where x.IsActive
                //            select new { x.ProjectID, x.ProjectCode, x.ProjectName, x.ProjectDesc, x.InternalOrder, CostCenter = x.CostCentre, ProjectGrant = md.MstName, ResearchArea = re.RsDesc, x.StartDate, x.EndDate, x.IsActive ,  TypeOfResearch = m.MstName }).ToList();

                //if (roleId != 1)
                //{
                //    list = (from x in list
                //            join y in DB.ProjectEmployee on x.ProjectID equals y.ProjectID
                //            where y.CheckRole && y.EmployeeID == empId
                //            select x).ToList();
                //}

                //int i = 1;
                //foreach (var item in list)
                //{
                //    GetDetails.Add(new ProjectMasterList()
                //    {
                //        ProjectID = item.ProjectID,
                //        ProjectCode = item.ProjectCode,
                //        ProjectName = item.ProjectName,
                //        ProjectDesc = item.ProjectDesc,
                //        InternalOrder = item.InternalOrder,
                //        CostCentre = item.CostCenter,
                //        ProjectGrant = item.ProjectGrant,
                //        ResearchArea = item.ResearchArea,
                //        StartDate = item.StartDate.Value.ToString("dd MMM yyyy"),
                //        EndDate = item.EndDate.Value.ToString("dd MMM yyyy"),
                //        ItemNo = i++,
                //        TypeofResearch = item.TypeOfResearch
                //    });
                //}

                DataSourceResult result = GetDetails.ToDataSourceResult(request);
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