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
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class NewEntryOnBehalfOneController : BaseController
    {
        private readonly string[] months = new string[] { "Month1", "Month2", "Month3" };
        // GET: NewEntryOnBehalfOne
        public ActionResult Index(string quarter = null)

        {
            //ViewBag.EmployeeList = DropdownList.EmployeeListViaRole((long)Session[Constants.SessionEmpID], (long)(Session[Constants.SessionRoleID]));
            // ViewBag.ProjectList = DropdownList.ProjectList((long)Session[Constants.SessionEmpID], (long)(Session[Constants.SessionRoleID]));
            ViewBag.Quadrent = quarter ?? GetQuardrent();
            long empId = (long)Session[Constants.SessionEmpID];
            ViewBag.QuarterList = DropdownList.PreviousAndQuarterListNewEntryOnBehalf(empId);

            NewEntryByProjectSelection model = new NewEntryByProjectSelection
            {
                Quarter = ViewBag.Quadrent,
                Month1 = "Jan-2019"
            };
            return View(model);
        }
        public string GetQuarter()
        {
            try
            {
                string currentquart = "Q1";

                var list = DB.Quarter.AsEnumerable().Where(x => x.Month == DateTime.Now.ToString("MM")).Select(x => new { x.Quarter }).FirstOrDefault();
                if (list != null)
                {
                    currentquart = list.Quarter;
                }

                return currentquart;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }

        }

        //Drop down selection results
        [HttpPost]
        public ActionResult GetProjectList([DataSourceRequest]DataSourceRequest request, NewEntryByProjectSelection MODEL, int projectId, string quarter = null)
        {
            ViewBag.Quadrent = GetQuardrent();
            quarter = quarter ?? GetQuardrent();
            ViewBag.CurrentMonth = DateTime.Now.Month;

            try
            {
                if (projectId != 0)
                {
                    
                    ViewBag.Quadrent = quarter;
                    string month1 = (quarter == "Q4") ? DateTime.Now.AddYears(-1).ToString("yyyy-" + (quarter == "Q4" ? "10" : quarter == "Q3" ? "07" : quarter == "Q2" ? "04" : "01") + "-01") : DateTime.Now.ToString("yyyy-" + (quarter == "Q4" ? "10" : quarter == "Q3" ? "07" : quarter == "Q2" ? "04" : "01") + "-01");
                    string month2 = (quarter == "Q4") ? DateTime.Now.AddYears(-1).ToString("yyyy-" + (quarter == "Q4" ? "11" : quarter == "Q3" ? "08" : quarter == "Q2" ? "05" : "02") + "-01") : DateTime.Now.ToString("yyyy-" + (quarter == "Q4" ? "11" : quarter == "Q3" ? "08" : quarter == "Q2" ? "05" : "02") + "-01");
                    string month3 = (quarter == "Q4") ? DateTime.Now.AddYears(-1).ToString("yyyy-" + (quarter == "Q4" ? "12" : quarter == "Q3" ? "09" : quarter == "Q2" ? "06" : "03") + "-01") : DateTime.Now.ToString("yyyy-" + (quarter == "Q4" ? "12" : quarter == "Q3" ? "09" : quarter == "Q2" ? "06" : "03") + "-01");
                    List<NewEntryByProjectSelection> GetDetails = DB.Database.SqlQuery<NewEntryByProjectSelection>(
                              @"exec " + Constants.P_GetNewEntryOnBehalfByProjectSelection + " @ProID,@empId,@month1,@month2,@month3",
                              new object[] {
                                  new SqlParameter("@ProID",projectId),
                                   new SqlParameter("@empId",  Session[Constants.SessionEmpID]),
                                   new SqlParameter("@month1",  month1),
                        new SqlParameter("@month2",  month2),
                        new SqlParameter("@month3",  month3)
                              }).Distinct().ToList();
                    ViewBag.ddlType = "Projectdll";
                    DateTime origDT1 = Convert.ToDateTime(month1);
                    DateTime lastDate1 = new DateTime(origDT1.Year, origDT1.Month, 1).AddMonths(1).AddDays(-1);
                    DateTime origDT2 = Convert.ToDateTime(month2);
                    DateTime lastDate2 = new DateTime(origDT2.Year, origDT2.Month, 1).AddMonths(1).AddDays(-1);
                    DateTime origDT3 = Convert.ToDateTime(month3);
                    DateTime lastDate3 = new DateTime(origDT3.Year, origDT3.Month, 1).AddMonths(1).AddDays(-1);
                    foreach (NewEntryByProjectSelection item in GetDetails)
                    {
                        item.InvolvementEditDays1 = item.InvolvementEditDays1 == 0 && item.InvolvementDays1 != 0 && item.IsEdit1 ? item.InvolvementDays1 : item.InvolvementEditDays1;
                        item.InvolvementEditDays2 = item.InvolvementEditDays2 == 0 && item.InvolvementDays2 != 0 && item.IsEdit2 ? item.InvolvementDays2 : item.InvolvementEditDays2;
                        item.InvolvementEditDays3 = item.InvolvementEditDays3 == 0 && item.InvolvementDays3 != 0 && item.IsEdit3 ? item.InvolvementDays3 : item.InvolvementEditDays3;
                        item.OldInvolvementEditDays1 = item.InvolvementEditDays1;
                        item.OldInvolvementEditDays2 = item.InvolvementEditDays2;
                        item.OldInvolvementEditDays3 = item.InvolvementEditDays3;

                        if (GetDetails.Any(x => !x.ShowWarning))
                        {
                            if (Models.Common.CheckDateRange(item.StartDate, Convert.ToDateTime(month1), lastDate3))
                            {
                                if (!Models.Common.CheckIfNotMonthDateDate(Convert.ToDateTime(month1), Convert.ToDateTime(month2), Convert.ToDateTime(month3), item.StartDate) || !Models.Common.CheckIfNotMonthDateDate(lastDate1, lastDate2, lastDate3, item.EndDate))
                                {
                                    item.ShowWarning = true;
                                }
                            }

                        }
                    }

                    
                    DataSourceResult result = GetDetails.ToDataSourceResult(request);
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
            return View("Index", MODEL);
        }

        [HttpPost]
        public ActionResult GetEmpList([DataSourceRequest]DataSourceRequest request, NewEntryByProjectSelection MODEL, int empId, string quarter = null)
        {
            //ViewBag.EmployeeList = DropdownList.EmployeeListViaRole((long)Session[Constants.SessionEmpID], (long)(Session[Constants.SessionRoleID]));
            //ViewBag.ProjectList = DropdownList.ProjectList((long)Session[Constants.SessionEmpID], (long)(Session[Constants.SessionRoleID]));
            ViewBag.Quadrent = GetQuardrent();
            quarter = quarter ?? GetQuardrent();
            try
            {
                if (empId != 0)
                {
                    long _pmId = (long)Session[Constants.SessionEmpID];
                    ViewBag.Quadrent = quarter;
                    string month1 = (quarter == "Q4" ) ? DateTime.Now.AddYears(-1).ToString("yyyy-" + (quarter == "Q4" ? "10" : quarter == "Q3" ? "07" : quarter == "Q2" ? "04" : "01") + "-01") : DateTime.Now.ToString("yyyy-" + (quarter == "Q4" ? "10" : quarter == "Q3" ? "07" : quarter == "Q2" ? "04" : "01") + "-01");
                    string month2 = (quarter == "Q4" ) ? DateTime.Now.AddYears(-1).ToString("yyyy-" + (quarter == "Q4" ? "11" : quarter == "Q3" ? "08" : quarter == "Q2" ? "05" : "02") + "-01") : DateTime.Now.ToString("yyyy-" + (quarter == "Q4" ? "11" : quarter == "Q3" ? "08" : quarter == "Q2" ? "05" : "02") + "-01");
                    string month3 = (quarter == "Q4" ) ? DateTime.Now.AddYears(-1).ToString("yyyy-" + (quarter == "Q4" ? "12" : quarter == "Q3" ? "09" : quarter == "Q2" ? "06" : "03") + "-01") : DateTime.Now.ToString("yyyy-" + (quarter == "Q4" ? "12" : quarter == "Q3" ? "09" : quarter == "Q2" ? "06" : "03") + "-01");

                    List<NewEntryByProjectSelection> GetDetails = DB.Database.SqlQuery<NewEntryByProjectSelection>(
                             @"exec " + Constants.P_GetNewEntryOnBehalfByEmployeeSelection + " @EmpID,@PmID,@month1,@month2,@month3",
                             new object[] {
                                new SqlParameter("@EmpID", empId),
                                new SqlParameter("@PmID", _pmId),
                                new SqlParameter("@month1",  month1),
                        new SqlParameter("@month2",  month2),
                        new SqlParameter("@month3",  month3)
                             }).ToList();

                    if (GetDetails.Any() && Convert.ToInt64(Session[Constants.SessionRoleID]) != 1)
                    {
                        List<long> projects = DropdownList.ProjectManagerPMS(_pmId, Convert.ToInt64(Session[Constants.SessionRoleID])).Select(x => x.ProjectID).ToList();
                        GetDetails = GetDetails.Where(x => projects.Contains(x.ProjectID)).ToList();

                    }
                    ViewBag.ddlType = "Empdll";
                    DateTime origDT1 = Convert.ToDateTime(month1);
                    DateTime lastDate1 = new DateTime(origDT1.Year, origDT1.Month, 1).AddMonths(1).AddDays(-1);
                    DateTime origDT2 = Convert.ToDateTime(month2);
                    DateTime lastDate2 = new DateTime(origDT2.Year, origDT2.Month, 1).AddMonths(1).AddDays(-1);
                    DateTime origDT3 = Convert.ToDateTime(month3);
                    DateTime lastDate3 = new DateTime(origDT3.Year, origDT3.Month, 1).AddMonths(1).AddDays(-1);
                    foreach (NewEntryByProjectSelection item in GetDetails)
                    {
                        item.InvolvementEditDays1 = item.InvolvementEditDays1 == 0 && item.InvolvementDays1 != 0 && item.IsEdit1 ? item.InvolvementDays1 : item.InvolvementEditDays1;
                        item.InvolvementEditDays2 = item.InvolvementEditDays2 == 0 && item.InvolvementDays2 != 0 && item.IsEdit2 ? item.InvolvementDays2 : item.InvolvementEditDays2;
                        item.InvolvementEditDays3 = item.InvolvementEditDays3 == 0 && item.InvolvementDays3 != 0 && item.IsEdit3 ? item.InvolvementDays3 : item.InvolvementEditDays3;
                        item.OldInvolvementEditDays1 = item.InvolvementEditDays1;
                        item.OldInvolvementEditDays2 = item.InvolvementEditDays2;
                        item.OldInvolvementEditDays3 = item.InvolvementEditDays3;

                        if (GetDetails.Any(x => !x.ShowWarning))
                        {
                            if (Models.Common.CheckDateRange(item.StartDate, Convert.ToDateTime(month1), lastDate3))
                            {
                                if (!Models.Common.CheckIfNotMonthDateDate(Convert.ToDateTime(month1), Convert.ToDateTime(month2), Convert.ToDateTime(month3), item.StartDate) || !Models.Common.CheckIfNotMonthDateDate(lastDate1, lastDate2, lastDate3, item.EndDate))
                                {
                                    item.ShowWarning = true;
                                }
                            }

                        }
                    }
                    DataSourceResult result = GetDetails.ToDataSourceResult(request);
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
            return View("Index", MODEL);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Editing_Update(DataSourceRequest request, NewEntryByProjectSelection[] MODEL)
        {
            ViewBag.ProjectList = DropdownList.ProjectList((long)Session[Constants.SessionEmpID], (long)(Session[Constants.SessionRoleID]));
            ViewBag.EmployeeList = DropdownList.EmployeeList();
            ViewBag.Quadrent = GetQuardrent();
            if (MODEL != null)
            {
                try
                {
                    long _entryId = (long)Session[Constants.SessionEmpID];
                    string quarter = MODEL.Any() ? MODEL[0].Quarter : GetQuarter();
                    var listMonth = (from Quart in DB.Quarter where Quart.Quarter == quarter select new { Quart.Month }).ToList();
                    string month1 = DateTime.Now.ToString("yyyy/" + listMonth[0].Month + "/01");
                    string month2 = DateTime.Now.ToString("yyyy/" + listMonth[1].Month + "/01");
                    string month3 = DateTime.Now.ToString("yyyy/" + listMonth[2].Month + "/01");
                    int currentmonth = GetQuarter() == quarter ? DateTime.Now.Month : Convert.ToInt32(listMonth[2].Month);
                    List<KeyValuePair<string,string>> emailStatus = new List<KeyValuePair<string,string>>();
                    if (MODEL != null && ModelState.IsValid)
                    {
                        foreach (NewEntryByProjectSelection product in MODEL)
                        {
                            int _currentNumber = 0;
                            IQueryable<int> items = DB.EmpTimeSheet.OrderByDescending(u => u.SequenceNo).Take(1).Select(e => e.SequenceNo);
                            foreach (int ir in items)
                            {
                                _currentNumber = ir;
                            }
                            if (_currentNumber == 0) { _currentNumber = 100; }
                            else { _currentNumber++; }
                            if (currentmonth == Convert.ToInt32(listMonth[0].Month))
                            {
                                NewEntryModel newEntryModel = new NewEntryModel();
                                newEntryModel.InvolveMonth = Convert.ToDateTime(month1);
                                newEntryModel.DaysCount = Convert.ToDecimal(product.InvolvementDays1);
                                newEntryModel.DaysEditCount = Convert.ToDecimal(product.InvolvementEditDays1);
                                newEntryModel.InvolvePercent = CalculateInvolvementPercentage(product.EmployeeID, product.InvolvementEditDays1, newEntryModel);
                                NewEntryModel exists = DB.EmpTimeSheet.FirstOrDefault(x => x.InvolveMonth == newEntryModel.InvolveMonth && x.EmpId == product.EmployeeID && x.ProjectID == product.ProjectID);
                                
                                if (exists != null)
                                {
                                    newEntryModel.TsID = exists.TsID;
                                    newEntryModel.RefNo = exists.RefNo;
                                    newEntryModel.SequenceNo = exists.SequenceNo;
                                    newEntryModel.EntryBy = (long)Session[Constants.SessionEmpID]; //empid
                                    newEntryModel.EntryDate = DateTime.Now;
                                    newEntryModel.Quart = quarter;
                                    newEntryModel.EntryRole = (long)Session[Constants.SessionRoleID];
                                    newEntryModel.EmpId = product.EmployeeID; //empid
                                    newEntryModel.ProjectID = product.ProjectID;
                                    newEntryModel.EmpRemarks = "";
                                    newEntryModel.ApproveRejectComments = "";
                                    newEntryModel.ApproveRejectStatus = "A";
                                    newEntryModel.ApproveRejectUser = (long)Session[Constants.SessionEmpID];
                                    newEntryModel.ApproveRejectDate = DateTime.Now;
                                    newEntryModel.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));
                                    DB.Entry(exists).CurrentValues.SetValues(newEntryModel);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    
                                    if (newEntryModel.DaysEditCount != 0 || product.IsEdit1)

                                    {

                                        newEntryModel.RefNo = AutoGen.GetReferenceNumber();
                                        newEntryModel.SequenceNo = _currentNumber;
                                        newEntryModel.EntryBy = _entryId; //empid
                                        newEntryModel.EntryDate = DateTime.Now;
                                        newEntryModel.EntryRole = (long)Session[Constants.SessionRoleID];
                                        newEntryModel.EmpId = product.EmployeeID; //empid
                                        newEntryModel.ProjectID = product.ProjectID;
                                        newEntryModel.Quart = quarter;
                                        newEntryModel.EmpRemarks = "";
                                        newEntryModel.ApproveRejectComments = "";
                                        newEntryModel.ApproveRejectStatus = "A";
                                        newEntryModel.ApproveRejectUser = (long)Session[Constants.SessionEmpID];
                                        newEntryModel.ApproveRejectDate = DateTime.Now;
                                        newEntryModel.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));
                                        DB.EmpTimeSheet.Add(newEntryModel);
                                        DB.SaveChanges();


                                    }
                                }
                            }
                            if (currentmonth == Convert.ToInt32(listMonth[1].Month))
                            {
                                foreach (string month in months)
                                {
                                    NewEntryModel newEntryModel = new NewEntryModel();
                                    bool isEdit = false;
                                    if (month == months[0])
                                    {
                                        newEntryModel.InvolveMonth = Convert.ToDateTime(month1);
                                        newEntryModel.DaysCount = Convert.ToDecimal(product.InvolvementDays1);
                                        newEntryModel.DaysEditCount = Convert.ToDecimal(product.InvolvementEditDays1);
                                        newEntryModel.InvolvePercent = CalculateInvolvementPercentage(product.EmployeeID, product.InvolvementEditDays1, newEntryModel);
                                        isEdit = product.IsEdit1;
                                    }
                                    if (month == months[1])
                                    {
                                        newEntryModel.InvolveMonth = Convert.ToDateTime(month2);
                                        newEntryModel.DaysCount = Convert.ToDecimal(product.InvolvementDays2);
                                        newEntryModel.DaysEditCount = Convert.ToDecimal(product.InvolvementEditDays2);
                                        newEntryModel.InvolvePercent = CalculateInvolvementPercentage(product.EmployeeID, product.InvolvementEditDays2, newEntryModel);
                                        isEdit = product.IsEdit2;
                                    }
                                    if (newEntryModel.DaysEditCount != 0 || isEdit)
                                    {
                                        NewEntryModel exists = DB.EmpTimeSheet.FirstOrDefault(x => x.InvolveMonth == newEntryModel.InvolveMonth && x.EmpId == product.EmployeeID && x.ProjectID == product.ProjectID);
                                        if (ModelState.IsValid)
                                        {
                                            if (exists != null)
                                            {
                                                newEntryModel.TsID = exists.TsID;
                                                newEntryModel.RefNo = exists.RefNo;
                                                newEntryModel.SequenceNo = exists.SequenceNo;
                                                newEntryModel.EntryBy = (long)Session[Constants.SessionEmpID]; //empid
                                                newEntryModel.EntryDate = DateTime.Now;
                                                newEntryModel.Quart = quarter;
                                                newEntryModel.EntryRole = (long)Session[Constants.SessionRoleID];
                                                newEntryModel.EmpId = product.EmployeeID; //empid
                                                newEntryModel.ProjectID = product.ProjectID;
                                                newEntryModel.EmpRemarks = "";
                                                newEntryModel.ApproveRejectComments = "";
                                                newEntryModel.ApproveRejectStatus = "A";
                                                newEntryModel.ApproveRejectUser = (long)Session[Constants.SessionEmpID];
                                                newEntryModel.ApproveRejectDate = DateTime.Now;
                                                newEntryModel.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));
                                                DB.Entry(exists).CurrentValues.SetValues(newEntryModel);
                                                DB.SaveChanges();
                                            }
                                            else
                                            {
                                                newEntryModel.RefNo = AutoGen.GetReferenceNumber();
                                                newEntryModel.SequenceNo = _currentNumber;
                                                newEntryModel.EntryBy = _entryId; //empid
                                                newEntryModel.EntryDate = DateTime.Now;
                                                newEntryModel.EntryRole = (long)Session[Constants.SessionRoleID];
                                                newEntryModel.EmpId = product.EmployeeID; //empid
                                                newEntryModel.ProjectID = product.ProjectID;
                                                newEntryModel.Quart = quarter;
                                                newEntryModel.EmpRemarks = "";
                                                newEntryModel.ApproveRejectComments = "";
                                                newEntryModel.ApproveRejectStatus = "A";
                                                newEntryModel.ApproveRejectUser = (long)Session[Constants.SessionEmpID];
                                                newEntryModel.ApproveRejectDate = DateTime.Now;
                                                newEntryModel.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));

                                                DB.EmpTimeSheet.Add(newEntryModel);
                                                DB.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            TempData["Error"] = ResourceMessage.DayscountExceed;
                                            //return View("Index", BindData(model.QID));
                                        }

                                    }
                                }
                            }
                            if (currentmonth == Convert.ToInt32(listMonth[2].Month))
                            {
                                foreach (string month in months)
                                {
                                    NewEntryModel newEntryModel = new NewEntryModel();
                                    bool isEdit = false;
                                    if (month == months[0])
                                    {
                                        newEntryModel.InvolveMonth = Convert.ToDateTime(month1);
                                        newEntryModel.DaysCount = Convert.ToDecimal(product.InvolvementDays1);
                                        newEntryModel.DaysEditCount = Convert.ToDecimal(product.InvolvementEditDays1);
                                        newEntryModel.InvolvePercent = CalculateInvolvementPercentage(product.EmployeeID, product.InvolvementEditDays1, newEntryModel);
                                        isEdit = product.IsEdit1;
                                    }
                                    if (month == months[1])
                                    {
                                        newEntryModel.InvolveMonth = Convert.ToDateTime(month2);
                                        newEntryModel.DaysCount = Convert.ToDecimal(product.InvolvementDays2);
                                        newEntryModel.DaysEditCount = Convert.ToDecimal(product.InvolvementEditDays2);
                                        newEntryModel.InvolvePercent = CalculateInvolvementPercentage(product.EmployeeID, product.InvolvementEditDays2, newEntryModel);
                                        isEdit = product.IsEdit2;
                                    }
                                    if (month == months[2])
                                    {
                                        newEntryModel.InvolveMonth = Convert.ToDateTime(month3);
                                        newEntryModel.DaysCount = Convert.ToDecimal(product.InvolvementDays3);
                                        newEntryModel.DaysEditCount = Convert.ToDecimal(product.InvolvementEditDays3);
                                        newEntryModel.InvolvePercent = CalculateInvolvementPercentage(product.EmployeeID, product.InvolvementEditDays3, newEntryModel);
                                        isEdit = product.IsEdit3;
                                    }

                                    if (newEntryModel.DaysEditCount != 0 || isEdit)
                                    {
                                        NewEntryModel exists = DB.EmpTimeSheet.FirstOrDefault(x => x.InvolveMonth == newEntryModel.InvolveMonth && x.EmpId == product.EmployeeID && x.ProjectID == product.ProjectID);
                                        if (ModelState.IsValid)
                                        {
                                            if (exists != null)
                                            {
                                                newEntryModel.TsID = exists.TsID;
                                                newEntryModel.RefNo = exists.RefNo;
                                                newEntryModel.SequenceNo = exists.SequenceNo;
                                                newEntryModel.EntryBy = (long)Session[Constants.SessionEmpID]; //empid
                                                newEntryModel.EntryDate = DateTime.Now;
                                                newEntryModel.Quart = quarter;
                                                newEntryModel.EntryRole = (long)Session[Constants.SessionRoleID];
                                                newEntryModel.EmpId = product.EmployeeID; //empid
                                                newEntryModel.ProjectID = product.ProjectID;
                                                newEntryModel.EmpRemarks = "";
                                                newEntryModel.ApproveRejectComments = "";
                                                newEntryModel.ApproveRejectStatus = "A";
                                                newEntryModel.ApproveRejectUser = (long)Session[Constants.SessionEmpID];
                                                newEntryModel.ApproveRejectDate = DateTime.Now;
                                                newEntryModel.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));
                                                DB.Entry(exists).CurrentValues.SetValues(newEntryModel);
                                                DB.SaveChanges();
                                            }
                                            else
                                            {
                                                newEntryModel.RefNo = AutoGen.GetReferenceNumber();
                                                newEntryModel.SequenceNo = _currentNumber;
                                                newEntryModel.EntryBy = _entryId; //empid
                                                newEntryModel.EntryDate = DateTime.Now;
                                                newEntryModel.EntryRole = (long)Session[Constants.SessionRoleID];
                                                newEntryModel.EmpId = product.EmployeeID; //empid
                                                newEntryModel.ProjectID = product.ProjectID;
                                                newEntryModel.Quart = quarter;
                                                newEntryModel.EmpRemarks = "";
                                                newEntryModel.ApproveRejectComments = "";
                                                newEntryModel.ApproveRejectStatus = "A";
                                                newEntryModel.ApproveRejectUser = (long)Session[Constants.SessionEmpID];
                                                newEntryModel.ApproveRejectDate = DateTime.Now;
                                                newEntryModel.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));
                                                DB.EmpTimeSheet.Add(newEntryModel);
                                                DB.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            TempData["Error"] = ResourceMessage.DayscountExceed;
                                            //return View("Index", BindData(model.QID));
                                        }

                                    }
                                }
                            }

                            if ((product.IsEdit1 && product.InvolvementEditDays1 > 0) || (product.IsEdit2 && product.InvolvementEditDays2 > 0) || (product.IsEdit3 && product.InvolvementEditDays3 > 0))
                            {
                                var SubmittedMonths = new List<string>();
                                if (product.IsEdit1 && product.InvolvementEditDays1 > 0)
                                {
                                    SubmittedMonths.Add(product.Month1 + " " + Convert.ToDateTime(month1).Year);
                                }
                                if (product.IsEdit2 && product.InvolvementEditDays2 > 0)
                                {
                                    SubmittedMonths.Add(product.Month2 + " " + Convert.ToDateTime(month2).Year);
                                }
                                if (product.IsEdit3 && product.InvolvementEditDays3 > 0)
                                {
                                    SubmittedMonths.Add(product.Month3 + " " + Convert.ToDateTime(month3).Year);
                                }

                                string projectName = DB.ProjectMaster.FirstOrDefault(x => x.ProjectID == product.ProjectID).ProjectName;
                                var projectManagers = (from x in DB.ProjectEmployee.Where(x => x.ProjectID == product.ProjectID && x.EmployeeID != product.EmployeeID && x.CheckRole)
                                                       join y in DB.Employee on x.EmployeeID equals y.EmployeeID
                                                       join z in DB.User on y.UserID equals z.UserID
                                                       select new { z.Email, y.EmpFirstName, y.EmpLastName, y.EmpMiddleName }).ToList<dynamic>();
                                if (SubmittedMonths.Any() && projectManagers.Any())
                                {
                                    var emp = DB.Employee.FirstOrDefault(x => x.EmployeeID == product.EmployeeID);
                                    var emailObj = new TimeSheetSubmissionEmailModel()
                                    {
                                        EmpName = Models.Common.GetName(emp.EmpFirstName, emp.EmpLastName, emp.EmpMiddleName),
                                        ManagerInfo = projectManagers,
                                        ProjectName = projectName,
                                        SubmissionDates = string.Join(", ", SubmittedMonths)

                                    };
                                    bool emailResult = await Email.SendTimeSubmissionEmail(emailObj);
                                    if (!emailResult)
                                    {
                                        emailStatus.Add(new KeyValuePair<string, string>(emailObj.EmpName, projectName));
                                    }
                                }

                            }
                        }


                    }

                    return Json(new { MODEL , emailStatus });


                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog(ex);
                    throw ex;
                }
            }
            else
            {
                ViewBag.Quadrent = GetQuardrent();
                long empId = (long)Session[Constants.SessionEmpID];
                ViewBag.QuarterList = DropdownList.PreviousAndQuarterListNewEntryOnBehalf(empId);

                NewEntryByProjectSelection model = new NewEntryByProjectSelection
                {
                    Quarter = ViewBag.Quadrent,
                    Month1 = "Jan-2019"
                };
                return View("Index", model);
            }

        }

        private decimal CalculateInvolvementPercentage(long _empId, decimal editDays, NewEntryModel newEntryModel)
        {
            WorkDaysModel obj = DB.Workdays.FirstOrDefault(x => x.EmpID == _empId && x.InvolveMonth == newEntryModel.InvolveMonth.Date);
            decimal empDaysCount = obj == null ? 0 : obj.DaysCount;
            if (obj == null)
            {
                empDaysCount = DB.Settings.Where(x => x.SetCode == "ManDays").First().SetValue;
            }
            decimal involveMentPercentage = (editDays * 100) / Convert.ToDecimal(empDaysCount);
            return involveMentPercentage;
        }

        public static string GetQuardrent()
        {
            string Quard = "";
            int currentmonth = DateTime.Now.Month;

            if (currentmonth == 1 || currentmonth == 2 || currentmonth == 3)
            {
                Quard = "Q1";
            }
            else if (currentmonth == 4 || currentmonth == 5 || currentmonth == 6)
            {
                Quard = "Q2";
            }
            else if (currentmonth == 7 || currentmonth == 8 || currentmonth == 9)
            {
                Quard = "Q3";
            }
            else if (currentmonth == 10 || currentmonth == 11 || currentmonth == 12)
            {
                Quard = "Q4";
            }
            return Quard;
        }

        [HttpPost]
        public JsonResult Read([DataSourceRequest]DataSourceRequest request)
        {

            try
            {
                List<NewEntryList> GetDetails = DB.Database.SqlQuery<NewEntryList>(
                           @"exec " + Constants.P_GetEmpTimesheet_NewEntry_Test + " @EmpID,@IsNewEntry",
                           new object[] {
                        new SqlParameter("@EmpID",  (long)Session[Constants.SessionEmpID]),
                        new SqlParameter("@IsNewEntry",  true)
                           }).ToList();

                GetDetails = GetDetails.OrderByDescending(x => x.CreatedOn).ToList();

                var res = (from x in GetDetails
                           group x by x.SequenceNo into g
                           select new { g.Key, List = g.ToList() }).ToList();

                GetDetails = new List<NewEntryList>();
                foreach (var item in res)
                {
                    GetDetails.Add(item.List.FirstOrDefault());
                }

                //  GetDetails = GetDetails.OrderByDescending(x => x.RefNo).ToList();
                for (int i = 0; i < GetDetails.Count; i++)
                {
                    GetDetails[i].ItemNo = i + 1;
                }
                return Json(GetDetails.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        public JsonResult GetQuarters()
        {
            var empId = (Int64)Session[Constants.SessionEmpID];
            return Json(DropdownList.PreviousAndQuarterListNewEntryOnBehalf(empId), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetProjects(string text)
        {
            IEnumerable<ProjectLookUp> projects = DropdownList.ProjectManagerPMS((long)Session[Constants.SessionEmpID], (long)(Session[Constants.SessionRoleID]));

            if (!string.IsNullOrEmpty(text))
            {
                projects = projects.Where(p => p.ProjectName.Contains(text));
            }

            return Json(projects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMembers(string text)
        {
            IEnumerable<EmployeeLookUp> emps = DropdownList.EmployeeListViaRole((long)Session[Constants.SessionEmpID], (long)(Session[Constants.SessionRoleID]));

            if (!string.IsNullOrEmpty(text))
            {
                emps = emps.Where(p => p.EmpName.Contains(text));
            }

            return Json(emps, JsonRequestBehavior.AllowGet);
        }

    }
}