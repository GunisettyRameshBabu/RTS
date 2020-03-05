using SCTimeSheet_DAL;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace SCTimeSheet.Common
{
    public class DropdownList : EntityBase
    {

        public static IEnumerable<ProjectLookUp> GetProjectListByGrantId(long empId, long roleId, long grantId)
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                //IEnumerable<ProjectLookUp> GetDetails = DB.Database.SqlQuery<ProjectLookUp>(
                //             @"exec " + Constants.P_ProjectList_Employee + " @EmpID, @RoleID",
                //             new object[] {
                //  new SqlParameter("@EmpID", empId),
                //  new SqlParameter("@RoleID", roleId)
                //             }).ToList().Distinct();

                if (roleId == 1)
                {
                    IEnumerable<ProjectLookUp> GetDetails = (from y in DB.ProjectMaster
                                                             where y.ProjectGrant == grantId
                                                             select new ProjectLookUp { ProjectName = y.ProjectName, ProjectID = y.ProjectID }).Distinct().ToList();

                    return GetDetails;
                }
                else
                {
                    IEnumerable<ProjectLookUp> GetDetails = (from y in DB.ProjectMaster
                                                             join x in DB.ProjectEmployee on y.ProjectID equals x.ProjectID

                                                             where y.ProjectGrant == grantId && x.EmployeeID == empId
                                                             select new ProjectLookUp { ProjectName = y.ProjectName, ProjectID = y.ProjectID }).Distinct().ToList();

                    return GetDetails;
                }



            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }
        public static IEnumerable<ProjectLookUp> ProjectList(long empId, long roleId)
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                //IEnumerable<ProjectLookUp> GetDetails = DB.Database.SqlQuery<ProjectLookUp>(
                //             @"exec " + Constants.P_ProjectList_Employee + " @EmpID, @RoleID",
                //             new object[] {
                //  new SqlParameter("@EmpID", empId),
                //  new SqlParameter("@RoleID", roleId)
                //             }).ToList().Distinct();

                IEnumerable<ProjectLookUp> GetDetails = (from x in DB.ProjectMaster
                                                         select new ProjectLookUp() { ProjectID = x.ProjectID, ProjectName = x.ProjectName, IsActive = x.IsActive }).ToList();

                if (roleId != 1)
                {
                    GetDetails = (from x in GetDetails
                                  join y in DB.ProjectEmployee on x.ProjectID equals y.ProjectID
                                  where y.EmployeeID == empId && y.IsActive && x.IsActive
                                  select x).Distinct().ToList();
                }


                return GetDetails;

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        public static IEnumerable<ProjectLookUp> ProjectManagerPMS(long empId, long roleId)
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            IList<ProjectLookUp> result = new List<ProjectLookUp>();
            try
            {
                if (roleId == 1)
                {
                    //result = (from x in DB.ProjectEmployee
                    //          join y in DB.ProjectMaster on x.ProjectID equals y.ProjectID
                    //          where x.EmployeeID == empId
                    //          select new ProjectLookUp() { ProjectID = x.ProjectID, ProjectName = y.ProjectName }).ToList();

                    result = (from x in DB.ProjectMaster
                              where x.StartDate <= DateTime.Now
                              select new ProjectLookUp() { ProjectID = x.ProjectID, ProjectName = x.ProjectName }).ToList();
                }
                else
                {
                    result = (from x in DB.ProjectEmployee
                              join y in DB.ProjectMaster on x.ProjectID equals y.ProjectID
                              where x.EmployeeID == empId && x.CheckRole
                              select new ProjectLookUp() { ProjectID = x.ProjectID, ProjectName = y.ProjectName }).ToList();
                }


                return result;

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        public static IEnumerable<ProjectLookUpAdmin> ProjectListAdmin()
        {

            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {

                IEnumerable<ProjectLookUpAdmin> projectsadmin = DB.ProjectMaster.Select(x => new ProjectLookUpAdmin { ProjectID = x.ProjectID, ProjectName = x.ProjectCode + "-" + x.ProjectName });
                return projectsadmin;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        public static IEnumerable<SelectListItem> PreviousAndQuarterList(long empId, bool isNewEntry = false)
        {
            int month = DateTime.Now.Month;
            string currentQuarter = month <= 3 ? "Q1" : month > 3 && month <= 6 ? "Q2" : month > 6 && month <= 9 ? "Q3" : "Q4";
            List<SelectListItem> list = new List<SelectListItem>();
            DateTime month1 = Convert.ToDateTime(currentQuarter == "Q1" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + 10 + "/01") : currentQuarter == "Q3" ? DateTime.Now.ToString("yyyy/" + 04 + "/01") : currentQuarter == "Q2" ? DateTime.Now.ToString("yyyy/" + 01 + "/01") : DateTime.Now.ToString("yyyy/" + 07 + "/01"));
            DateTime month2 = Convert.ToDateTime(currentQuarter == "Q1" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + 11 + "/01") : currentQuarter == "Q3" ? DateTime.Now.ToString("yyyy/" + 05 + "/01") : currentQuarter == "Q2" ? DateTime.Now.ToString("yyyy/" + 02 + "/01") : DateTime.Now.ToString("yyyy/" + 08 + "/01"));
            DateTime month3 = Convert.ToDateTime(currentQuarter == "Q1" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + 12 + "/31") : currentQuarter == "Q3" ? DateTime.Now.ToString("yyyy/" + 06 + "/30") : currentQuarter == "Q2" ? DateTime.Now.ToString("yyyy/" + 03 + "/31") : DateTime.Now.ToString("yyyy/" + 09 + "/30"));
            DateTime month4 = Convert.ToDateTime(currentQuarter == "Q1" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + 12 + "/01") : currentQuarter == "Q3" ? DateTime.Now.ToString("yyyy/" + 06 + "/01") : currentQuarter == "Q2" ? DateTime.Now.ToString("yyyy/" + 03 + "/01") : DateTime.Now.ToString("yyyy/" + 09 + "/01"));
            using (ApplicationDBContext db = new ApplicationDBContext())
            {
                List<DateTime> involveMonths = new List<DateTime>() { month1, month2, month4 };
                //if (isNewEntry)
                //{
                List<TimeSheetLockModel> lockPeriods = db.TimeSheetLocks.Where(x => x.StartDate >= month1.Date && x.EndDate <= month3.Date).OrderByDescending(x => x.Id).ToList();
                List<ProjectEmployeesModel> allProjects = (from x in db.ProjectEmployee.Where(x => x.EmployeeID == empId && (x.StartDate <= month1.Date || (x.StartDate >= month1.Date && x.StartDate <= month3.Date)) && x.EndDate >= month3.Date && x.IsActive)
                                                           join y in db.ProjectMaster on x.ProjectID equals y.ProjectID
                                                           where y.IsActive
                                                           select x).ToList();

                long rejectStatus = Convert.ToInt64(ConfigurationManager.AppSettings["StatusRejected"]);
                List<long> exist = (from x in allProjects
                                    join y in db.EmpTimeSheet on new { EmpId = x.EmployeeID, x.ProjectID } equals new { y.EmpId, y.ProjectID }
                                    where involveMonths.Contains(y.InvolveMonth) && y.EmpId == empId && y.Status != rejectStatus
                                    select y.ProjectID).Distinct().ToList();
                // bool entryExist = db.NewEntry.Any(x => involveMonths.Contains(x.InvolveMonth) && x.EmpId == empId);
                bool entryExist = (allProjects.Count == exist.Count);
                if ((lockPeriods.Any() && !lockPeriods[0].Status) && (!entryExist || !isNewEntry))
                {
                    string preQuarter = month <= 3 ? "Q4" : month > 3 && month <= 6 ? "Q1" : month > 6 && month <= 9 ? "Q2" : "Q3";
                    list.Add(new SelectListItem() { Text = preQuarter + " " + (preQuarter == "Q4" ? DateTime.Now.Year - 1 : DateTime.Now.Year), Value = preQuarter });
                }
                else if (!lockPeriods.Any() && (!entryExist || !isNewEntry))
                {
                    string preQuarter = month <= 3 ? "Q4" : month > 3 && month <= 6 ? "Q1" : month > 6 && month <= 9 ? "Q2" : "Q3";
                    list.Add(new SelectListItem() { Text = preQuarter + " " + (preQuarter == "Q4" ? DateTime.Now.Year - 1 : DateTime.Now.Year), Value = preQuarter });
                }
                else if (!isNewEntry)
                {
                    string preQuarter = month <= 3 ? "Q4" : month > 3 && month <= 6 ? "Q1" : month > 6 && month <= 9 ? "Q2" : "Q3";
                    list.Add(new SelectListItem() { Text = preQuarter + " " + (preQuarter == "Q4" ? DateTime.Now.Year - 1 : DateTime.Now.Year), Value = preQuarter });
                }
                //}
                //else
                //{
                //    if (!db.TimeSheetLocks.Any(x => x.StartDate >= month1.Date && x.EndDate <= month3.Date && !x.Status))
                //    {
                //        var preQuarter = month <= 3 ? "Q4" : month > 3 && month <= 6 ? "Q1" : month > 6 && month <= 9 ? "Q2" : "Q3";
                //        list.Add(new SelectListItem() { Text = preQuarter, Value = preQuarter });
                //    }
                //}

            }


            list.Add(new SelectListItem() { Text = currentQuarter + " " + DateTime.Now.Year, Value = currentQuarter });
            return list;
        }

        public static IEnumerable<SelectListItem> PreviousAndQuarterListNewEntryOnBehalf(long empId)
        {
            int month = DateTime.Now.Month;
            string currentQuarter = month <= 3 ? "Q1" : month > 3 && month <= 6 ? "Q2" : month > 6 && month <= 9 ? "Q3" : "Q4";
            List<SelectListItem> list = new List<SelectListItem>();
            DateTime month1 = Convert.ToDateTime(currentQuarter == "Q1" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + 10 + "/01") : currentQuarter == "Q3" ? DateTime.Now.ToString("yyyy/" + 04 + "/01") : currentQuarter == "Q2" ? DateTime.Now.ToString("yyyy/" + 01 + "/01") : DateTime.Now.ToString("yyyy/" + 07 + "/01"));
            DateTime month2 = Convert.ToDateTime(currentQuarter == "Q1" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + 11 + "/01") : currentQuarter == "Q3" ? DateTime.Now.ToString("yyyy/" + 05 + "/01") : currentQuarter == "Q2" ? DateTime.Now.ToString("yyyy/" + 02 + "/01") : DateTime.Now.ToString("yyyy/" + 08 + "/01"));
            DateTime month3 = Convert.ToDateTime(currentQuarter == "Q1" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + 12 + "/31") : currentQuarter == "Q3" ? DateTime.Now.ToString("yyyy/" + 06 + "/30") : currentQuarter == "Q2" ? DateTime.Now.ToString("yyyy/" + 03 + "/31") : DateTime.Now.ToString("yyyy/" + 09 + "/30"));
            DateTime month4 = Convert.ToDateTime(currentQuarter == "Q1" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + 12 + "/01") : currentQuarter == "Q3" ? DateTime.Now.ToString("yyyy/" + 06 + "/01") : currentQuarter == "Q2" ? DateTime.Now.ToString("yyyy/" + 03 + "/01") : DateTime.Now.ToString("yyyy/" + 09 + "/01"));
            using (ApplicationDBContext db = new ApplicationDBContext())
            {
                List<DateTime> involveMonths = new List<DateTime>() { month1, month2, month4 };

                List<TimeSheetLockModel> lockPeriods = db.TimeSheetLocks.Where(x => x.StartDate >= month1.Date && x.EndDate <= month3.Date).OrderByDescending(x => x.Id).ToList();
                if (!lockPeriods.Any())
                {
                    string preQuarter = month <= 3 ? "Q4" : month > 3 && month <= 6 ? "Q1" : month > 6 && month <= 9 ? "Q2" : "Q3";
                    list.Add(new SelectListItem() { Text = preQuarter + " " + (preQuarter == "Q4" ? DateTime.Now.Year - 1 : DateTime.Now.Year), Value = preQuarter });
                }
                else if (lockPeriods.Any() && !lockPeriods[0].Status)
                {
                    string preQuarter = month <= 3 ? "Q4" : month > 3 && month <= 6 ? "Q1" : month > 6 && month <= 9 ? "Q2" : "Q3";
                    list.Add(new SelectListItem() { Text = preQuarter + " " + (preQuarter == "Q4" ? DateTime.Now.Year - 1 : DateTime.Now.Year), Value = preQuarter });
                }
            }


            list.Add(new SelectListItem() { Text = currentQuarter + " " + DateTime.Now.Year, Value = currentQuarter });
            return list;
        }


        #region Status List

        public static IEnumerable<StatusLookUp> StatusList()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                IEnumerable<StatusLookUp> status = DB.Status.Select(x => new StatusLookUp { StatusID = x.StatusID, StatusDesc = x.StatusDesc });
                return status;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        #endregion

        #region Employee List

        public static IEnumerable<EmployeeLookUp> EmployeeList()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                IEnumerable<EmployeeLookUp> projects = DB.Employee.AsEnumerable().Select(x => new EmployeeLookUp { EmployeeID = x.EmployeeID, EmpName = SCTimeSheet.Models.Common.GetName(x.EmpFirstName, x.EmpLastName, x.EmpMiddleName) }).ToList();
                return projects;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        #endregion

        #region Employee List based on the user role

        public static IEnumerable<EmployeeLookUp> EmployeeListViaRole(long PMid, long roleId)
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            List<EmployeeLookUp> result = new List<EmployeeLookUp>();
            try
            {

                if (roleId == 1)
                {
                    result = (from x in DB.Employee
                              where x.EmployeeID != PMid
                              select x).AsEnumerable().Select(x => new EmployeeLookUp() { EmployeeID = x.EmployeeID, EmpName = SCTimeSheet.Models.Common.GetName(x.EmpFirstName, x.EmpLastName, x.EmpMiddleName) }).Distinct().ToList();
                }
                else
                {
                    List<long> projects = (from x in DB.ProjectEmployee
                                           join y in DB.ProjectMaster on x.ProjectID equals y.ProjectID
                                           where x.EmployeeID == PMid && x.CheckRole
                                           select x.ProjectID).ToList();

                    result = (from x in DB.Employee
                              join y in DB.ProjectEmployee on x.EmployeeID equals y.EmployeeID
                              where x.EmployeeID != PMid && projects.Contains(y.ProjectID)
                              select x).AsEnumerable().Select(x => new EmployeeLookUp() { EmployeeID = x.EmployeeID, EmpName = SCTimeSheet.Models.Common.GetName(x.EmpFirstName, x.EmpLastName, x.EmpMiddleName) }).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }

            return result;
        }

        #endregion


        #region Grant List

        public static IEnumerable<MasterLookUp> GrantList(long? empId = null)
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                if (empId.HasValue)
                {
                    long grantId = Convert.ToInt64(ReadConfig.GetValue("TypeGrant"));
                    DateTime lastDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    IEnumerable<MasterLookUp> Grants = (from x in DB.MasterData
                                                        join z in DB.ProjectMaster on x.MstID equals z.ProjectGrant
                                                        join y in DB.ProjectEmployee on z.ProjectID equals y.ProjectID
                                                        where y.EmployeeID == empId.Value
                                                        select new MasterLookUp { MstID = x.MstID, MstCode = x.MstCode + "-" + x.MstName }).Distinct().ToList();
                    return Grants;
                }
                else
                {
                    IEnumerable<MasterLookUp> Grants = DB.MasterData.AsEnumerable().Where(x => x.MstTypeID == Convert.ToInt64(ReadConfig.GetValue("TypeGrant"))).Select(x => new MasterLookUp { MstID = x.MstID, MstCode = x.MstCode + "-" + x.MstName }).ToList();
                    return Grants;
                }

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        public static IEnumerable<MasterLookUp> GetGrantListByProjectId(long projectId, long? empId = null)
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                long typeGrant = Convert.ToInt64(ReadConfig.GetValue("TypeGrant"));
                if (empId.HasValue)
                {
                    IEnumerable<MasterLookUp> Grants = (from x in DB.ProjectMaster
                                                        join y in DB.MasterData on x.ProjectGrant equals y.MstID
                                                        join z in DB.ProjectEmployee on x.ProjectID equals z.ProjectID
                                                        where y.MstTypeID == typeGrant && z.EmployeeID == empId
                                                        && x.ProjectID == projectId && z.EndDate >= DateTime.Now
                                                        select new MasterLookUp { MstID = y.MstID, MstCode = y.MstCode + "-" + y.MstName }).Distinct().ToList();
                    return Grants;
                }
                else
                {
                    IEnumerable<MasterLookUp> Grants = (from x in DB.ProjectMaster
                                                        join y in DB.MasterData on x.ProjectGrant equals y.MstID
                                                        where y.MstTypeID == typeGrant
                                                        && x.ProjectID == projectId
                                                        select new MasterLookUp { MstID = y.MstID, MstCode = y.MstCode + "-" + y.MstName }).ToList();
                    return Grants;
                }

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        #endregion


        #region Role List

        public static IEnumerable<RoleLookup> RoleList()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                IEnumerable<RoleLookup> roles = DB.MasterData.AsEnumerable().Where(x => x.MstTypeID == Convert.ToInt64(ReadConfig.GetValue("TypeEmpRole"))).Select(x => new RoleLookup { RoleID = x.MstID, RoleName = x.MstCode }).ToList();

                //IEnumerable<RoleLookup> roles = DB.Role.Select(x => new RoleLookup { RoleID = x.RoleID, RoleName = x.RoleName}).ToList();
                return roles;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        #endregion

        #region Theme List
        public static IEnumerable<MasterLookUp> ThemeList()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                IEnumerable<MasterLookUp> roles = DB.MasterData.AsEnumerable().Where(x => x.MstTypeID == Convert.ToInt64(ReadConfig.GetValue("TypeTheme"))).Select(x => new MasterLookUp { MstID = x.MstID, MstCode = x.MstCode }).ToList();
                return roles;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region Report List

        public static IEnumerable<ReportLookup> ReportList()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                IEnumerable<ReportLookup> reportlist = DB.Report.AsEnumerable().Where(x => x.IsActive == true).Select(x => new ReportLookup { ReportID = x.ReportID, ReportName = x.ReportName }).ToList();
                return reportlist;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        #endregion

        #region Research List

        public static IEnumerable<ResearchLookup> ResearchList()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                IEnumerable<ResearchLookup> researchlist = DB.ResearchMaster.AsEnumerable().Where(x => x.IsActive == true).Select(x => new ResearchLookup { RsID = x.RsID, RsDesc = x.RsDesc }).ToList();
                return researchlist;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        #endregion

        #region ResearchType List

        public static IEnumerable<MasterLookUp> ResearchTypeList()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                IEnumerable<MasterLookUp> ResearchTypelist = DB.MasterData.AsEnumerable().Where(x => x.MstTypeID == Convert.ToInt64(ReadConfig.GetValue("TypeResearchType"))).Select(x => new MasterLookUp { MstID = x.MstID, MstCode = x.MstName }).ToList();
                return ResearchTypelist;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        #endregion


        #region CostCenter List

        public static IEnumerable<CostCenterLookup> CostCenterList()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            try
            {
                IEnumerable<CostCenterLookup> cost = DB.MasterData.AsEnumerable().Where(x => x.MstTypeID == Convert.ToInt64(ReadConfig.GetValue("TypeCostcenter"))).Select(x => new CostCenterLookup { CostID = x.MstID, CostName = x.MstCode }).ToList();

                //IEnumerable<RoleLookup> roles = DB.Role.Select(x => new RoleLookup { RoleID = x.RoleID, RoleName = x.RoleName}).ToList();
                return cost;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        #endregion




    }
}