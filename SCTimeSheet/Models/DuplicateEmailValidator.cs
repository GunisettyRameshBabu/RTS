using SCTimeSheet_DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCTimeSheet.Models
{

    public class DuplicateEmailValidator : ValidationAttribute , IClientValidatable
    {
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule mvr = new ModelClientValidationRule();
            mvr.ErrorMessage = "User Already Exists , Please Chose another user";
            mvr.ValidationType = "duplicateemailvalidator";
            return new[] { mvr };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();
                ApplicationDBContext db = new ApplicationDBContext();
                if (db.User.Any(x => x.Email.ToLower() == email.ToLower()))
                {
                    return new ValidationResult("User already exists");
                }
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("" + validationContext.DisplayName + " is required");
            }
        }
    }
}