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
        [NoCache]
        [Route("EmployeeEdit/{ProjectId}/{EmployeeID}")]
        public ActionResult EmployeeEdit(long ProjectId, long EmployeeID)
        {

            try
            {
                ProjectEmpList GetDetails = DB.Database.SqlQuery<ProjectEmpList>(
                           @"exec " + Constants.P_GetEmp_Project_Details_Edit + " @EmpId,@ProjectId",
                           new object[] {
                        new SqlParameter("@ProjectId",  ProjectId),
                         new SqlParameter("@EmpId",  EmployeeID)
                           }).ToList().FirstOrDefault();

                ViewBag.ProjectId = ProjectId;
                ViewBag.RoleList = DropdownList.RoleList();
                ViewBag.RoleID = GetDetails.RoleID;
                return View(GetDetails);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }

        }



        public class EmplID
        {
            public string EmployID { get; set; }
        }
        [HttpPost]
        [SubmitButton(Name = "action", Argument = "SaveAsDraft")]
        public ActionResult Save(ProjectMasterModelNew model)
        {
            try
            {
                GetDefaults();
                List<KeyValuePair<KeyValuePair<long, string>, KeyValuePair<decimal, int>>> empPercentageStatus = new List<KeyValuePair<KeyValuePair<long, string>, KeyValuePair<decimal, int>>>();
                if (!string.IsNullOrEmpty(model.ProjectMembers))
                {
                    string[] empList = model.ProjectMembers.Split(',');
                    string[] empNames = model.ProjectMembersNames.Split(',');
                    for (int i = 0; i < empList.Length; i++)
                    {
                        long empId = Convert.ToInt64(empList[i]);
                        string empName = empNames[i];
                        List<int> totalInvolvement = DB.Employee.Where(x => x.EmployeeID == empId).Select(x => x.TotalInvolvement ?? 100).ToList();
                        decimal? currentInvolvement = (from x in DB.ProjectEmployee
                                                       join y in DB.ProjectMaster on x.ProjectID equals y.ProjectID
                                                       where x.EmployeeID == empId && (x.EndDate >= DateTime.Now || !x.EndDate.HasValue) && x.ProjectID != model.ProjectID
                                                       select x.InvPercentage).Sum();

                        // = currentInvolvement + EPL.InvPercentage;
                        if (currentInvolvement + model.InvPercentage > totalInvolvement[0])
                        {
                            empPercentageStatus.Add(new KeyValuePair<KeyValuePair<long, string>, KeyValuePair<decimal, int>>(new KeyValuePair<long, string>(empId, empName), new KeyValuePair<decimal, int>(currentInvolvement.Value, totalInvolvement[0])));
                        }
                    }

                }

                StringBuilder errorMessage = new StringBuilder();
                if (empPercentageStatus.Any())
                {
                    foreach (KeyValuePair<KeyValuePair<long, string>, KeyValuePair<decimal, int>> item in empPercentageStatus)
                    {
                        errorMessage.Append($"Emplyee {item.Key.Value} has exceeded the maximum allocation , current allocation is {item.Value.Key} , maximum allocation is {item.Value.Value} and Available allocation is { item.Value.Value - item.Value.Key } line line");

                    }

                    ViewBag.Message = errorMessage.ToString();


                }
                else
                {
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

                    if (!string.IsNullOrEmpty(model.ProjectMembers))
                    {
                        model.ProjectID = projectId;

                        model.IsActive = true;

                        foreach (string item in model.ProjectMembers.Split(','))
                        {
                            EmployeeModel refRole = DB.Employee.FirstOrDefault(x => x.EmployeeID.ToString() == item);
                            List<EmployeeProjectList> GetDetails = DB.Database.SqlQuery<EmployeeProjectList>(
                     @"exec " + Constants.P_InsertProjectEmployee + " @ProjectId,@EmployeeID,@CheckRole,@InvPercentage,@RefRole,@StartDate,@EndDate,@Grant,@IsActive,@CreatedBy",
                     new object[] {
                        new SqlParameter("@ProjectId",   model.ProjectID),
                        new SqlParameter("@EmployeeID", item),
                       new SqlParameter("@CheckRole",   model.CheckRole),
                        new SqlParameter("@InvPercentage",  model.InvPercentage),
                        new SqlParameter("@RefRole",  model.IsRDProject == 1 ? (refRole.RoleID ?? 0) : 0),
                        new SqlParameter("@StartDate",   model.StartDate),
                          new SqlParameter("@EndDate",   model.EndDate),
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
                                        RoleID = model.CheckRole ? Convert.ToInt64(ReadConfig.GetValue("RolePM")) : Convert.ToInt64(ReadConfig.GetValue("RoleEmployee"))
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



            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
                LogHelper.ErrorLog(ex);
            }
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Edit()
        {
            try
            {
                int ProjectId = Convert.ToInt16(Session["ProjectId"]);
                ViewBag.GrantList = DropdownList.GrantList();
                ViewBag.ResearchList = DropdownList.ResearchList();
                ViewBag.ResearchTypeList = DropdownList.ResearchTypeList();

                ProjectMasterModel projectmasterlist = DB.ProjectMaster.Where(x => x.ProjectID == ProjectId).FirstOrDefault();

                Session["ProjectDetails"] = projectmasterlist;
                //return Json(new { data= projectmasterlist });
                return RedirectToAction("ProjectEdit", "ProjectMaster");


            }
            catch (Exception ex)
            {

                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult EmpEdit()
        {
            try
            {

                //return Json(new { data= projectmasterlist });
                return RedirectToAction("EmployeeEdit", "ProjectMaster");


            }
            catch (Exception ex)
            {

                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new { success = false });
                }

                ViewBag.GrantList = DropdownList.GrantList();
                ViewBag.ResearchList = DropdownList.ResearchList();
                ViewBag.ResearchTypeList = DropdownList.ResearchTypeList();
                ProjectMasterModel existingModel = DB.ProjectMaster.Where(x => x.ProjectID == id).FirstOrDefault();
                existingModel.IsActive = false;
                DB.ProjectMaster.Attach(existingModel);
                DB.Entry(existingModel).State = System.Data.Entity.EntityState.Modified;
                DB.SaveChanges();
                return Json(new { success = true });

            }
            catch (Exception ex)
            {

                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GetRedirect(string projectid)
        {
            try
            {
                Session["ProjectId"] = projectid;
                return Json(new { redirectUrl = "ProjectMaster/ProjectEdit" });
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetProjectMember([DataSourceRequest]DataSourceRequest request, long projectId)
        {
            try
            {
                // string projectid = Session["ProjectId"].ToString();

                IEnumerable<EmployeeProjectListUI> GetDetails = DB.Database.SqlQuery<EmployeeProjectListUI>(
                           @"exec " + Constants.P_GetEmp_Project_Details + " @ProjectId",
                           new object[] {
                        new SqlParameter("@ProjectId",  projectId)
                           }).ToList().Distinct();
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
        public ActionResult GetEditvalues(string EmployeeID)
        {
            try
            {
                Session["EmpId"] = EmployeeID;
                return Json(new { redirectUrl = "ProjectMaster/EmployeeEdit" });
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        [HttpPost]
        [SubmitButton(Name = "action", Argument = "UpdateEmployee")]
        public ActionResult UpdateEmployee(ProjectEmpList EEP)
        {
            try
            {
                if (EEP.StartDate == DateTime.MinValue || EEP.EndDate == DateTime.MinValue)
                {
                    ViewBag.RoleList = DropdownList.RoleList();
                    ViewBag.Message = "Please enter valid start date or end date";
                    return View("EmployeeEdit", EEP);
                }
                if (EEP.EndDate < EEP.StartDate)
                {
                    ViewBag.RoleList = DropdownList.RoleList();
                    ViewBag.Message = "End Date Can not be less than the start date";
                    return View("EmployeeEdit", EEP);
                }
                long empid = EEP.EmployeeID;
                DateTime startDate = Convert.ToDateTime(EEP.StartDate);
                DateTime endDate = Convert.ToDateTime(EEP.EndDate);
                DateTime currentDate = DateTime.Now;
                //long projectID = Convert.ToInt64(Session["ProjectId"]);
                List<int> totalInvolvement = DB.Employee.Where(x => x.EmployeeID == empid).Select(x => x.TotalInvolvement ?? 100).ToList();
                decimal? currentInvolvement = (from x in DB.ProjectEmployee
                                               join y in DB.ProjectMaster on x.ProjectID equals y.ProjectID
                                               where x.EmployeeID == empid && (x.EndDate >= DateTime.Now || !x.EndDate.HasValue) && x.ProjectID != EEP.ProjectID
                                               select x.InvPercentage).Sum();

                // currentInvolvement = currentInvolvement + EEP.InvPercentage;
                if (totalInvolvement[0] >= (currentInvolvement ?? 0))
                {
                    decimal? availableInvolvement = totalInvolvement[0] - (currentInvolvement ?? 0);
                    if (availableInvolvement >= EEP.InvPercentage)
                    {
                        // var projectId = Convert.ToInt64(Session["ProjectId"].ToString());
                        ProjectEmployeesModel emp = DB.ProjectEmployee.AsQueryable().FirstOrDefault(x => x.EmployeeID == EEP.EmployeeID && x.ProjectID == EEP.ProjectID);
                        if (emp != null)
                        {
                            emp.CheckRole = EEP.CheckRole;
                            emp.InvPercentage = EEP.InvPercentage;
                            emp.ModifiedBy = (long)Session[Constants.SessionEmpID];
                            emp.ModifiedDate = DateTime.Now;
                            emp.StartDate = EEP.StartDate;
                            emp.EndDate = EEP.EndDate;
                            emp.RefRole = EEP.IsRDProject == 1 ? EEP.RoleID : 0;

                            DB.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                            DB.SaveChanges();
                        }
                    }
                    else
                    {
                        ViewBag.RoleList = DropdownList.RoleList();
                        ViewBag.Message = "The maximum limit of involvement percentage is exceed. Please edit Percentage of Involvment field";
                        return View("EmployeeEdit", EEP);
                    }
                }
                else
                {
                    ViewBag.RoleList = DropdownList.RoleList();
                    ViewBag.Message = "Exceeds the Total Involvement Percentage Limit";
                    return View("EmployeeEdit", EEP);
                }
                return RedirectToAction("ProjectEdit", "ProjectMaster", new { id = EEP.ProjectID });
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
                List<KeyValuePair<KeyValuePair<long, string>, KeyValuePair<decimal, int>>> empPercentageStatus = new List<KeyValuePair<KeyValuePair<long, string>, KeyValuePair<decimal, int>>>();
                if (!string.IsNullOrEmpty(EPL.ProjectMembers))
                {
                    string[] empList = EPL.ProjectMembers.Split(',');
                    string[] empNames = EPL.ProjectMembersNames.Split(',');
                    for (int i = 0; i < empList.Length; i++)
                    {
                        long empId = Convert.ToInt64(empList[i]);
                        string empName = empNames[i];
                        List<int> totalInvolvement = DB.Employee.Where(x => x.EmployeeID == empId).Select(x => x.TotalInvolvement ?? 100).ToList();
                        decimal? currentInvolvement = (from x in DB.ProjectEmployee
                                                       join y in DB.ProjectMaster on x.ProjectID equals y.ProjectID
                                                       where x.EmployeeID == empId && (x.EndDate >= DateTime.Now || !x.EndDate.HasValue) && x.ProjectID != EPL.ProjectID
                                                       select x.InvPercentage).Sum();

                        // = currentInvolvement + EPL.InvPercentage;
                        if (currentInvolvement + EPL.InvPercentage > totalInvolvement[0])
                        {
                            empPercentageStatus.Add(new KeyValuePair<KeyValuePair<long, string>, KeyValuePair<decimal, int>>(new KeyValuePair<long, string>(empId, empName), new KeyValuePair<decimal, int>(currentInvolvement.Value, totalInvolvement[0])));
                        }
                    }

                }
                StringBuilder errorMessage = new StringBuilder();
                if (empPercentageStatus.Any())
                {
                    foreach (KeyValuePair<KeyValuePair<long, string>, KeyValuePair<decimal, int>> item in empPercentageStatus)
                    {
                        errorMessage.Append($"Emplyee {item.Key.Value} has exceeded the maximum allocation , current allocation is {item.Value.Key} , maximum allocation is {item.Value.Value} and Available allocation is { item.Value.Value - item.Value.Key } line line");

                    }

                    ViewBag.Message = errorMessage.ToString();


                }
                if (EPL.Theme.HasValue)
                {
                    var eligibleThemeGrants = ConfigurationManager.AppSettings["ThemeGrandCodes"].ToString().Split(',').ToList();
                    var themeMstType = Convert.ToInt32(ConfigurationManager.AppSettings["TypeGrant"]);
                    var themes = DB.MasterData.Where(x => x.MstTypeID == themeMstType && eligibleThemeGrants.Contains(x.MstCode)).Select( x=> x.MstID);
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


                // long projectID = Convert.ToInt64(Session["ProjectId"]);

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


                if (!string.IsNullOrEmpty(EPL.ProjectMembers))
                {
                    int i = 0;
                    string[] names = EPL.ProjectMembersNames.Split(',');
                    foreach (string item in EPL.ProjectMembers.Split(','))
                    {

                        long id = Convert.ToInt64(item);

                        if (!DB.ProjectEmployee.Any(x => x.ProjectID == EPL.ProjectID && x.EmployeeID == id) && !empPercentageStatus.Any(x => x.Key.Key == id))
                        {
                            EmployeeModel refRole = DB.Employee.FirstOrDefault(x => x.EmployeeID.ToString() == item);
                            List<EmployeeProjectList> GetDetails2 = DB.Database.SqlQuery<EmployeeProjectList>(
                   @"exec " + Constants.P_InsertProjectEmployee + " @ProjectId,@EmployeeID,@CheckRole,@InvPercentage,@RefRole,@StartDate,@EndDate,@Grant,@IsActive,@CreatedBy",
                   new object[] {
                        new SqlParameter("@ProjectId",   EPL.ProjectID),
                        new SqlParameter("@EmployeeID", item),
                       new SqlParameter("@CheckRole",   EPL.CheckRole),
                        new SqlParameter("@InvPercentage",EPL.InvPercentage),
                        new SqlParameter("@RefRole",EPL.IsRDProject == 1 ? (refRole.RoleID ?? 0) : 0),
                        new SqlParameter("@StartDate", EPL.MemberStartDate.HasValue ? EPL.MemberStartDate.Value : EPL.StartDate),
                        new SqlParameter("@EndDate",  EPL.MemberEndDate.HasValue ? EPL.MemberEndDate.Value : EPL.EndDate),
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
                                        RoleID = EPL.CheckRole ? Convert.ToInt64(ReadConfig.GetValue("RolePM")) : Convert.ToInt64(ReadConfig.GetValue("RoleEmployee"))
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
                            if (!empPercentageStatus.Any(x => x.Key.Key == id))
                            {
                                ViewBag.Message = ViewBag.Message + $"line { names[i] } already exist";
                            }

                        }
                        i++;

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
                EPL.ProjectMembers = "";
                EPL.ProjectMembersNames = "";
                EPL.EmpSearchText = "";
                EPL.CheckRole = false;
                EPL.InvPercentage = null;
                EPL.StartDate = DateTime.Now;
                EPL.EndDate = DateTime.Now;
                EPL.RefRole = 0;
                return View("ProjectEdit", EPL);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }


        [HttpPost]
        [SubmitButton(Name = "action", Argument = "SearchEmployee")]
        public ActionResult SearchEmployee(ProjectMasterModel PMM)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
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

        [HttpPost]
        public ActionResult GetEmployeeID(string[] Empid)
        {
            try
            {
                //ProjectEmployeesModel PM = new ProjectEmployeesModel();
                Session["EmpID"] = Empid;
                return Json("Success", JsonRequestBehavior.AllowGet);


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