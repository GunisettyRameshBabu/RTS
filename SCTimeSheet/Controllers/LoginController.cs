using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using SCTimeSheet_UTIL.Resource;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SCTimeSheet.Controllers
{
    public class LoginController : BaseController
    {
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        // GET: Login
        public ActionResult Index()
        {

            try
            {
                if (Request.IsAuthenticated)
                {
                    //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    //if (authCookie != null)
                    //{
                        
                       // FormsAuthenticationTicket decTicket = FormsAuthentication.Decrypt(authCookie.Value);
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

                    string defaultPage = "NewEntry";
                    if ((long)Session[Constants.SessionRoleID] == Convert.ToInt64(ReadConfig.GetValue("RoleEmployee")))
                    {
                        defaultPage = "EmployeeDashboard";
                    }
                    else if ((long)Session[Constants.SessionRoleID] == Convert.ToInt64(ReadConfig.GetValue("RolePM")))
                    {
                        defaultPage = "ManagerDashboard";
                    }
                    else if ((long)Session[Constants.SessionRoleID] == Convert.ToInt64(ReadConfig.GetValue("RoleAdmin")))
                    {
                        defaultPage = "AdminDashboard";
                    }
                    else if ((long)Session[Constants.SessionRoleID] == Convert.ToInt64(ReadConfig.GetValue("RoleFinance")))
                    {
                        defaultPage = "AdminTimeSheetLock";
                    }


                    return RedirectToAction("Index", defaultPage);
                    //}
                }

            }
            catch (Exception ex)
            {

                LogHelper.ErrorLog(ex);
            }



            return View();
        }

        [HttpPost]
        [SubmitButton(Name = "action", Argument = "Login")]
        public void Login()
        {
            if (!Request.IsAuthenticated)
            {
                 
        HttpContext.GetOwinContext()
                    .Authentication.Challenge(new AuthenticationProperties { RedirectUri =  redirectUri },
                        OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else {
                try
                {

                    UserModel userExists = DB.User.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                    if (userExists != null)
                    {

                        if (userExists.IsActive)
                        {
                            var role = DB.Role.Join(DB.User, x => x.RoleID, y => y.RoleID, (x, y) => new { x.RoleName, x.RoleID }).Where(z => z.RoleID == userExists.RoleID).FirstOrDefault();
                            if (role != null)
                            {
                                FormsAuthenticationTicket frmAuthTicket = new FormsAuthenticationTicket(User.Identity.Name, true, Global.G_SessionTimeout);
                                string encTicket = FormsAuthentication.Encrypt(frmAuthTicket);
                                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                                {
                                    Expires = DateTime.Now.AddMonths(1),
                                    HttpOnly = true
                                };
                                Response.Cookies.Add(faCookie);

                                //EmployeeModel empDetails = DB.Employee.Where(x => x.UserID == userExists.UserID).FirstOrDefault(); //employee model
                                //if (empDetails != null)
                                //{
                                //    Session[Constants.SessionEmpID] = empDetails.EmployeeID; //empdetails.empid
                                //    Session[Constants.SessionEmpName] = empDetails.EmpFirstName + " " + empDetails.EmpLastName; // Employee Name
                                //}
                                // var empDetails = DB.Employee.Where(x => x.UserID == userDetails.UserID).FirstOrDefault(); //employee model
                                var empDetails = (from x in DB.Employee
                                                  where x.UserID == userExists.UserID
                                                  select new { x.EmployeeID, Name = (x.EmpFirstName ?? "") +  " " + (x.EmpMiddleName ?? "") + " " + (x.EmpLastName ?? "") }).FirstOrDefault();
                                if (empDetails != null)
                                {
                                    Session[Constants.SessionEmpID] = empDetails.EmployeeID;
                                    Session[Constants.SessionEmpName] = empDetails.Name;
                                }
                                Session[Constants.SessionUserID] = userExists.UserID;
                                Session[Constants.SessionRoleID] = userExists.RoleID;

                                if (userExists.RoleID == Convert.ToInt64(ReadConfig.GetValue("RoleEmployee")))
                                {
                                }
                                else if (userExists.RoleID == Convert.ToInt64(ReadConfig.GetValue("RolePM")))
                                {
                                }
                                else if (userExists.RoleID == Convert.ToInt64(ReadConfig.GetValue("RoleAdmin")))
                                {
                                }
                                else if (userExists.RoleID == Convert.ToInt64(ReadConfig.GetValue("RoleFinance")))
                                {
                                }
                            }
                            else
                            {
                                ViewBag.ErrorMsg = ResourceMessage.NoRole;
                                //return View("Index");
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMsg = ResourceMessage.NotActive;
                           // return View("Index");
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMsg = ResourceMessage.InvalidUser;
                        //return View("Index");
                    }


                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog(ex);
                }
            }
           

           // return RedirectToAction("Index");
        }



        public ActionResult Logout()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                authCookie.Expires = DateTime.Now.AddDays(-1); // make it expire yesterday
                Response.Cookies.Add(authCookie); // overwrite it
            }
            Session.Clear();
            // FormsAuthentication.SignOut();
            // Send an OpenID Connect sign-out request.
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            return RedirectToAction("Index", "Login", new { Area = string.Empty });
        }

    }

}