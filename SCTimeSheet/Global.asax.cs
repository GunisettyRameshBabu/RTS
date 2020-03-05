using SCTimeSheet_DAL;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SCTimeSheet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            HttpContext httpContext = HttpContext.Current;
            var exception = Server.GetLastError();

            if (httpContext != null && (exception != null))
            {
                var httpException = exception as HttpException;
                int ExceptionCode = httpException.GetHttpCode();

                if (httpContext.Request.Url.AbsoluteUri == Request.Url.GetLeftPart(UriPartial.Authority) + "/")
                {
                    ErrorLogModel log = LogHelper.LogError(exception, true);
                }
                else
                {
                    Response.Clear();
                    Server.ClearError();
                    Response.TrySkipIisCustomErrors = true;
                    var routeData = new RouteData();
                    routeData.Values["controller"] = "Error";
                    routeData.Values["action"] = "Index";
                    routeData.Values["exception"] = exception;
                    routeData.Values["exceptioncode"] = ExceptionCode;
                    HttpContext.Current.Response.ContentType = "text/html";

                    IController errorsController = new Controllers.ErrorController();
                    var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
                    errorsController.Execute(rc);
                }
            }
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            string user = User?.Identity?.Name;
            if (!string.IsNullOrEmpty(user))
            {
                ApplicationDBContext DB = new ApplicationDBContext();
                var userDetails = DB.User.Where(x => x.Email == user).FirstOrDefault();
                if (userDetails != null)
                {
                    var empDetails = (from x in DB.Employee
                                      where x.UserID == userDetails.UserID
                                      select x.EmployeeID).FirstOrDefault();
                    if (empDetails > 0)
                    {
                        Session[Constants.SessionEmpID] = empDetails;
                    }
                    Session[Constants.SessionUserID] = userDetails.UserID;
                    Session[Constants.SessionRoleID] = userDetails.RoleID;
                }
            }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
            newCulture.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = newCulture;
        }
    }
}
