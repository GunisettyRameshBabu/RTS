using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCTimeSheet.Models
{
    public class EmailModel
    {
        [Required(ErrorMessage = "Please select from email")]
        public string From { get; set; }
        [Required(ErrorMessage = "Please select To email")]
        public List<string> To { get; set; }
        [Required(ErrorMessage = "Please enter subject")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Please enter email body")]

        [AllowHtml]
        public string Message { get; set; }

        [Display(Name = "Additional Emails")]
        public string AdditionalEmails { get; set; }
    }
}