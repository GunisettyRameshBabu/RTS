using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SCTimeSheet.Models;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class UsersController : BaseController
    {
        // private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<User> res = (from x in DB.User
                              join y in DB.Employee
                              on x.UserID equals y.UserID
                              join r in DB.Role on x.RoleID equals r.RoleID
                              select new User()
                              {
                                  Email = x.Email,
                                  UserID = x.UserID,
                                  RoleID = r.RoleID,
                                  RoleName = r.RoleName,
                                  EmployeeID = y.EmployeeID,
                                  IsActive = x.IsActive,
                                  EmployeeCode = y.EmpCode.Trim(),
                                  FirstName = y.EmpFirstName,
                                  LastName = y.EmpLastName,
                                  MiddleName = y.EmpMiddleName,
                                  Gender = y.EmpGender,
                                  DOB = y.EmpDOB.Value,
                                  Nationality = y.Nationality.Value,
                                  PermanentResidance = y.PermenantResidence,
                                  Qualification = y.Qualification.Value
                              }).ToList();
            return Json(res.ToDataSourceResult(request));
        }
        public JsonResult GetRoles()
        {
            List<RoleModel> res = DB.Role.ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMasterData(int type)
        {
            List<MasterDataModel> res = DB.MasterData.Where(x => x.MstTypeID == type).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountries()
        {
            List<Country> res = DB.Countries.Where(x => x.IsActive).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmployee(string empCode)
        {
            EmployeeModel res = DB.Employee.Where(x => x.EmpCode == empCode).FirstOrDefault();
            if (res != null)
            {
                if (DB.User.Any(x => x.Email == res.Email))
                {
                    return Json(new { data = "User Already Exist , Please try another employee", status = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { data = res, status = HttpStatusCode.OK }, JsonRequestBehavior.AllowGet);
            }

            var result = new { data = "Employee Not Found", status = HttpStatusCode.BadRequest };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DB.Dispose();
            }
            base.Dispose(disposing);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Users_Create([DataSourceRequest] DataSourceRequest request, User product)
        {
            if (product != null && ModelState.IsValid)
            {
                EmployeeModel emp = DB.Employee.FirstOrDefault(x => x.EmpCode == product.EmployeeCode);
                if (emp != null)
                {
                    //db.User.Add(product);
                    UserModel user = new UserModel
                    {
                        CreatedBy = (long)Session[Constants.SessionEmpID],
                        CreatedDate = DateTime.Now,
                        Email = emp.Email,
                        IsActive = true,
                        Password = "123",
                        RoleID = product.RoleID
                    };
                    DB.User.Add(user);
                    DB.Entry(user).State = System.Data.Entity.EntityState.Added;
                    DB.SaveChanges();


                    emp.UserID = user.UserID;
                    DB.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                    DB.SaveChanges();
                    var usr = new User()
                    {
                        IsActive = user.IsActive,
                        RoleID = user.RoleID,
                        UserID = user.UserID,
                        Email = user.Email,
                        EmployeeCode = emp.EmpCode,
                        EmployeeID = emp.EmployeeID,
                        FirstName = emp.EmpFirstName,
                        LastName = emp.EmpLastName,
                        MiddleName = emp.EmpMiddleName,
                        RoleName = DB.Role.FirstOrDefault(x => x.RoleID == user.RoleID)?.RoleName
                    };
                    return Json(new[] { usr }.ToDataSourceResult(request, ModelState));
                }
                //else
                //{
                //    emp.UserID = user.UserID;
                //    DB.Employee.Attach(emp);
                //    DB.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                //}

            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Users_Update([DataSourceRequest] DataSourceRequest request, User user)
        {
            if (user != null && ModelState.IsValid)
            {
                UserModel usr = DB.User.FirstOrDefault(x => x.UserID == user.UserID);
                usr.ModifiedDate = DateTime.Now;
                usr.ModifiedBy = (long)Session[Constants.SessionEmpID];
                usr.RoleID = user.RoleID;
                DB.User.Attach(usr);
                DB.Entry(usr).State = EntityState.Modified;
                DB.SaveChanges();

                user = (from x in DB.User
                                  join y in DB.Employee
                                  on x.UserID equals y.UserID
                                  join r in DB.Role on x.RoleID equals r.RoleID
                                  where x.UserID == user.UserID
                                  select new User()
                                  {
                                      Email = x.Email,
                                      UserID = x.UserID,
                                      RoleID = r.RoleID,
                                      RoleName = r.RoleName,
                                      EmployeeID = y.EmployeeID,
                                      IsActive = x.IsActive,
                                      EmployeeCode = y.EmpCode,
                                      FirstName = y.EmpFirstName,
                                      LastName = y.EmpLastName,
                                      MiddleName = y.EmpMiddleName,
                                      Gender = y.EmpGender,
                                      DOB = y.EmpDOB.Value,
                                      Nationality = y.Nationality.Value,
                                      PermanentResidance = y.PermenantResidence,
                                      Qualification = y.Qualification.Value
                                  }).FirstOrDefault();
            }
            return Json(new[] { user }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Users_Destroy([DataSourceRequest] DataSourceRequest request, User product)
        {
            if (product != null)
            {
                // productService.Destroy(product);
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }
    }


}
