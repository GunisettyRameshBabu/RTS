using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class AdminTimeSheetLockController : BaseController
    {
        // GET: AdminTimeSheetLock
        public ActionResult Index()
        {
            SetViewState();
            return View();
        }

        private void SetViewState()
        {
            long empId = (long)Session[Constants.SessionEmpID];
            int month = DateTime.Now.Month;

            List<SelectListItem> list = new List<SelectListItem>();

            string preQuarter = month <= 3 ? "Q4" : month > 3 && month <= 6 ? "Q1" : month > 6 && month <= 9 ? "Q2" : "Q3";
            list.Add(new SelectListItem() { Text = preQuarter + " " + (preQuarter == "Q4" ? DateTime.Now.Year - 1 : DateTime.Now.Year), Value = preQuarter });

            ViewBag.QuarterList = list;
            ViewBag.LockStatus = (from x in DB.TimeSheetLocks
                                  join y in DB.Employee on x.updatedBy equals y.UserID
                                  orderby x.Id descending
                                  select x.Status == true ? "UnLock" : "Lock").FirstOrDefault();

            if (ViewBag.LockStatus == "" || ViewBag.LockStatus == null)
            {
                ViewBag.LockStatus = "Lock";
            }



        }

        private int GetQuarter()
        {
            int result = 0;
            switch (DateTime.Now.Month)
            {
                case 1:
                case 2:
                case 3:
                    result = 1;
                    break;

                case 4:
                case 5:
                case 6:
                    result = 2;
                    break;


                case 7:
                case 8:
                case 9:
                    result = 3;
                    break;

                case 10:
                case 11:
                case 12:
                    result = 4;
                    break;
            }
            return result;
        }

        // POST: AdminTimeSheetLock/Create
        [HttpPost]
        public ActionResult Create(SCTimeSheet_DAL.Models.TimeSheetLockModel collection)
        {
            try
            {
                SetViewState();
                if (collection == null)
                {
                    ViewBag.ErrorMessage = "Invalid Object";

                    return View("Index", collection);
                }

                if (string.IsNullOrEmpty(collection.Quarter))
                {
                    ViewBag.ErrorMessage = "Please select the quarter";

                    return View("Index", collection);
                }

                switch (collection.Quarter)
                {
                    case "Q1":
                        collection.StartDate = new DateTime(DateTime.Now.Year, 1, 01);
                        collection.EndDate = new DateTime(DateTime.Now.Year, 3, 31);

                        break;

                    case "Q2":
                        collection.StartDate = new DateTime(DateTime.Now.Year, 4, 01);
                        collection.EndDate = new DateTime(DateTime.Now.Year, 6, 30);
                        break;

                    case "Q3":
                        collection.StartDate = new DateTime(DateTime.Now.Year, 7, 01);
                        collection.EndDate = new DateTime(DateTime.Now.Year, 9, 30);
                        break;

                    case "Q4":
                        collection.StartDate = new DateTime(DateTime.Now.Year - 1, 10, 01);
                        collection.EndDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
                        break;


                }
                // TODO: Add insert logic here
                collection.UpdatedDate = DateTime.Now;
                collection.updatedBy = Convert.ToInt32(Session[Constants.SessionUserID]);
                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Please fill mandatory fields";
                    return View("Index", collection);
                }
                else
                {
                    bool valid = true;
                    StringBuilder stringBuilder = new StringBuilder();
                    if (collection.EndDate < collection.StartDate)
                    {
                        stringBuilder.Append("End date can not be less than start date");
                        stringBuilder.AppendLine();
                        valid = false;
                    }



                    if (collection.EndDate.Month - collection.StartDate.Month > 3)
                    {
                        stringBuilder.Append("Date Range Can not be more than one quarter");
                        stringBuilder.AppendLine();
                        valid = false;
                    }

                    if (valid)
                    {
                        TimeSheetLockModel items = DB.TimeSheetLocks.Where(x => x.StartDate == collection.StartDate && x.EndDate == collection.EndDate).OrderByDescending(x => x.Id).FirstOrDefault();
                        //if (items == null )
                        //{
                        //var item = DB.TimeSheetLocks.FirstOrDefault(x => x.Id == (items.Any() ? items[0].Id : 0));
                        //if (item == null)
                        //{
                        if (items == null || items.Status == false)
                        {
                            var year = collection.Quarter == "Q4" ? DateTime.Now.Year - 1 : DateTime.Now.Year;
                            var deletedRec = DB.EmpTimeSheet.Where(x => x.Quart == collection.Quarter && x.InvolveMonth.Year == year && x.Status == 2).ToList();
                            foreach (var item in deletedRec)
                            {
                                item.Status = Convert.ToInt64(ReadConfig.GetValue("StatusRejected"));
                                item.ApproveRejectComments = "Rejected Due to Timesheet locked";
                                item.ApproveRejectStatus = "R";
                                item.ApproveRejectUser = 1;
                                item.ApproveRejectDate = DateTime.Now;
                                DB.EmpTimeSheet.Attach(item);
                                DB.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                DB.SaveChanges();
                            }
                        }
                        ViewBag.ErrorMessage = "";
                        collection.Status = items == null ? true : !items.Status;
                        collection.Quarter = collection.Quarter == "Q4" ? collection.Quarter + " " + (DateTime.Now.Year - 1) : collection.Quarter + " " + DateTime.Now.Year;
                        DB.TimeSheetLocks.Add(collection);
                        DB.SaveChanges();

                        //}
                        //else
                        //{
                        //ViewBag.ErrorMessage = "";
                        //collection.Id = item.Id;
                        //DB.Entry(item).CurrentValues.SetValues(collection);
                        //DB.SaveChanges();

                        //}

                        return RedirectToAction("Index");

                        // }
                        //else
                        //{
                        //    ViewBag.ErrorMessage = "Time Sheet Already Exist with the same combination";
                        //    return View("Index", collection);
                        //}
                    }

                    else
                    {
                        ViewBag.ErrorMessage = stringBuilder.ToString();
                        return View("Index", collection);
                    }


                }

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);

                return View();
            }
        }



        [HttpPost]
        public JsonResult GetTimeSheetLockDetails([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                List<TimeSheetLockUIModel> GetDetails = (from x in DB.TimeSheetLocks
                                                         join y in DB.Employee on x.updatedBy equals y.UserID
                                                         orderby x.Id descending
                                                         select new {x,y }).AsEnumerable().Select(x => new TimeSheetLockUIModel() { StartDate = x.x.StartDate, EndDate = x.x.EndDate, Status = x.x.Status == true ? "Locked" : "UnLocked", updatedBy = SCTimeSheet.Models.Common.GetName(x.y.EmpFirstName,x.y.EmpLastName,x.y.EmpMiddleName), UpdatedDate = x.x.UpdatedDate, Quarter = x.x.Quarter }).ToList();

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
