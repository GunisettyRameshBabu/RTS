using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using System;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult Index()
        {
            string CurrentController = (string)this.RouteData.Values["controller"];
            string CurrentAction = (string)this.RouteData.Values["action"];
            int CurrentExceptionCode = (int)this.RouteData.Values["exceptioncode"];
            Exception ex = (Exception)this.RouteData.Values["exception"];
            TempData["exceptioncode"] = CurrentExceptionCode;
            if (CurrentExceptionCode == 404) TempData["headermessage"] = "Page not found.";
            else if (CurrentExceptionCode == 403) TempData["headermessage"] = "Access denied.";
            else TempData["headermessage"] = "";

            ErrorLogModel log = LogHelper.LogError(ex, true);
            return View(log);
        }
    }
}