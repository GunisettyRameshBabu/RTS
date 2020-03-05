using SCTimeSheet_DAL;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class UserAccess
    {
        public static Dictionary<string, string> UserAccessMatrix;
        public static string sessionPageAccess = Constants.SessionPageAccess;

        static UserAccess()
        {
            UserAccessMatrix = new Dictionary<string, string>();

            #region Add Action to Dictionary

            ApplicationDBContext DB = new ApplicationDBContext();
            System.Data.Entity.DbSet<PageModel> list = DB.Page;

            foreach (PageModel item in list)
            {
                if (!UserAccessMatrix.ContainsKey(item.PageName))
                {
                    UserAccessMatrix.Add((item.PageName).ToLower() + "|" + Constants.Access, item.PageName); // Role
                }
            }

            #endregion
        }

        public bool AuthorizationCheck(Controller pController, long userId, string controllerName, [CallerMemberName]string memberName = "")
        {
            try
            {
                controllerName = controllerName.ToLower();
                HttpSessionStateBase session = pController.Session;

#if ACCESSTEST
        session[sessionPageAccess]= null;
#endif

                //if (controllerName == "login")
                session[sessionPageAccess] = null;

                if (session[sessionPageAccess] == null && pController.User.Identity.IsAuthenticated)
                {
                    SetupRolePermission(session, userId);
                }

                if (controllerName != "")
                {
                    List<string> rolesList = session[sessionPageAccess] as List<string>;
                    string key = string.Format("{0}|{1}", controllerName, Constants.Access);
                    if (UserAccessMatrix.ContainsKey(key))
                    {
                        if (rolesList.Contains(key))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
            }
            return false;
        }

        private void SetupRolePermission(HttpSessionStateBase pSession, long userId)
        {
            using (ApplicationDBContext db = new ApplicationDBContext())
            {
                List<string> rolesList = new List<string>();

                List<PageModel> moduleList = db.Page.ToList();

                var roles = db.User.Select(x => new { x.RoleID, x.UserID }).Where(x => x.UserID == userId).FirstOrDefault();
                if (roles != null)
                {
                    List<PageMappingList> pages = db.PageMapping.Join(db.Page, x => x.PageID, y => y.PageID, (x, y) => new PageMappingList { PageID = x.PageID, RoleID = x.RoleID, IsActive = x.IsActive, PageName = y.PageName }).Where(x => x.RoleID == roles.RoleID && x.IsActive).ToList();
                    if (pages != null)
                    {
                        foreach (PageMappingList page in pages)
                        {
                            if (!rolesList.Contains(string.Format("{0}|" + Constants.Access, page.PageName.ToLower())))
                            {
                                rolesList.Add(string.Format("{0}|" + Constants.Access, page.PageName.ToLower()));
                            }
                        }
                    }
                }

                pSession[sessionPageAccess] = rolesList;

                //foreach (PageModel mdl in moduleList)
                //{
                //    if (!rolesList.Contains(string.Format("{0}|" + Constants.Access, mdl.PageName.ToLower())))
                //        rolesList.Add(string.Format("{0}|" + Constants.Access, mdl.PageName.ToLower()));

                //}


                //    List<string> rolesList = new List<string>();
                ////List<string> fnList = new List<string>();
                //try
                //{
                //    var isSystemAdmin = db.User.Where(x => x.UserId == userId && !x.DeletionFlag).FirstOrDefault().IsSystemAdmin;
                //    //var isSystemAdmin = UserHelper.IsSystemAdmin(userId);

                //    if (!isSystemAdmin)
                //    {
                //        if (!rolesList.Contains(string.Format("{0}|" + Constants.moduleActionView, "dashboard")))
                //            rolesList.Add(string.Format("{0}|" + Constants.moduleActionView, "dashboard"));

                //        var roles = db.RoleAssignment.Select(x => new { x.RoleId, x.FromDate, x.IsActive, x.UserId, ToDate = x.ToDate == null ? DateTime.Now : x.ToDate }).Where(x => x.UserId == userId && x.FromDate <= DateTime.Now && x.ToDate >= DateTime.Now && x.IsActive);
                //        // var roles = db.RoleAssignment.Where(x => x.UserId == userId && x.FromDate>=DateTime.Now && x.ToDate = (x.ToDate==null) ? DateTime.Now : x.ToDate <= DateTime.Now).ToList();
                //        foreach (var item in roles)
                //        {
                //            //var roleAuthorization = db.RoleAuthorization.Join(db.Module, x => x.ModuleId, y => y.ModuleId, (x, y) => new { x.RoleId, x.ModuleId, y.ModuleGroup, x.IsActive, x.ActionCreate, x.ActionModify, x.ActionView }).Where(x => x.IsActive && x.RoleId == item.RoleId).ToList();
                //            var roleAuthorization = db.RoleAuthorization.Join(db.Module, x => x.ModuleId, y => y.ModuleId, (x, y) => new { x.RoleId, x.ModuleId, y.ModuleGroup, x.ActionCreate, x.ActionModify, x.ActionView }).Where(x => x.RoleId == item.RoleId && (x.ActionCreate || x.ActionModify || x.ActionView)).ToList();
                //            ///change needed

                //            foreach (var roleAuth in roleAuthorization)
                //            {
                //                //if (roleAuth.ActionCreate)
                //                //{
                //                //    if (!rolesList.Contains(string.Format("{0}|" + Constants.moduleActionCreate, roleAuth.ModuleId.ToLower())))
                //                //        rolesList.Add(string.Format("{0}|" + Constants.moduleActionCreate, roleAuth.ModuleId.ToLower()));
                //                //}
                //                //if (roleAuth.ActionModify)
                //                //{
                //                //    if (!rolesList.Contains(string.Format("{0}|" + Constants.moduleActionModify, roleAuth.ModuleId.ToLower())))
                //                //        rolesList.Add(string.Format("{0}|" + Constants.moduleActionModify, roleAuth.ModuleId.ToLower()));
                //                //}
                //                if (roleAuth.ActionView)
                //                {
                //                    if (!rolesList.Contains(string.Format("{0}|" + Constants.moduleActionView, roleAuth.ModuleId.ToLower())))
                //                        rolesList.Add(string.Format("{0}|" + Constants.moduleActionView, roleAuth.ModuleId.ToLower()));
                //                }
                //                //if (roleAuth.ModuleGroup == Constants.moduleGroupPage)
                //                //{
                //                //    if (!rolesList.Contains(string.Format("{0}", roleAuth.ModuleId)))
                //                //        rolesList.Add(string.Format("{0}", roleAuth.ModuleId));
                //                //}
                //                //else if (roleAuth.ModuleGroup == Constants.moduleGroupFn)
                //                //{
                //                //    if (roleAuth.ActionCreate)
                //                //    {
                //                //        if (!fnList.Contains(string.Format("{0}|" + Constants.moduleActionCreate, roleAuth.ModuleId)))
                //                //            fnList.Add(string.Format("{0}|" + Constants.moduleActionCreate, roleAuth.ModuleId));
                //                //    }
                //                //    if (roleAuth.ActionModify)
                //                //    {
                //                //        if (!fnList.Contains(string.Format("{0}|" + Constants.moduleActionModify, roleAuth.ModuleId)))
                //                //            fnList.Add(string.Format("{0}|" + Constants.moduleActionModify, roleAuth.ModuleId));
                //                //    }
                //                //    if (roleAuth.ActionView)
                //                //    {
                //                //        if (!fnList.Contains(string.Format("{0}|" + Constants.moduleActionView, roleAuth.ModuleId)))
                //                //            fnList.Add(string.Format("{0}|" + Constants.moduleActionView, roleAuth.ModuleId));
                //                //    }
                //                //}
                //            }
                //        }
                //    }
                //    else
                //    {
                //        List<ModuleModel> moduleList = db.Module.ToList();

                //        foreach (ModuleModel mdl in moduleList)
                //        {
                //            //if (!rolesList.Contains(string.Format("{0}|" + Constants.moduleActionCreate, mdl.ModuleId.ToLower())))
                //            //    rolesList.Add(string.Format("{0}|" + Constants.moduleActionCreate, mdl.ModuleId.ToLower()));

                //            //if (!rolesList.Contains(string.Format("{0}|" + Constants.moduleActionModify, mdl.ModuleId.ToLower())))
                //            //    rolesList.Add(string.Format("{0}|" + Constants.moduleActionModify, mdl.ModuleId.ToLower()));

                //            if (!rolesList.Contains(string.Format("{0}|" + Constants.moduleActionView, mdl.ModuleId.ToLower())))
                //                rolesList.Add(string.Format("{0}|" + Constants.moduleActionView, mdl.ModuleId.ToLower()));

                //            //if (mdl.ModuleGroup == Constants.moduleGroupPage)
                //            //{
                //            //    if (!rolesList.Contains(string.Format("{0}", mdl.ModuleId)))
                //            //        rolesList.Add(string.Format("{0}", mdl.ModuleId));
                //            //}
                //            //else
                //            //{
                //            //    if (!fnList.Contains(string.Format("{0}|" + Constants.moduleActionCreate, mdl.ModuleId)))
                //            //        fnList.Add(string.Format("{0}|" + Constants.moduleActionCreate, mdl.ModuleId));

                //            //    if (!fnList.Contains(string.Format("{0}|" + Constants.moduleActionModify, mdl.ModuleId)))
                //            //        fnList.Add(string.Format("{0}|" + Constants.moduleActionModify, mdl.ModuleId));

                //            //    if (!fnList.Contains(string.Format("{0}|" + Constants.moduleActionView, mdl.ModuleId)))
                //            //        fnList.Add(string.Format("{0}|" + Constants.moduleActionView, mdl.ModuleId));
                //            //}
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    LogHelper.ErrorLog(ex);
                //}

                //pSession[sessionPageAccess] = rolesList;
                ////pSession[sessionFunctionAccess] = fnList;
            }
        }

        public static bool HasRole(HttpSessionStateBase pSession, string permission)
        {
            List<string> roleList = pSession[sessionPageAccess] as List<string>;
            if (roleList != null)
            {
                return roleList.Contains(permission);
            }
            else
            {
                return false;
            }
        }
    }
}