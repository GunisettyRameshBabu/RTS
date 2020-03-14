using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SCTimeSheet.Common;
using SCTimeSheet.Models;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using SCTimeSheet_UTIL.Resource;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace SCTimeSheet.Controllers
{
    public class ProjectMasterController : BaseController
    {
        // GET: ProjectMaster

        [NoCache]
        public ActionResult Index()
        {
            GetDefaults();
            //ViewBag.Pid = TempData["pId"];

            return View(new ProjectMasterModelNew());
        }

        private void GetDefaults()
        {
            ViewBag.RoleList = DropdownList.RoleList();
            ViewBag.GrantList = DropdownList.GrantList();
            ViewBag.ThemeList = DropdownList.ThemeList();
            ViewBag.ResearchList = DropdownList.ResearchList();
            ViewBag.ResearchTypeList = DropdownList.ResearchTypeList();
            ViewBag.projectList = DropdownList.ProjectListAdmin();
            ViewBag.ProjectID = TempData["projectid"];
            ViewBag.costList = DropdownList.CostCenterList();
        }

        [NoCache]
        [Route("ProjectEdit/{id}")]
        public ActionResult ProjectEdit(long id)
        {
            ViewBag.Message = TempData["message"] ?? "";
            // int ProjectId = Convert.ToInt16(Session["ProjectId"]);
            ViewBag.GrantList = DropdownList.GrantList();
            ViewBag.ThemeList = DropdownList.ThemeList();
            ViewBag.RoleList = DropdownList.RoleList();
            ViewBag.projectList = DropdownList.ProjectListAdmin();
            ViewBag.ResearchList = DropdownList.ResearchList();
            ViewBag.ResearchTypeList = DropdownList.ResearchTypeList();
            ViewBag.costList = DropdownList.CostCenterList();
            ProjectListEdit GetDetails = DB.Database.SqlQuery<ProjectListEdit>(
                           @"exec " + Constants.P_GetProject_Details_Edit + " @ProjectId",
                           new object[] {
                        new SqlParameter("@ProjectId",  id)
                           }).ToList().FirstOrDefault();

            ViewBag.ProjectId = id;
            ViewBag.GrantID = GetDetails.ProjectGrant;
            //ViewBag.ResearchList = DropdownList.ResearchList();
            //ViewBag.ResearchTypeList = DropdownList.ResearchTypeList();
            ViewBag.ReasearchID = GetDetails.ResearchArea;
            ViewBag.TypeID = GetDetails.TypeofResearch;
            return View(GetDetails);
        }

        [HttpPost]
        [SubmitButton(Name = "action", Argument = "SaveAsDraft")]
        public ActionResult Save(ProjectMasterModelNew model)
        {
            try
            {
                GetDefaults();
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var modifiedOrNewEmpList = json_serializer.Deserialize<EmployeeProjectList[]>(model.EmployeeProjectList);

                ModelState.Remove("ProjectID");
                // model.ValidateModel(ModelState);
                model.MemberProjectGrant = model.ProjectGrant;

                Session["ProjectGrant"] = model.ProjectGrant;
                model.IsActive = true;
                if (model.Theme.HasValue)
                {
                    var eligibleThemeGrants = ConfigurationManager.AppSettings["ThemeGrandCodes"].ToString().Split(',').ToList();
                    var themeMstType = Convert.ToInt32(ConfigurationManager.AppSettings["TypeGrant"]);
                    var themes = DB.MasterData.Where(x => x.MstTypeID == themeMstType && eligibleThemeGrants.Contains(x.MstCode)).Select(x => x.MstID);
                    if (!themes.Contains(model.ProjectGrant))
                    {
                        model.Theme = null;
                    }
                }
                ProjectMasterModel newModel = new ProjectMasterModel
                {
                    ProjectID = model.ProjectID,
                    ProjectCode = model.ProjectCode,
                    ProjectName = model.ProjectName,
                    ProjectDesc = model.ProjectDesc,
                    InternalOrder = model.InternalOrder,
                    CostCentre = model.CostCentre,
                    ProjectGrant = model.ProjectGrant,
                    ResearchArea = model.ResearchArea,
                    TypeofResearch = model.TypeofResearch,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ProjectDuration = model.ProjectDuration,
                    IsActive = true,
                    IsRDProject = model.IsRDProject,
                    Theme = model.Theme
                };
                DB.ProjectMaster.Add(newModel);
                DB.SaveChanges();

                long projectId = DB.ProjectMaster.Where(x => x.ProjectCode == model.ProjectCode).Select(x => x.ProjectID).FirstOrDefault();
                string projectName = DB.ProjectMaster.Where(x => x.ProjectCode == model.ProjectCode).Select(x => x.ProjectName).FirstOrDefault();
                Session["pId"] = projectId;
                Session["pName"] = projectName;
                TempData["projectid"] = 1;

                TempData["Success"] = ResourceMessage.NewProjectAdd;

                Session["ProjectGrant"] = model.ProjectGrant;

                if (modifiedOrNewEmpList.Any())
                {
                    model.ProjectID = projectId;

                    model.IsActive = true;

                    foreach (var item in modifiedOrNewEmpList)
                    {
                        EmployeeModel refRole = DB.Employee.FirstOrDefault(x => x.EmployeeID == item.EmployeeID);
                        List<EmployeeProjectList> GetDetails = DB.Database.SqlQuery<EmployeeProjectList>(
                 @"exec " + Constants.P_InsertProjectEmployee + " @ProjectId,@EmployeeID,@CheckRole,@InvPercentage,@RefRole,@StartDate,@EndDate,@Grant,@IsActive,@CreatedBy",
                 new object[] {
                        new SqlParameter("@ProjectId",   model.ProjectID),
                        new SqlParameter("@EmployeeID", item.EmployeeID),
                       new SqlParameter("@CheckRole",   item.IsManager),
                        new SqlParameter("@InvPercentage",  item.InvPercentage),
                        new SqlParameter("@RefRole",  model.IsRDProject == 1 ? (refRole.RoleID ?? 0) : 0),
                        new SqlParameter("@StartDate",   item.StartDate),
                          new SqlParameter("@EndDate",   item.EndDate),
                          new SqlParameter("@Grant", model.ProjectGrant),
                          new SqlParameter("@IsActive",   model.IsActive),
                          new SqlParameter("@CreatedBy",Session["EmployeeId"].ToString())
                 }).ToList();

                        if (model.IsRDProject == 1 && refRole.RoleID == null)
                        {
                            ViewBag.Message = ViewBag.Message + $"line Employee { SCTimeSheet.Models.Common.GetName(refRole.EmpFirstName, refRole.EmpLastName, refRole.EmpMiddleName) } Role is blank, please modify manually or contact administrator";
                        }

                        if (!string.IsNullOrEmpty(refRole.Email))
                        {
                            if (!DB.User.Any(x => x.Email == refRole.Email))
                            {
                                UserModel user = new UserModel
                                {
                                    Email = refRole.Email,
                                    CreatedBy = 1,
                                    CreatedDate = DateTime.Now,
                                    IsActive = true,
                                    Password = "123",
                                    RoleID = item.IsManager ? Convert.ToInt64(ReadConfig.GetValue("RolePM")) : Convert.ToInt64(ReadConfig.GetValue("RoleEmployee"))
                                };
                                DB.User.Add(user);
                                DB.SaveChanges();

                                refRole.UserID = user.UserID;
                                DB.Employee.Attach(refRole);
                                DB.Entry(refRole).State = System.Data.Entity.EntityState.Modified;
                                DB.SaveChanges();
                            }
                        }
                        else
                        {
                            ViewBag.Message = ViewBag.Message + $"line Employee { SCTimeSheet.Models.Common.GetName(refRole.EmpFirstName, refRole.EmpLastName, refRole.EmpMiddleName) } email is blank, please contact administrator";
                        }
                    }
                }
                TempData.Add("ProjectCreated", $"{model.ProjectName } is created successfully");
                return RedirectToAction("Index", "ProjectMain");




            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
                LogHelper.ErrorLog(ex);
            }
            return View("Index", model);
        }



        [HttpPost]
        public JsonResult GetProjectMember([DataSourceRequest]DataSourceRequest request, long? projectId)
        {
            try
            {
                // string projectid = Session["ProjectId"].ToString();
                if (projectId.HasValue)
                {
                    IEnumerable<EmployeeProjectListUI> GetDetails = DB.Database.SqlQuery<EmployeeProjectListUI>(
                           @"exec " + Constants.P_GetEmp_Project_Details + " @ProjectId",
                           new object[] {
                        new SqlParameter("@ProjectId",  projectId)
                           }).ToList().Distinct();
                    DataSourceResult result = GetDetails.ToDataSourceResult(request);
                    return Json(result);
                }
                else
                {
                    return Json(new List<EmployeeProjectListUI>());
                }

                //return Json(GetDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult UpdateProject(ProjectListEdit EPL)
        {
            try
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var modifiedOrNewEmpList = json_serializer.Deserialize<EmployeeProjectList[]>(EPL.EmployeeProjectList);

                StringBuilder errorMessage = new StringBuilder();

                if (EPL.Theme.HasValue)
                {
                    var eligibleThemeGrants = ConfigurationManager.AppSettings["ThemeGrandCodes"].ToString().Split(',').ToList();
                    var themeMstType = Convert.ToInt32(ConfigurationManager.AppSettings["TypeGrant"]);
                    var themes = DB.MasterData.Where(x => x.MstTypeID == themeMstType && eligibleThemeGrants.Contains(x.MstCode)).Select(x => x.MstID);
                    if (!themes.Contains(EPL.ProjectGrant))
                    {
                        EPL.Theme = null;
                    }
                }

                if (EPL.IsRDProject == 1)
                {
                    IEnumerable<EmployeeProjectList> GetDetails = DB.Database.SqlQuery<EmployeeProjectList>(
                              @"exec " + Constants.P_UpdateProjectList + " @ProjectName,@ProjectId,@ProjectCode,@StartDate,@EndDate,@InternalOrder,@ProjectGrant,@IsRDProject,@Theme,@ResearchArea,@TypeofResearch,@CostCenter,@ProjectDesc",
                              new object[] {
                                  new SqlParameter("@ProjectName",  EPL.ProjectName),
                        new SqlParameter("@ProjectId",  EPL.ProjectID),
                        new SqlParameter("@ProjectCode", EPL.ProjectCode),
                        new SqlParameter("@StartDate",  EPL.StartDate),
                        new SqlParameter("@EndDate",  EPL.EndDate),
                        new SqlParameter("@InternalOrder",  EPL.InternalOrder),
                        new SqlParameter("@ProjectGrant",  EPL.ProjectGrant),
                        new SqlParameter("@ResearchArea",  EPL.ResearchArea),
                        new SqlParameter("@TypeofResearch",  EPL.TypeofResearch),
                        new SqlParameter("@IsRDProject",EPL.IsRDProject),
                         new SqlParameter("@CostCenter",EPL.CostCentre),
                          new SqlParameter("@ProjectDesc",EPL.ProjectDesc ?? ""),
                          new SqlParameter("@Theme",(object)EPL.Theme ?? DBNull.Value),
                              }).ToList().Distinct();
                }
                else
                {
                    IEnumerable<EmployeeProjectList> GetDetails = DB.Database.SqlQuery<EmployeeProjectList>(
                              @"exec " + Constants.P_UpdateProjectList + " @ProjectName,@ProjectId,@ProjectCode,@StartDate,@EndDate,@InternalOrder,@ProjectGrant,@IsRDProject,@Theme,null,null,@CostCenter,@ProjectDesc",
                              new object[] {
                                  new SqlParameter("@ProjectName",  EPL.ProjectName),
                        new SqlParameter("@ProjectId",  EPL.ProjectID),
                        new SqlParameter("@ProjectCode", EPL.ProjectCode),
                        new SqlParameter("@StartDate",  EPL.StartDate),
                        new SqlParameter("@EndDate",  EPL.EndDate),
                        new SqlParameter("@InternalOrder",  EPL.InternalOrder),
                        new SqlParameter("@ProjectGrant",  EPL.ProjectGrant),
                        new SqlParameter("@IsRDProject", EPL.IsRDProject),
                        new SqlParameter("@Theme",(object)EPL.Theme ?? DBNull.Value),
                         new SqlParameter("@CostCenter",EPL.CostCentre),
                          new SqlParameter("@ProjectDesc",EPL.ProjectDesc ?? ""),


                              }).ToList().Distinct();
                }
                List<ProjectEmployeesModel> projectMembers = DB.ProjectEmployee.Where(x => x.ProjectID == EPL.ProjectID).ToList();

                foreach (ProjectEmployeesModel item in projectMembers)
                {
                    if (item.EndDate > EPL.EndDate)
                    {
                        item.EndDate = EPL.EndDate;
                        DB.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        DB.SaveChanges();
                    }
                }
                foreach (var item in modifiedOrNewEmpList)
                {
                    if (!DB.ProjectEmployee.Any(x => x.ProjectID == EPL.ProjectID && x.EmployeeID == item.EmployeeID))
                    {
                        EmployeeModel refRole = DB.Employee.FirstOrDefault(x => x.EmployeeID == item.EmployeeID);
                        List<EmployeeProjectList> GetDetails2 = DB.Database.SqlQuery<EmployeeProjectList>(
               @"exec " + Constants.P_InsertProjectEmployee + " @ProjectId,@EmployeeID,@CheckRole,@InvPercentage,@RefRole,@StartDate,@EndDate,@Grant,@IsActive,@CreatedBy",
               new object[] {
                        new SqlParameter("@ProjectId",   EPL.ProjectID),
                        new SqlParameter("@EmployeeID", item.EmployeeID),
                       new SqlParameter("@CheckRole",   item.IsManager),
                        new SqlParameter("@InvPercentage",item.InvPercentage),
                        new SqlParameter("@RefRole",EPL.IsRDProject == 1 ? (refRole.RoleID ?? 0) : 0),
                        new SqlParameter("@StartDate",  item.StartDate),
                        new SqlParameter("@EndDate",  item.EndDate),
                          new SqlParameter("@Grant", Convert.ToInt64(EPL.ProjectGrant)),
                          new SqlParameter("@IsActive",   1),
                          new SqlParameter("@CreatedBy",Session["EmployeeId"].ToString())
               }).ToList();

                        if (EPL.IsRDProject == 1 && refRole.RoleID == null)
                        {
                            ViewBag.Message = ViewBag.Message + $"line Employee { SCTimeSheet.Models.Common.GetName(refRole.EmpFirstName, refRole.EmpLastName, refRole.EmpMiddleName) } Role is blank, please modify manually or contact administrator";
                        }

                        if (!string.IsNullOrEmpty(refRole.Email))
                        {
                            if (!DB.User.Any(x => x.Email == refRole.Email))
                            {
                                UserModel user = new UserModel
                                {
                                    Email = refRole.Email,
                                    CreatedBy = 1,
                                    CreatedDate = DateTime.Now,
                                    IsActive = true,
                                    Password = "123",
                                    RoleID = item.IsManager ? Convert.ToInt64(ReadConfig.GetValue("RolePM")) : Convert.ToInt64(ReadConfig.GetValue("RoleEmployee"))
                                };
                                DB.User.Add(user);
                                DB.SaveChanges();

                                refRole.UserID = user.UserID;
                                DB.Employee.Attach(refRole);
                                DB.Entry(refRole).State = System.Data.Entity.EntityState.Modified;
                                DB.SaveChanges();
                            }
                        }
                        else
                        {
                            ViewBag.Message = ViewBag.Message + $"line Employee { SCTimeSheet.Models.Common.GetName(refRole.EmpFirstName, refRole.EmpLastName, refRole.EmpMiddleName) } email is blank, please contact administrator";
                        }


                    }
                    else
                    {
                        var emp = DB.ProjectEmployee.FirstOrDefault(x => x.ProjectID == EPL.ProjectID && x.EmployeeID == item.EmployeeID);
                        emp.InvPercentage = item.InvPercentage;
                        emp.StartDate = item.StartDate;
                        emp.EndDate = item.EndDate;
                        emp.CheckRole = item.IsManager;
                        emp.RefRole = item.EmpRole;
                        DB.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                        DB.SaveChanges();
                    }

                }
                // return RedirectToAction("Index", "ProjectMain");
                ViewBag.Message = ViewBag.Message + $"line {EPL.ProjectName } is updated successfully";

                GetDefaults();
                ViewBag.ProjectId = EPL.ProjectID;
                ViewBag.GrantID = EPL.ProjectGrant;

                ViewBag.ReasearchID = EPL.ResearchArea;
                ViewBag.TypeID = EPL.TypeofResearch;

                //EPL.ProjectMembers = string.Join(",", empPercentageStatus.Select(x => x.Key.Key));
                //EPL.ProjectMembersNames = string.Join(",", empPercentageStatus.Select(x => x.Key.Value));
                EPL.EmpSearchText = "";
                return View("ProjectEdit", EPL);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult checkInvolvementPercentage(long[] empIds, long projectId)
        {
            List<dynamic> currentPercentages = new List<dynamic>();
            foreach (var item in empIds)
            {
                List<int> totalInvolvement = DB.Employee.Where(x => x.EmployeeID == item).Select(x => x.TotalInvolvement ?? 100).ToList();
                decimal? currentInvolvement = (from x in DB.ProjectEmployee
                                               join y in DB.ProjectMaster on x.ProjectID equals y.ProjectID
                                               where x.EmployeeID == item && (x.EndDate >= DateTime.Now || !x.EndDate.HasValue) && x.ProjectID != projectId
                                               select x.InvPercentage).Sum();
                currentPercentages.Add(new { totalInvolvement, currentInvolvement, EmpId = item });
            }


            return Json(currentPercentages, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetEmployeeSearch([DataSourceRequest]DataSourceRequest request, string SearchText)
        {
            try
            {
                string searchtext = "%" + SearchText + "%";
                List<EmployeeSearch> GetDetails = DB.Database.SqlQuery<EmployeeSearch>(
                             @"exec " + Constants.P_GetEmployeeSearch + " @EmpName",
                             new object[] {
                        new SqlParameter("@EmpName", searchtext)
                             }).ToList();

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
        public ActionResult GetProjectDate(string projectid)
        {
            try
            {
                List<ProjectMasterModel> GetDetails = DB.Database.SqlQuery<ProjectMasterModel>(
                             @"exec " + Constants.P_GetProjectDate + " @ProjectId",
                             new object[] {
                        new SqlParameter("@ProjectId", projectid)
                             }).ToList();

                return Json(GetDetails, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }
        public bool GetThemeList(int grantId)
        {
            var eligibleThemeGrants = ConfigurationManager.AppSettings["ThemeGrandCodes"].ToString().Split(',').ToList();
            var themeMstType = Convert.ToInt32(ConfigurationManager.AppSettings["TypeGrant"]);
            var grant = DB.MasterData.FirstOrDefault(x => x.MstTypeID == themeMstType && x.MstID == grantId)?.MstCode;
            return grant != null && eligibleThemeGrants.Contains(grant);
        }

        public JsonResult GetEmpRoleList()
        {
            return Json(DropdownList.RoleList(), JsonRequestBehavior.AllowGet);
        }
    }
}