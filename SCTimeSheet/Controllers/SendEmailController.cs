using SCTimeSheet.Common;
using SCTimeSheet.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class SendEmailController : BaseController
    {
        // GET: SendEmail
        public ActionResult Index(bool isSuccess = false)
        {
            ViewBag.ToErrorMessage = "";
            ViewBag.FromErrorMessage = "";
            ViewBag.MessageErrorMessage = "";
            ViewBag.Message = isSuccess ? "Email has been sent successfully" : "";
            return View();
        }


        public JsonResult GetUsers(string text)
        {

            if (!string.IsNullOrEmpty(text))
            {
                var emails = (from x in DB.User.Where(x => x.Email.Contains(text))
                              select new { x.Email }).ToList();

                return Json(emails, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json((from x in DB.User
                             select new { x.Email }).ToList(), JsonRequestBehavior.AllowGet);
            }




        }

        public JsonResult GetProjects(string text)
        {

            if (!string.IsNullOrEmpty(text))
            {
                var quarterDates = Models.Common.DatesOfQuarter(DateTime.Now);
                var startDate = quarterDates[0];
                var endDate = quarterDates[1];
                var emails = (from x in DB.ProjectMaster.Where(x => x.ProjectName.ToLower().Contains(text.ToLower()))
                              where (x.EndDate >= startDate && x.EndDate <= endDate ) || x.EndDate >= endDate
                              select new { x.ProjectID, x.ProjectName }).ToList();

                return Json(emails, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }




        }

        public JsonResult GetGrantType(string text)
        {

            if (!string.IsNullOrEmpty(text))
            {
                var emails = (from x in DropdownList.GrantList().Where(x => x.MstCode.ToLower().Contains(text.ToLower()))
                              select new { x.MstCode, x.MstID }).ToList();

                return Json(emails, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }




        }

        [HttpPost]
        public async Task<ActionResult> Email(EmailModel collection, HttpPostedFileBase[] files)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", collection);
                }

                if (!string.IsNullOrEmpty(collection.AdditionalEmails))
                {
                    foreach (var item in collection.AdditionalEmails.Split(','))
                    {
                        if (!Models.Common.IsEmailValid(item))
                        {
                            ModelState.AddModelError("AdditionalEmails", "Invalid additional emails , Please correct");
                            return View("Index", collection);
                        }
                    }
                }

                string _mailId = collection.From;
                //string _mailTo = collection.To;
                string _mailPassword = ReadConfig.GetValue("MailPassword");
                int _mailPort = Convert.ToInt32(ReadConfig.GetValue("MailPort"));
                string _mailHost = ReadConfig.GetValue("MailHost");
                bool _enableSsl = Convert.ToBoolean(ReadConfig.GetValue("EnableSsl"));

                var mail = new MailMessage();
                foreach (var item in collection.To)
                {
                    mail.To.Add(new MailAddress(item));
                }

                foreach (var item in collection.AdditionalEmails.Split(','))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        mail.To.Add(new MailAddress(item));

                    }
                }
                mail.From = new MailAddress(_mailId);
                mail.CC.Add(new MailAddress(_mailId));
                mail.Subject = collection.Subject;
                string Body = collection.Message;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                if (files != null && files.Any())
                {
                    foreach (var item in files)
                    {
                        mail.Attachments.Add(new Attachment(item.InputStream, item.FileName));
                    }
                }

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = _mailId,
                        Password = _mailPassword
                    };
                    smtp.Credentials = credential;
                    smtp.Host = _mailHost;
                    smtp.Port = _mailPort;
                    smtp.EnableSsl = _enableSsl;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                ViewBag.Message = "Email delivery failure, please contact system administrator.";
                return View("Index", collection);
            }

            return RedirectToAction("Index", new { isSuccess = true });
        }

        [HttpPost]
        public JsonResult GetEmails(long projectId)
        {
            var quarterDates = Models.Common.DatesOfQuarter(DateTime.Now);
            var startDate = quarterDates[0];
            var endDate = quarterDates[1];
            return Json((from x in DB.User
                         join y in DB.Employee on x.UserID equals y.UserID
                         join z in DB.ProjectEmployee on y.EmployeeID equals z.EmployeeID
                         where z.ProjectID == projectId && ((z.EndDate >= startDate && z.EndDate <= endDate) || z.EndDate >= endDate)
                         select x.Email).ToList(), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetEmailsByGrantType(long mstID)
        {
            var quarterDates = Models.Common.DatesOfQuarter(DateTime.Now);
            var startDate = quarterDates[0];
            var endDate = quarterDates[1];
            var result = (from x in DB.User
                          join y in DB.Employee on x.UserID equals y.UserID
                          join z in DB.ProjectEmployee on y.EmployeeID equals z.EmployeeID
                          join p in DB.ProjectMaster on z.ProjectID equals p.ProjectID
                          where z.ProjectGrant == mstID && ((z.EndDate >= startDate && z.EndDate <= endDate) || z.EndDate >= endDate)
                          && ((p.EndDate >= startDate && p.EndDate <= endDate) || p.EndDate >= endDate)
                          select new { x.Email, z.ProjectID, p.ProjectName }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult RemoveGrantType(long mstID, List<long> selectedProjects)
        {
            List<string> list = new List<string>();
            var mstProjectList = new List<long>();
            var quarterDates = Models.Common.DatesOfQuarter(DateTime.Now);
            var startDate = quarterDates[0];
            var endDate = quarterDates[1];
            if (selectedProjects != null)
            {
                mstProjectList = (from x in DB.ProjectMaster
                                  join y in selectedProjects on x.ProjectID equals y
                                  where x.ProjectGrant == mstID && ((x.EndDate >= startDate && x.EndDate <= endDate) || x.EndDate >= endDate)
                                  select x.ProjectID).ToList();

                selectedProjects = selectedProjects.Where(x => !mstProjectList.Contains(x)).ToList();

                list = (from x in DB.User
                        join y in DB.Employee on x.UserID equals y.UserID
                        join z in DB.ProjectEmployee on y.EmployeeID equals z.EmployeeID
                        where selectedProjects.Contains(z.ProjectID) && ((z.EndDate >= startDate && z.EndDate <= endDate) || z.EndDate >= endDate)
                        select x.Email).ToList();


            }
            var result = (from x in DB.User
                          join y in DB.Employee on x.UserID equals y.UserID
                          join z in DB.ProjectEmployee on y.EmployeeID equals z.EmployeeID
                          join p in DB.ProjectMaster on z.ProjectID equals p.ProjectID
                          where z.ProjectGrant == mstID && ((z.EndDate >= startDate && z.EndDate <= endDate) || z.EndDate >= endDate) && ((p.EndDate >= startDate && p.EndDate <= endDate) || p.EndDate >= endDate) &&
                          !list.Contains(x.Email)
                          select new { x.Email, z.ProjectID, p.ProjectName }).ToList();



            return Json(new { data = result, projects = mstProjectList }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult RemoveProjectEmails(long projectId, List<long> selectedProjects)
        {
            List<string> list = new List<string>();
            var quarterDates = Models.Common.DatesOfQuarter(DateTime.Now);
            var startDate = quarterDates[0];
            var endDate = quarterDates[1];
            if (selectedProjects != null)
            {
                selectedProjects = selectedProjects.Where(x => x != projectId).ToList();
                list = (from x in DB.User
                        join y in DB.Employee on x.UserID equals y.UserID
                        join z in DB.ProjectEmployee on y.EmployeeID equals z.EmployeeID
                        where selectedProjects.Contains(z.ProjectID) && ((z.EndDate >= startDate && z.EndDate <= endDate) || z.EndDate >= endDate)
                        select x.Email).ToList();
            }
            var result = (from x in DB.User
                          join y in DB.Employee on x.UserID equals y.UserID
                          join z in DB.ProjectEmployee on y.EmployeeID equals z.EmployeeID
                          join p in DB.ProjectMaster on z.ProjectID equals p.ProjectID
                          where z.ProjectID == projectId && ((z.EndDate >= startDate && z.EndDate <= endDate) || z.EndDate >= endDate) 
                          && ((p.EndDate >= startDate && p.EndDate <= endDate) || p.EndDate >= endDate) &&
                          !list.Contains(x.Email)
                          select new { x.Email, p.ProjectGrant }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}
