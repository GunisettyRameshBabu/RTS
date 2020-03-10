using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SCTimeSheet_DAL.Models;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SCTimeSheet.Controllers
{
    public class CommonMasterController : BaseController
    {
        // GET: CommonMaster
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SaveGrant(string grantName,string grantCode)
        {
            try
            {
                MasterDataModel model = new MasterDataModel();
                var existing = DB.MasterData.FirstOrDefault(x => x.MstName == grantName);
                if (existing != null)
                {
                    return Json(new { data = false });
                }
                else
                {
                    model.MstTypeID = 3;
                    model.MstCode = grantCode;
                    model.MstName = grantName;
                    model.IsActive = true;
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = (Int64)Session[Constants.SessionEmpID];
                    DB.MasterData.Add(model);
                    DB.SaveChanges();

                    //send mail
                    await SendEmail(grantName, grantCode);
                    return Json(new { data = true });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                return Json(new { data = "Error" });
            }
                 
        }

        private static async Task SendEmail(string grantName, string grantCode,bool isNew = true )
        {
            string _mailId = ReadConfig.GetValue("SystemEmail");
            string _mailTo = ReadConfig.GetValue("MailTo");
            string _mailPassword = ReadConfig.GetValue("MailPassword");
            int _mailPort = Convert.ToInt32(ReadConfig.GetValue("MailPort"));
            string _mailHost = ReadConfig.GetValue("MailHost");
            bool _enableSsl = Convert.ToBoolean(ReadConfig.GetValue("EnableSsl"));

            var mail = new MailMessage();
            foreach (var item in _mailTo.Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    mail.To.Add(new MailAddress(item));
                }
            }
            mail.From = new MailAddress(_mailId);
            mail.Subject = "Sembcorp Timesheet Grant Type Updates";
            string text = isNew ? "New Grant type added successfully." : "Grant Modified Successfully";
            string Body = $" Hi, <br/><br/> { text }  <br/>Grant Type : " + grantName + " <br/> Grand Code : " + grantCode + "<br/><br/>Regards, <br/>Admin." +
                "<br/><br/><b>---- This is a system generated mail; it does not require replying. -----</b>";
            mail.Body = Body;
            mail.IsBodyHtml = true;

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

        [HttpPost]
        public ActionResult GetGrantTypes([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                var GetDetails = DB.MasterData.Where(x => x.MstTypeID == 3 && x.IsActive==true).OrderByDescending(x => x.ModifiedDate).ToList();
                DataSourceResult result = GetDetails.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UpdateGrant([DataSourceRequest]DataSourceRequest request, FormCollection collection)
        {
            var code = collection["models[0].MstCode"];
            var id = Convert.ToInt64(collection["models[0].MstID"]);

            var MasterData = DB.MasterData.FirstOrDefault(x => x.MstID == id);
            try
            {
                
                MasterData.MstName = collection["models[0].MstName"]; //model.MstName;
                MasterData.MstCode = code;
                MasterData.ModifiedDate = DateTime.Now;
                MasterData.ModifiedBy = (Int64)Session[Constants.SessionEmpID];
                if ( ModelState.IsValid)
                {
                   
                    DB.MasterData.Attach(MasterData);
                    DB.Entry(MasterData).State = System.Data.Entity.EntityState.Modified;
                    DB.SaveChanges();
                    try
                    {
                        await SendEmail(MasterData.MstName, MasterData.MstCode, false);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.ErrorLog(ex);
                    }
                    
                    return RedirectToAction("Index");
                }
                return Json(new[] { MasterData }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                return Json(new[] { MasterData }.ToDataSourceResult(request, ModelState));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> DestroyGrant(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var code = collection["models[0].MstCode"];
                    var MasterData = DB.MasterData.FirstOrDefault(x => x.MstCode == code);
                    MasterData.IsActive = false; //model.MstName;
                    MasterData.ModifiedDate = DateTime.Now;
                    MasterData.ModifiedBy = (Int64)Session[Constants.SessionEmpID];
                    DB.MasterData.Attach(MasterData);
                    DB.Entry(MasterData).State = System.Data.Entity.EntityState.Modified;
                    DB.SaveChanges();
                    try
                    {
                        await SendEmail(MasterData.MstName, MasterData.MstCode, false);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.ErrorLog(ex);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                throw ex;
            }
        }
    }
}