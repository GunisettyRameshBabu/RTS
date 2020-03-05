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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class NewEntryController : BaseController
    {
        private readonly string[] months = new string[] { "Month1", "Month2", "Month3" };

        // GET: NewEntry
        [NoCache]
        public ActionResult Index(string quarter = null)
        {
            quarter = quarter ?? GetQuarter();
            long empId = Convert.ToInt64(Session[Constants.SessionEmpID]);
            ViewBag.QuarterList = DropdownList.PreviousAndQuarterList(empId, true);
            if (TempData["EmailNotificationErrors"] != null)
            {
                var emailErrors = (TempData["EmailNotificationErrors"] as List<string>);
                if (emailErrors.Any())
                {
                    ViewBag.EmailNotificationErrors = "Unable to send notification email for the below projects line" + string.Join("lineline", emailErrors);
                }
                else
                {
                    ViewBag.EmailNotificationErrors = "";
                }

            }
            else
            {
                ViewBag.EmailNotificationErrors = "";
            }
            return View(BindData(quarter));

        }

        [NoCache]
        [HttpPost]
        [ActionName("ChangeQuarter")]
        public ActionResult ChangeQuarter(string quarter)
        {
            return RedirectToActionPermanent("Index", new { quarter });
        }


        private decimal CalculateInvolvementPercentage(long _empId, decimal editDays, NewEntryModel newEntryModel)
        {
            decimal? empDaysCount = DB.Workdays.FirstOrDefault(x => x.EmpID == _empId && x.InvolveMonth == newEntryModel.InvolveMonth.Date)?.DaysCount;
            if (empDaysCount == null)
            {
                empDaysCount = DB.Settings.Where(x => x.SetCode == "ManDays").First().SetValue;
            }
            decimal involveMentPercentage = (editDays * 100) / Convert.ToInt32(empDaysCount);
            return involveMentPercentage;
        }


        [HttpPost]
        [SubmitButton(Name = "action", Argument = "Submit")]
        public async Task<ActionResult> Submit(NewEntryBox model)
        {
            long _empId = (long)Session[Constants.SessionEmpID];
            string _empName = Session[Constants.SessionEmpName].ToString();
            List<string> emailStatus = new List<string>();
            try
            {
                decimal edit1 = 0;
                decimal edit2 = 0;
                decimal edit3 = 0;
                model.Items[model.Items.Count - 1].InvolvementEditDays1 = 0;
                model.Items[model.Items.Count - 1].InvolvementEditDays2 = 0;
                model.Items[model.Items.Count - 1].InvolvementEditDays3 = 0;
                for (int i = 0; i < model.Items.Count - 1; i++)
                {
                    edit1 = edit1 + model.Items[i].InvolvementEditDays1;
                    edit2 = edit2 + model.Items[i].InvolvementEditDays2;
                    edit3 = edit3 + model.Items[i].InvolvementEditDays3;
                }
                model.Items[model.Items.Count - 1].InvolvementEditDays1 = edit1;
                model.Items[model.Items.Count - 1].InvolvementEditDays2 = edit2;
                model.Items[model.Items.Count - 1].InvolvementEditDays3 = edit3;

                if (Convert.ToDouble(model.Items[model.Items.Count - 1].InvolvementDays1) < Convert.ToDouble(model.Items[model.Items.Count - 1].InvolvementEditDays1) || Convert.ToDouble(model.Items[model.Items.Count - 1].InvolvementDays2) < Convert.ToDouble(model.Items[model.Items.Count - 1].InvolvementEditDays2) || Convert.ToDouble(model.Items[model.Items.Count - 1].InvolvementDays3) < Convert.ToDouble(model.Items[model.Items.Count - 1].InvolvementEditDays3))
                {
                    ViewBag.ErrorMessage = "Total Input is more than allowable days, Please adjust your timesheet";
                    ViewBag.QuarterList = model.QID;
                    ViewBag.QuarterList = DropdownList.PreviousAndQuarterList(_empId, true);
                    return View("Index", BindData(model.QID));
                }

                if (model != null)
                {

                    foreach (BoxItems item in model.Items.Where(x => x.IsEdit1 || x.IsEdit2 || x.IsEdit3))
                    {

                        if (item.ProjectID != 0)
                        {
                            int _currentNumber = 0;
                            IQueryable<int> items = DB.EmpTimeSheet.OrderByDescending(u => u.SequenceNo).Take(1).Select(e => e.SequenceNo);
                            foreach (int ir in items)
                            {
                                _currentNumber = ir;
                            }
                            if (_currentNumber == 0) { _currentNumber = 100; }
                            else { _currentNumber++; }

                            foreach (string month in months)
                            {
                                bool isEditable = false;
                                NewEntryModel newEntryModel = new NewEntryModel();
                                int roleId = Convert.ToInt32(Session[Constants.SessionRoleID]);
                                newEntryModel.ApproveRejectStatus = roleId == 1 ? "A" : DB.ProjectEmployee.FirstOrDefault(x => x.EmployeeID == _empId && x.ProjectID == item.ProjectID).CheckRole == true ? "A" : null;
                                if (month == months[0])
                                {
                                    newEntryModel.InvolveMonth = Convert.ToDateTime(model.Month1);
                                    newEntryModel.DaysCount = item.InvolvementDays1;
                                    newEntryModel.DaysEditCount = item.InvolvementEditDays1;
                                    newEntryModel.InvolvePercent = CalculateInvolvementPercentage(_empId, item.InvolvementEditDays1, newEntryModel);
                                    if (item.IsEdit1)
                                    {
                                        isEditable = true;
                                    }
                                }
                                if (month == months[1])
                                {
                                    newEntryModel.InvolveMonth = Convert.ToDateTime(model.Month2);
                                    newEntryModel.DaysCount = item.InvolvementDays2;
                                    newEntryModel.DaysEditCount = item.InvolvementEditDays2;
                                    newEntryModel.InvolvePercent = CalculateInvolvementPercentage(_empId, item.InvolvementEditDays2, newEntryModel);
                                    if (item.IsEdit2)
                                    {
                                        isEditable = true;
                                    }
                                }
                                if (month == months[2])
                                {
                                    newEntryModel.InvolveMonth = Convert.ToDateTime(model.Month3);
                                    newEntryModel.DaysCount = item.InvolvementDays3;
                                    newEntryModel.DaysEditCount = item.InvolvementEditDays3;
                                    newEntryModel.InvolvePercent = CalculateInvolvementPercentage(_empId, item.InvolvementEditDays3, newEntryModel);
                                    if (item.IsEdit3)
                                    {
                                        isEditable = true;
                                    }
                                }
                                if (newEntryModel.DaysCount != 0)
                                {
                                    if (isEditable)
                                    {
                                        NewEntryModel exists = DB.EmpTimeSheet.FirstOrDefault(x => x.InvolveMonth == newEntryModel.InvolveMonth && x.EmpId == _empId && x.ProjectID == item.ProjectID);
                                        if (exists != null)
                                        {

                                            newEntryModel.TsID = exists.TsID;
                                            newEntryModel.RefNo = exists.RefNo;
                                            newEntryModel.SequenceNo = exists.SequenceNo;
                                            newEntryModel.EntryBy = _empId; //empid
                                            newEntryModel.EntryDate = DateTime.Now;
                                            newEntryModel.Quart = model.QID;
                                            newEntryModel.EntryRole = (long)Session[Constants.SessionRoleID];
                                            newEntryModel.EmpId = _empId; //empid
                                            newEntryModel.ProjectID = item.ProjectID;
                                            newEntryModel.EmpRemarks = "";
                                            newEntryModel.Status = newEntryModel.ApproveRejectStatus == "A" ? Convert.ToInt64(ReadConfig.GetValue("StatusApproved")) : Convert.ToInt64(ReadConfig.GetValue("StatusPending"));
                                            if (newEntryModel.ApproveRejectStatus == "A")
                                            {
                                                newEntryModel.ApproveRejectUser = _empId;
                                            }
                                            DB.Entry(exists).CurrentValues.SetValues(newEntryModel);
                                        }
                                        else
                                        {

                                            newEntryModel.RefNo = AutoGen.GetReferenceNumber();
                                            newEntryModel.SequenceNo = _currentNumber;
                                            newEntryModel.EntryBy = _empId; //empid
                                            newEntryModel.EntryDate = DateTime.Now;
                                            newEntryModel.Quart = model.QID;
                                            newEntryModel.EntryRole = (long)Session[Constants.SessionRoleID];
                                            newEntryModel.EmpId = _empId; //empid
                                            newEntryModel.ProjectID = item.ProjectID;
                                            newEntryModel.EmpRemarks = "";
                                            newEntryModel.Status = newEntryModel.ApproveRejectStatus == "A" ? Convert.ToInt64(ReadConfig.GetValue("StatusApproved")) : Convert.ToInt64(ReadConfig.GetValue("StatusPending"));
                                            if (newEntryModel.ApproveRejectStatus == "A")
                                            {
                                                newEntryModel.ApproveRejectUser = _empId;
                                            }
                                            DB.EmpTimeSheet.Add(newEntryModel);
                                        }
                                        DB.SaveChanges();

                                        if (newEntryModel.Status == Convert.ToInt64(ReadConfig.GetValue("StatusApproved")))
                                        {
                                            TempData["Success"] = ResourceMessage.NewEntryApprove;
                                        }
                                        else if (newEntryModel.Status == Convert.ToInt64(ReadConfig.GetValue("StatusPending")))
                                        {

                                            TempData["Success"] = ResourceMessage.NewEntrySubmit;
                                        }

                                    }
                                }
                            }
                            
                            if ((item.InvolvementDays1 > 0 && item.InvolvementEditDays1 > 0) || (item.InvolvementDays2 > 0 && item.InvolvementEditDays2 > 0) || (item.InvolvementDays3 > 0 && item.InvolvementEditDays3 > 0))
                            {
                                var SubmittedMonths = new List<string>();
                                if (item.IsEdit1 && item.InvolvementEditDays1 > 0)
                                {
                                    SubmittedMonths.Add(model.Month1);
                                }
                                if (item.IsEdit2 && item.InvolvementEditDays2 > 0)
                                {
                                    SubmittedMonths.Add(model.Month2);
                                }
                                if (item.IsEdit3 && item.InvolvementEditDays3 > 0)
                                {
                                    SubmittedMonths.Add(model.Month3);
                                }

                                string projectName = DB.ProjectMaster.FirstOrDefault(x => x.ProjectID == item.ProjectID).ProjectName;
                                var projectManagers = (from x in DB.ProjectEmployee.Where(x => x.ProjectID == item.ProjectID && x.EmployeeID != _empId && x.CheckRole)
                                                                 join y in DB.Employee on x.EmployeeID equals y.EmployeeID
                                                                 join z in DB.User on y.UserID equals z.UserID
                                                                 select new { z.Email, y.EmpFirstName, y.EmpLastName, y.EmpMiddleName }).ToList<dynamic>();
                                if (SubmittedMonths.Any() && projectManagers.Any())
                                {
                                    bool emailResult = await Email.SendTimeSubmissionEmail(new TimeSheetSubmissionEmailModel()
                                    {
                                        EmpName = _empName,
                                        ManagerInfo = projectManagers,
                                        ProjectName = projectName,
                                        SubmissionDates = string.Join(", ", SubmittedMonths)

                                    });
                                    if (!emailResult)
                                    {
                                        emailStatus.Add(projectName);
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
                LogHelper.ErrorLog(ex);
            }
            TempData["EmailNotificationErrors"] = emailStatus;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Read([DataSourceRequest]DataSourceRequest request)
        {

            try
            {
                System.Collections.Generic.List<NewEntryList> GetDetails = DB.Database.SqlQuery<NewEntryList>(
                           @"exec " + Constants.P_GetEmpTimesheet_NewEntry_Test + " @EmpID,@IsNewEntry",
                           new object[] {
                        new SqlParameter("@EmpID",  (long)Session[Constants.SessionEmpID]),
 new SqlParameter("@IsNewEntry",  false)
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

                for (int i = 1; i <= GetDetails.Count; i++)
                {
                    GetDetails[i - 1].ItemNo = i;
                }


                return Json(GetDetails.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }


        [HttpPost]
        [SubmitButton(Name = "action", Argument = "Select")]
        public ActionResult Select(FormCollection formcollection)
        {
            string quarter = formcollection["QID"];
            ViewBag.QuarterList = quarter;
            long _empId = (long)Session[Constants.SessionEmpID];
            ViewBag.QuarterList = DropdownList.PreviousAndQuarterList(_empId, true);
            return View("Index", BindData(quarter));
        }



        public NewEntryBox BindData(string quart)
        {

            var list = (from Quart in DB.Quarter
                        where Quart.Quarter == quart
                        select new
                        {
                            Quart.Month
                        }).ToList();

            string currentQuarter = GetQuarter();

            string currentmonth = quart == currentQuarter ? DateTime.Now.Month.ToString("d2") : quart == "Q4" ? "10" : quart == "Q3" ? "07" : quart == "Q2" ? "04" : "01";

            string month1 = quart != currentQuarter && quart == "Q4" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + list[0].Month + "/01") : DateTime.Now.ToString("yyyy/" + list[0].Month + "/01");
            string month2 = quart != currentQuarter && quart == "Q4" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + list[1].Month + "/01") : DateTime.Now.ToString("yyyy/" + list[1].Month + "/01");
            string month3 = quart != currentQuarter && quart == "Q4" ? DateTime.Now.AddYears(-1).ToString("yyyy/" + list[2].Month + "/01") : DateTime.Now.ToString("yyyy/" + list[2].Month + "/01");

            if (currentmonth == list[0].Month)
            {
                TempData["CheckMonth"] = 1;
            }
            else if (currentmonth == list[1].Month)
            {
                TempData["CheckMonth"] = 2;
            }
            else if (currentmonth == list[2].Month)
            {
                TempData["CheckMonth"] = 3;
            }
            long empid = (long)Session[Constants.SessionEmpID];
            System.Collections.Generic.List<BoxItems> GetDetails = DB.Database.SqlQuery<BoxItems>(
                             @"exec " + Constants.P_GetNewEntryDefault_Test + " @EmpID,@Month1,@Month2,@Month3",
                             new object[] {
                        new SqlParameter("@EmpID",  empid),
                        new SqlParameter("@Month1",  month1),
                        new SqlParameter("@Month2",  month2),
                        new SqlParameter("@Month3",  month3)
                             }).ToList();


            int year = (quart == "Q4" && currentQuarter != "Q4") ? DateTime.Now.AddYears(-1).Year : DateTime.Now.Year;

            NewEntryBox model = new NewEntryBox
            {
                CheckMonth = TempData["CheckMonth"].ToString(),
                Month1 = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(list[0].Month)) + " " + year,
                Month2 = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(list[1].Month)) + " " + year,
                Month3 = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(list[2].Month)) + " " + year
            };

            foreach (BoxItems item in GetDetails)
            {
                item.InvolvementEditDays1 = item.InvolvementEditDays1 == 0 && item.InvolvementDays1 != 0 && item.IsEdit1 ? item.InvolvementDays1 : item.InvolvementEditDays1;
                item.InvolvementEditDays2 = item.InvolvementEditDays2 == 0 && item.InvolvementDays2 != 0 && item.IsEdit2 ? item.InvolvementDays2 : item.InvolvementEditDays2;
                item.InvolvementEditDays3 = item.InvolvementEditDays3 == 0 && item.InvolvementDays3 != 0 && item.IsEdit3 ? item.InvolvementDays3 : item.InvolvementEditDays3;
            }

            DateTime origDT1 = Convert.ToDateTime(month1);
            DateTime lastDate1 = new DateTime(origDT1.Year, origDT1.Month, 1).AddMonths(1).AddDays(-1);
            DateTime origDT2 = Convert.ToDateTime(month2);
            DateTime lastDate2 = new DateTime(origDT2.Year, origDT2.Month, 1).AddMonths(1).AddDays(-1);
            DateTime origDT3 = Convert.ToDateTime(month3);
            DateTime lastDate3 = new DateTime(origDT3.Year, origDT3.Month, 1).AddMonths(1).AddDays(-1);
            foreach (BoxItems item in GetDetails.Where(x => x.TsID != "Total"))
            {
                model.TotalDaysEditCount1 = model.TotalDaysEditCount1 + item.InvolvementEditDays1;
                model.TotalDaysEditCount2 = model.TotalDaysEditCount2 + item.InvolvementEditDays2;
                model.TotalDaysEditCount3 = model.TotalDaysEditCount3 + item.InvolvementEditDays3;
                item.OldInvolvementEditDays1 = item.InvolvementEditDays1;
                item.OldInvolvementEditDays2 = item.InvolvementEditDays2;
                item.OldInvolvementEditDays3 = item.InvolvementEditDays3;
                if (!model.ShowWarning)
                {
                    if (Models.Common.CheckDateRange(item.StartDate.Value, Convert.ToDateTime(month1), lastDate3))
                    {
                        if (!Models.Common.CheckIfNotMonthDateDate(Convert.ToDateTime(month1), Convert.ToDateTime(month2), Convert.ToDateTime(month3), item.StartDate.Value) || !Models.Common.CheckIfNotMonthDateDate(lastDate1, lastDate2, lastDate3, item.EndDate.Value))
                        {
                            model.ShowWarning = true;
                        }
                    }

                }

            }

            DateTime date1 = Convert.ToDateTime(month1);
            DateTime date2 = Convert.ToDateTime(month2);
            DateTime date3 = Convert.ToDateTime(month3);

            WorkDaysModel totalDays1 = DB.Workdays.FirstOrDefault(x => x.EmpID == empid && x.InvolveMonth == date1.Date);
            WorkDaysModel totalDays2 = DB.Workdays.FirstOrDefault(x => x.EmpID == empid && x.InvolveMonth == date2.Date);
            WorkDaysModel totalDays3 = DB.Workdays.FirstOrDefault(x => x.EmpID == empid && x.InvolveMonth == date3.Date);
            long manDays = 0;
            if (totalDays1 == null || totalDays2 == null || totalDays3 == null)
            {
                manDays = DB.Settings.Where(x => x.SetCode == "ManDays").First().SetValue;
            }
            if (GetDetails.Any())
            {

                int totalInvolvementPercentage = (from x in DB.Employee
                                                  where x.EmployeeID == empid
                                                  select x.TotalInvolvement ?? 100).FirstOrDefault();

                GetDetails[GetDetails.Count - 1].InvolvementEditDays1 = model.TotalDaysEditCount1;
                GetDetails[GetDetails.Count - 1].InvolvementEditDays2 = model.TotalDaysEditCount2;
                GetDetails[GetDetails.Count - 1].InvolvementEditDays3 = model.TotalDaysEditCount3;

                GetDetails[GetDetails.Count - 1].InvolvementDays1 = totalInvolvementPercentage * (totalDays1 != null ? totalDays1.DaysCount : manDays) / 100;
                GetDetails[GetDetails.Count - 1].InvolvementDays2 = totalInvolvementPercentage * (totalDays2 != null ? totalDays2.DaysCount : manDays) / 100;
                GetDetails[GetDetails.Count - 1].InvolvementDays3 = totalInvolvementPercentage * (totalDays3 != null ? totalDays3.DaysCount : manDays) / 100;
            }

            model.Count1 = model.TotalDaysEditCount1 == 0 && model.TotalDaysCount1 == 0 && date1 <= DateTime.Now ? totalDays1 != null ? totalDays1.DaysCount : manDays : model.TotalDaysEditCount1;
            model.Count2 = model.TotalDaysEditCount2 == 0 && model.TotalDaysCount2 == 0 && date2 <= DateTime.Now ? totalDays2 != null ? totalDays2.DaysCount : manDays : model.TotalDaysEditCount2;
            model.Count3 = model.TotalDaysEditCount3 == 0 && model.TotalDaysCount3 == 0 && date3 <= DateTime.Now ? totalDays3 != null ? totalDays3.DaysCount : manDays : model.TotalDaysEditCount3;

            model.QID = quart;

            model.CurrentYear = (quart == "Q4" && currentQuarter != "Q4") ? DateTime.Now.AddYears(-1).Year : DateTime.Now.Year;
            model.Items = GetDetails;
            foreach (BoxItems item in model.Items.Where(x => x.ProjectID == 0))
            {
                item.IsEdit1 = false;
                item.IsEdit2 = false;
                item.IsEdit3 = false;
            }

            ViewBag.IsEditable = model.Items.Any(x => (x.IsEdit1 || x.IsEdit2 || x.IsEdit3) && x.ProjectID != 0);
            return model;

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


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            ViewBag.ProjectList = DropdownList.ProjectList((long)Session[Constants.SessionEmpID], (long)(Session[Constants.SessionRoleID]));
            try
            {

                var entry = DB.EmpTimeSheet.Where(x => x.TsID == id).Select(x => new { x.Quart }).FirstOrDefault();

                return View("Index", BindData(entry.Quart));
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }
    }
}