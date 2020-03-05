using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
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
    public class TimesheetApprovalController : BaseController
    {
        // GET: TimesheetApproval
        [HttpGet]
        public ActionResult Index()
        {
            PendingListforApproval model = new PendingListforApproval();
            return View(model);
        }
        [HttpPost]
        public JsonResult PendingTimesheetList([DataSourceRequest]DataSourceRequest request)
        {
            var GetDetails = DB.Database.SqlQuery<PendingListforApproval>(
                        @"exec " + Constants.P_GetEmp_PendingTimesheet + " @Owner,@Role",
                        new object[] {
                        new SqlParameter("@Owner",  (Int64)Session[Constants.SessionEmpID]),
                        new SqlParameter("@Role",  (Int64)Session[Constants.SessionRoleID])
                        }).ToList();
            var res = (from x in GetDetails
                       group x by x.SequenceNo into g
                       select new { g.Key, List = g.ToList() }).ToList();

            GetDetails = new List<PendingListforApproval>();
            foreach (var item in res)
            {
                GetDetails.Add(item.List.FirstOrDefault());
            }
        
            return Json(GetDetails.ToDataSourceResult(request));
        }

        [HttpPost]
        public JsonResult ActionedTimesheetList([DataSourceRequest]DataSourceRequest request)
        {
            var GetDetails = DB.Database.SqlQuery<ApprovedTimesheet>(
            @"exec " + Constants.P_GetEmp_ActionedTimesheet + " @Owner",
            new object[] {
                        new SqlParameter("@Owner",  (Int64)Session[Constants.SessionEmpID])
            }).OrderByDescending(x => x.ApproveRejectDate).ToList();

            var res = (from x in GetDetails
                       group x by x.SequenceNo into g
                       select new { g.Key, List = g.ToList() }).ToList();

            GetDetails = new List<ApprovedTimesheet>();
            foreach (var item in res)
            {
                GetDetails.Add(item.List.FirstOrDefault());
            }

            return Json(GetDetails.ToDataSourceResult(request));
        }

        [HttpPost]
        public JsonResult TimesheetApprove(string[] DATA)
        {
            try
            {
                foreach (var i in DATA)
                {

                    var res = JsonConvert.DeserializeObject<ActionItemList>(i);
                    var sequenceNubmer = DB.EmpTimeSheet.Find(res.TsID).SequenceNo;

                    var records = DB.EmpTimeSheet.Where(x => x.SequenceNo == sequenceNubmer).ToList();
                    foreach (var existing in records)
                    {
                        if (existing != null)
                        {
                            // existing.InvolvePercent = 0;
                            existing.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));
                            existing.ApproveRejectComments = "";
                            existing.ApproveRejectStatus = "A";
                            existing.ApproveRejectUser = (Int64)Session[Constants.SessionEmpID];
                            existing.ApproveRejectDate = DateTime.Now;
                            DB.EmpTimeSheet.Attach(existing);
                            DB.Entry(existing).State = System.Data.Entity.EntityState.Modified;
                           

                        }
                    }
                    DB.SaveChanges();



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
        public JsonResult DetailPopup(int Id,int SeqNo)
        {
            try
            {
                var GetDetails = DB.Database.SqlQuery<PendingListforApproval>(
                              @"exec " + Constants.P_GetEmpPendingTimesheet_POPUP + " @TsID,@SeqNo",
                              new object[] {
                        new SqlParameter("@TsID", Id),
                        new SqlParameter("@SeqNo", SeqNo)
                              }).ToList();
                return Json(new { data = GetDetails });
            }
            catch (Exception ex)
            {

                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult SingleApprove(PendingListforApproval model)
        {
            try
            {
                //var existing = DB.NewEntry.FirstOrDefault(x => x.RefNo == model.RefNo);
                //if (existing != null)
                //{
                    if (Request.Form["Approve"] != null)
                    {
                        var selectedRec = DB.EmpTimeSheet.Where(x => x.SequenceNo == model.SequenceNo).ToList();
                        foreach (var item in selectedRec)
                        {
                            item.InvolvePercent = 0;
                            item.Status = Convert.ToInt64(ReadConfig.GetValue("StatusApproved"));
                            item.ApproveRejectComments = model.ApproveRejectComments;
                            item.ApproveRejectStatus = "A";
                            item.ApproveRejectUser = (Int64)Session[Constants.SessionEmpID];
                            item.ApproveRejectDate = DateTime.Now;
                            DB.EmpTimeSheet.Attach(item);
                            DB.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            DB.SaveChanges();
                        }
                    }
                    else if (Request.Form["Reject"] != null)
                    {
                        var deletedRec = DB.EmpTimeSheet.Where(x => x.SequenceNo == model.SequenceNo).ToList();
                        foreach (var item in deletedRec)
                        {
                            item.Status = Convert.ToInt64(ReadConfig.GetValue("StatusRejected"));
                            item.ApproveRejectComments = model.ApproveRejectComments;
                            item.ApproveRejectStatus = "R";
                            item.ApproveRejectUser = (Int64)Session[Constants.SessionEmpID];
                            item.ApproveRejectDate = DateTime.Now;
                            DB.EmpTimeSheet.Attach(item);
                            DB.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            DB.SaveChanges();
                        }
                    }                  
               // }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
                LogHelper.ErrorLog(ex);
            }
            return View("Index", model);
        }

        public class ActionItemList
        {

            public Int64 TsID { get; set; }


        }
    }
}