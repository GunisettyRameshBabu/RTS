using SCTimeSheet_DAL;
using SCTimeSheet_HELPER;
using SCTimeSheet_UTIL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SCTimeSheet.Models
{
    public class Email
    {
        private static ApplicationDBContext DB { get; set; }

        public Email()
        {
            DB = new ApplicationDBContext();
        }
        public static async Task<bool> SendTimeSubmissionEmail(TimeSheetSubmissionEmailModel emailModel)
        {
            if (DB == null)
            {
                DB = new ApplicationDBContext();
            }
            bool result = false;
            try
            {
                var templateCode = ConfigurationManager.AppSettings["TSEmailTemplateCode"].ToString();
                SCTimeSheet_DAL.Models.EmailTemplates emailTemplate = DB.EmailTemplates.FirstOrDefault(x => x.EmailTemplateCode == templateCode);
                if (emailTemplate != null)
                {
                    MailMessage mail = new MailMessage();
                    foreach (var item in emailModel.ManagerInfo)
                    {
                        mail.To.Add(new MailAddress(item.Email));
                    }
                    string _mailId = ReadConfig.GetValue("SystemEmail");
                    string _mailPassword = ReadConfig.GetValue("MailPassword");
                    int _mailPort = Convert.ToInt32(ReadConfig.GetValue("MailPort"));
                    string _mailHost = ReadConfig.GetValue("MailHost");
                    bool _enableSsl = Convert.ToBoolean(ReadConfig.GetValue("EnableSsl"));
                    mail.From = new MailAddress(_mailId);
                    mail.CC.Add(new MailAddress(_mailId));
                    mail.Subject = emailTemplate.EmailSubject;
                    string Body = string.Format(emailTemplate.EmailBody, string.Join("/", emailModel.ManagerInfo.Select(x => Models.Common.GetName(x.EmpFirstName,x.EmpLastName , x.EmpMiddleName))), emailModel.EmpName, emailModel.SubmissionDates, emailModel.ProjectName);
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        NetworkCredential credential = new NetworkCredential
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
                    result = true;
                } else
                {
                    throw new Exception("Time Sheet Submission Email Template not found");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex);
                return result;
            }
            return result;
        }
    }

    public class TimeSheetSubmissionEmailModel
    {
        public TimeSheetSubmissionEmailModel()
        {
            ManagerInfo = new List<dynamic>();
        }

        public string EmpName { get; set; }

        public List<dynamic> ManagerInfo { get; set; }

        public string SubmissionDates { get; set; }

        public string ProjectName { get; set; }
    }
}