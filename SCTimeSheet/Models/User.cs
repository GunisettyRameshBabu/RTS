using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCTimeSheet.Models
{
    public class User
    {
        //[DuplicateEmailValidator]
        //[EmailAddress(ErrorMessage ="Please enter vali email")]
        //[Required(ErrorMessage = "Please Enter Email")]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Display(Name = "User ID")]
        public long UserID { get; set; }

        [Required(ErrorMessage ="Please Select Role")]
        
        [Display(Name = "Role Name")]
        public long RoleID { get; set; }

        
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Employee Code")]
        public long EmployeeID { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage ="Please enter Employee Code")]
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }

        [Display(Name = "Employee Name")]
        public string Name { get { return SCTimeSheet.Models.Common.GetName(FirstName,LastName,MiddleName); } }

       // [Required(ErrorMessage = "Please enter Employee Fist Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Please enter Employee Last Name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

       // [Required(ErrorMessage = "Please Select Gender")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        // [Required(ErrorMessage = "Please Select Date Of Birth")]
        [Display(Name = "Date Of Birth")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DOB { get; set; } = DateTime.Now;

       // [Required(ErrorMessage = "Please Select Nationality")]
        [Display(Name = "Nationality")]
        public long? Nationality { get; set; }

       // [Required(ErrorMessage = "Please Select Qualification")]
        [Display(Name = "Qualification")]
        public long? Qualification { get; set; }

       // [Required(ErrorMessage = "Please Select Permanent Residance")]
        [Display(Name = "Permanent Residance")]
        public long? PermanentResidance { get; set; }
    }
}