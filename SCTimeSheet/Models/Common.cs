using SCTimeSheet_DAL;
using SCTimeSheet_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SCTimeSheet.Models
{
    public class Common  
    {
        public static string FirstCharToUpper(string s)
        {
            // Check for empty string.  
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.  
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        public static List<TestClass> GetProjectDetails(int projectId,bool showTimeSheet)
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            List<TestClass> lstResults = new List<TestClass>();
            if (!showTimeSheet)
            {
                var list = (from x in DB.ProjectEmployee
                            join y in DB.Employee
                            on x.EmployeeID equals y.EmployeeID
                            where x.ProjectID == projectId
                            select new
                            {
                                x.EmployeeID,
                                IsManager = x.CheckRole ? "YES" : "NO",
                                x.StartDate,
                                x.EndDate,
                                x.InvPercentage,
                                y.EmpFirstName,
                                y.EmpLastName,
                                y.EmpMiddleName,
                                x.ProjectID

                            }).ToList();
                foreach (var item in list)
                {
                    lstResults.Add(new TestClass()
                    {
                        ProjectID = item.ProjectID,
                        EmpFirstName = GetName(item.EmpFirstName,item.EmpLastName,item.EmpMiddleName),
                        EmployeeID = item.EmployeeID,
                        EndDate = item.EndDate.Value.ToString("dd MMM yyyy"),
                        StartDate = item.StartDate.Value.ToString("dd MMM yyyy"),
                        InvPercentage = item.InvPercentage.Value,
                        IsManager = item.IsManager,


                    });
                }

                return lstResults;
            }
            else {
                int i = 1;
                var GetDetails = (from x in DB.ProjectEmployee
                                  join y in DB.EmpTimeSheet on new { x.EmployeeID, x.ProjectID } equals new { EmployeeID = y.EmpId, y.ProjectID } into timeSheets
                                  from y in timeSheets.DefaultIfEmpty()
                                  join p in DB.ProjectMaster on x.ProjectID equals p.ProjectID
                                  join m in DB.MasterData on p.ProjectGrant equals m.MstID
                                  join e in DB.Employee on x.EmployeeID equals e.EmployeeID
                                  join a in DB.Employee on y.ApproveRejectUser equals a.EmployeeID into approvedList
                                  from a in approvedList.DefaultIfEmpty()
                                  where p.ProjectID == projectId

                                  select new { x, y, p, m, e, a }
                                  ).ToList();

                var emps = GetDetails.OrderBy(x => x.p.ProjectCode).Select(x => x.x.EmployeeID).Distinct();
               // List<MonthList2> lstResult = new List<MonthList2>();
                foreach (var item in emps)
                {

                    var lstProjects = GetDetails.Where(x => x.x.EmployeeID == item && x.y != null).ToList();
                    var lstYears = new List<YearModel>();
                    foreach (var tYear in lstProjects.GroupBy(x => x.y.InvolveMonth.Year).OrderByDescending(x => x.Key))
                    {
                        var yearModel = new YearModel();
                        yearModel.Year = tYear.Key;

                        foreach (var tTImeSheet in tYear.ToList())
                        {
                            switch (tTImeSheet.y.InvolveMonth.Month)
                            {
                                case 1:
                                    yearModel.Jan = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y); break;
                                case 2:
                                    yearModel.Feb = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 3:
                                    yearModel.Mar = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 4:
                                    yearModel.Apr = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 5:
                                    yearModel.May = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 6:
                                    yearModel.Jun = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 7:
                                    yearModel.Jul = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 8:
                                    yearModel.Aug = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 9:
                                    yearModel.Sep = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 10:
                                    yearModel.Oct = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 11:
                                    yearModel.Nov = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                                case 12:
                                    yearModel.Dec = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status, tTImeSheet.e, tTImeSheet.a, tTImeSheet.y);
                                    break;
                            }
                        }
                        lstYears.Add(yearModel);
                    }
                    var itm = lstProjects.FirstOrDefault(x => x.x.EmployeeID == item) ?? GetDetails.FirstOrDefault(x => x.x.EmployeeID == item);
                    if (!lstYears.Any())
                    {
                        lstYears.Add(new YearModel() { Year = DateTime.Now.Year });
                    }
                    lstResults.Add(new TestClass()
                    {
                        ProjectID = itm.p.ProjectID,
                        EmpFirstName = GetName(itm.e.EmpFirstName, itm.e.EmpLastName, itm.e.EmpMiddleName),
                        EmployeeID = itm.e.EmployeeID,
                        EndDate = itm.x.EndDate.Value.ToString("dd MMM yyyy"),
                        StartDate = itm.x.StartDate.Value.ToString("dd MMM yyyy"),
                        InvPercentage = itm.x.InvPercentage.Value,
                        IsManager = itm.x.CheckRole ? "YES" : "NO",
                        MonthLists = lstYears
                    });

                   // i++;
                }

                return lstResults;
            }
            //var DB = new ApplicationDBContext();
            
        }
        public static List<MonthList2> GetTimeSheet(long[] projectId, long empId)
        {
            var DB = new ApplicationDBContext();
            int i = 1;
            var GetDetails = (from x in DB.ProjectEmployee
                              join y in DB.EmpTimeSheet on new { x.EmployeeID, x.ProjectID } equals new { EmployeeID = y.EmpId, y.ProjectID } into timeSheets
                              from y in timeSheets.DefaultIfEmpty()
                              join p in DB.ProjectMaster on x.ProjectID equals p.ProjectID
                              join m in DB.MasterData on p.ProjectGrant equals m.MstID
                              join e in DB.Employee on x.EmployeeID equals e.EmployeeID
                              join a in DB.Employee on y.ApproveRejectUser equals a.EmployeeID into approvedList
                              from a in approvedList.DefaultIfEmpty()
                              where (projectId.Any() ? projectId.Contains(x.ProjectID) : true) && x.EmployeeID == empId

                              select new { x, y, p, m, e, a }
                              ).ToList();

            var projects = GetDetails.OrderBy(x => x.p.ProjectCode).Select(x => x.x.ProjectID).Distinct();
            List<MonthList2> lstResult = new List<MonthList2>();
            foreach (var item in projects)
            {

                var lstProjects = GetDetails.Where(x => x.x.ProjectID == item && x.y != null).ToList();
                var lstYears = new List<YearModel>();
                foreach (var tYear in lstProjects.GroupBy(x => x.y.InvolveMonth.Year).OrderByDescending(x => x.Key))
                {
                    var yearModel = new YearModel();
                    yearModel.Year = tYear.Key;

                    foreach (var tTImeSheet in tYear.ToList())
                    {
                        switch (tTImeSheet.y.InvolveMonth.Month)
                        {
                            case 1:
                                yearModel.Jan = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a,tTImeSheet.y); break;
                            case 2:
                                yearModel.Feb = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 3:
                                yearModel.Mar = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 4:
                                yearModel.Apr = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 5:
                                yearModel.May = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 6:
                                yearModel.Jun = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 7:
                                yearModel.Jul = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 8:
                                yearModel.Aug = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 9:
                                yearModel.Sep = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 10:
                                yearModel.Oct = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 11:
                                yearModel.Nov = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                            case 12:
                                yearModel.Dec = tTImeSheet.y.Status + "-" + tTImeSheet.y.DaysEditCount + "-" + getToolTip(tTImeSheet.y.Status,tTImeSheet.e,tTImeSheet.a, tTImeSheet.y);
                                break;
                        }
                    }
                    lstYears.Add(yearModel);
                }
                var itm = lstProjects.FirstOrDefault(x => x.x.ProjectID == item) ?? GetDetails.FirstOrDefault(x => x.x.ProjectID == item);
                if (!lstYears.Any())
                {
                    lstYears.Add(new YearModel() { Year = DateTime.Now.Year });
                }
                lstResult.Add(new MonthList2()
                {
                    ProjectCode = itm.p.ProjectCode,
                    ProjectName = itm.p.ProjectName,
                    ProjectGrant = itm.m.MstName,
                    InvPercentage = itm.x.InvPercentage.Value,
                    EmpName = GetName(itm.e.EmpFirstName, itm.e.EmpLastName, itm.e.EmpMiddleName),
                    ProjectStartDate = itm.p.StartDate.Value,
                    ProjectEndDate = itm.p.EndDate.Value,
                    ProjectManager = itm.x.CheckRole ? "YES" : "NO",
                    ProjectID = itm.x.ProjectID,
                    EmployeeID = empId,
                    TimeSheet = lstYears,
                    Id = i
                });
               
                i++;
            }

            return lstResult;
        }

        private  static string getToolTip(long status,EmployeeModel et,EmployeeModel at,NewEntryModel nm)
        {
            string toopTip = "";
            switch (status)
            {
                case 1:
                    toopTip = "<div>Time Sheet Drafted</div>";
                    break;
                case 2:
                    toopTip = @"<table class=""table table-border""><tr><td><b>Status : </b></td><td>Pending Approval</td></table>";
                    break;
                case 3:
                    toopTip = @"<table class=""table table-border""><tr><td><b>Status :</b></td><td>Approved</td></tr><tr><td><b>Approval Date :</b></td><td>" + (nm.ApproveRejectDate.HasValue ? nm.ApproveRejectDate.Value.ToString() : "") + @"</td></tr><tr><td><b>Approval By :</b></td><td>"+ (at == null ? "" : ((at.EmpFirstName ?? "") + " " + (at.EmpMiddleName ?? "") + " " +( at.EmpLastName ?? "")))+@"</td></tr><tr><td><b>Approval Comments :</b></td><td>"+ (nm.ApproveRejectComments ?? "Approved")+@"</td></tr></table>";
                    break;
                case 4:
                    toopTip = @"<table class=""table table-border""><tr><td><b>Status :</b></td><td>Rejected</td></tr><tr><td><b>Approval Date :</b></td><td>" + (nm.ApproveRejectDate.HasValue ? nm.ApproveRejectDate.Value.ToString() : "") + @"</td></tr><tr><td><b>Approval By :</b></td><td>" + (at == null ? "" : ((at.EmpFirstName ?? "") + " " + (at.EmpMiddleName ?? "") + " " + (at.EmpLastName ?? ""))) + @"</td></tr><tr><td><b>Approval Comments :</b></td><td>" + (nm.ApproveRejectComments ?? "Rejected") + @"</td></tr></table>";
                    break;
                default:
                    break;
            }
            return toopTip;
        }

        public static string GetName(string firstName,string lastName ,string middleName)
        {
            var result = (string.IsNullOrEmpty(lastName) ? "" : (lastName + (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(middleName) ? " " : ""))) + (string.IsNullOrEmpty(firstName) ? "" : (firstName + (!string.IsNullOrEmpty(middleName) ? " " : ""))) + (string.IsNullOrEmpty(middleName) ? "" : middleName); 
            return result;
        }

        public static bool CheckDateRange(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck.InRange(startDate, endDate);
        }

        public static bool CheckIfNotMonthDateDate(DateTime month1, DateTime month2 , DateTime month3, DateTime dateTime)
        {
            return month1 == dateTime || month2 == dateTime || month3 == dateTime;
        }

        public static DateTime[] DatesOfQuarter(DateTime dtmValue)
        {
            var dtmReturn = new DateTime[2];

            var intQuarter = (int)Math.Ceiling(dtmValue.Month / 3M);

            var intLastMonthOfQuarter = intQuarter * 3;
            var intFirstMonthOfQuarter = intLastMonthOfQuarter - 2;
            var intLastDayOfQuarter = DateTime.DaysInMonth(dtmValue.Year, intLastMonthOfQuarter);

            dtmReturn[0] = new DateTime(dtmValue.Year, intFirstMonthOfQuarter, 1);
            dtmReturn[1] = new DateTime(dtmValue.Year, intLastMonthOfQuarter, intLastDayOfQuarter);

            return dtmReturn;
        }

        public static bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }

    public static class DateTimeExtensions
    {
        public static bool InRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }
    }
}