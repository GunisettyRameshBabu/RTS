using SCTimeSheet_DAL;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace SCTimeSheet.Controllers
{
    public abstract class BaseController : Controller
    {
        protected ApplicationDBContext DB { get; set; }

        public BaseController()
        {
            DB = new ApplicationDBContext();
        }

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
        #endregion

        #region On Authorization
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                base.OnAuthorization(filterContext);

                string controllername = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (!controllername.ToLower().Equals("login"))
                {
                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (authCookie == null)
                    {
                        if (controllername.ToLower().Equals("forgotpassword"))
                        {
                            RedirectToAction("Index", "ForgotPassword");
                        }
                        else
                        {
                            if (!Request.IsAuthenticated)
                            {
                                filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary(new { controller = "Login", action = "Index", area = string.Empty, returnUrl = filterContext.HttpContext.Request.RawUrl })
                            );

                                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                            }
                            else
                            {
                                var res = (from x in DB.User
                                           join y in DB.Employee on x.UserID equals y.UserID
                                           where x.Email == User.Identity.Name
                                           select new { x.RoleID, x.UserID, y.EmployeeID, Name = (y.EmpFirstName ?? "") + " " + (y.EmpMiddleName ?? "") + " " + (y.EmpLastName ?? "") }).FirstOrDefault();
                                if (res != null)
                                {
                                    Session[Constants.SessionEmpID] = res.EmployeeID; //empdetails.empid
                                    Session[Constants.SessionEmpName] = res.Name;

                                    Session[Constants.SessionUserID] = res.UserID;
                                    Session[Constants.SessionRoleID] = res.RoleID;
                                    
                                    var pageList = (from x in DB.PageMapping
                                                    join y in DB.Page on x.PageID equals y.PageID
                                                    where x.RoleID == res.RoleID && x.IsActive
                                                    select y.PageName.ToLower() + "|" + Constants.Access).ToList();
                                    Session[Constants.SessionPageAccess] = pageList;
                                }

                            }
                            
                        }
                    }
                    else
                    {
                        var user = DB.User.Where(c => c.Email == User.Identity.Name && c.IsActive).FirstOrDefault();
                        if (user == null)
                            filterContext.Result = new ViewResult { ViewName = "Unauthorized" };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
            }
        }

        #endregion

        #region On Action Executing
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var rd = context.HttpContext.Request.RequestContext.RouteData;
                string controllername = rd.Values["controller"].ToString().ToLower();

                if (!controllername.Equals("login"))
                {
                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (authCookie == null)
                    {
                        if (!Request.IsAuthenticated)
                        {
                            context.Result = new RedirectToRouteResult(
                     new RouteValueDictionary(new { controller = "Login", action = "Index", area = string.Empty })
                 );
                            context.Result.ExecuteResult(context.Controller.ControllerContext);
                        }
                        else
                        {
                            var res = (from x in DB.User
                                       join y in DB.Employee on x.UserID equals y.UserID
                                       where x.Email == User.Identity.Name
                                       select new { x.RoleID, x.UserID, y.EmployeeID, Name = (y.EmpFirstName ?? "") + " " + (y.EmpMiddleName ?? "") + " " + (y.EmpLastName ?? "") }).FirstOrDefault();
                            if (res != null)
                            {
                                Session[Constants.SessionEmpID] = res.EmployeeID; //empdetails.empid
                                Session[Constants.SessionEmpName] = res.Name;

                                Session[Constants.SessionUserID] = res.UserID;
                                Session[Constants.SessionRoleID] = res.RoleID;

                                var pageList = (from x in DB.PageMapping.Where(x => x.IsActive && x.RoleID == res.RoleID)
                                                join y in DB.Page on x.PageID equals y.PageID
                                                select y.PageName.ToLower() + "|" + Constants.Access).ToList();
                                Session[Constants.SessionPageAccess] = pageList;
                            }
                        }
                 
                    }
                    else
                    {
                        var user = DB.User.Where(c => c.Email == User.Identity.Name && c.IsActive).FirstOrDefault();
                        if (user == null)
                            context.Result = new ViewResult { ViewName = "Unauthorized" };
                        else
                        {
                            bool skipCheck = false;
                            try
                            {
                                object[] aa = context.ActionDescriptor.GetCustomAttributes(typeof(SkipAuthorizationAttribute), true);
                                skipCheck = aa != null && aa.Any();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.ErrorLog(ex);
                            }

                            if (!skipCheck)
                            {
                                Int64 userId = user.UserID;

                                if (!(new UserAccess()).AuthorizationCheck(this, userId, controllername))
                                    context.Result = new ViewResult { ViewName = "Unauthorized" };
                            }
                        }
                        base.OnActionExecuting(context);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
            }
        }

        #endregion

        #region Submit Button Attribute

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class SubmitButtonAttribute : ActionNameSelectorAttribute
        {

            public string Name { get; set; }
            public string Argument { get; set; }

            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                try
                {
                    var isValidName = false;
                    var keyValue = string.Format("{0}:{1}", Name, Argument);
                    var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

                    if (value != null)
                    {
                        controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                        isValidName = true;
                    }

                    return isValidName;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog(ex);
                    throw ex;
                }
            }
        }

        #endregion

        #region RestrictAccessToRoleAttribute

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class RestrictAccessToRoleAttribute : AuthorizeAttribute
        {
            public string Role { get; set; }

            public RestrictAccessToRoleAttribute(string role)
            {
                this.Role = role;
            }

            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                var isAuthorized = base.AuthorizeCore(httpContext);
                if (!isAuthorized)
                {
                    return false;
                }
                return true;
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new ViewResult { ViewName = "Unauthorized" };
                }
            }
        }

        public class NoCacheAttribute : ActionFilterAttribute
        {
            public override void OnResultExecuting(ResultExecutingContext filterContext)
            {
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetNoStore();

                base.OnResultExecuting(filterContext);
            }
        }
        #endregion

        #region SkipAuthorizationAttribute

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class SkipAuthorizationAttribute : Attribute
        {

        }

        #endregion

        
    }
}