using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Vidly.Models
{
    public class Min18YearsIfAMember: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           var customer = (Customer)validationContext.ObjectInstance;

            if(customer.MembershipTypeId == MembershipType.PayAsYouGo || customer.MembershipTypeId==MembershipType.Unknown)
            {
                return ValidationResult.Success;
            }
            if(customer.Birthdate == null)
            {
                return new ValidationResult("Birthdate is required.");
            }

            var age = DateTime.Today.Year - customer.Birthdate.Value.Year;

            return (age > 18) ? ValidationResult.Success : new ValidationResult("Customer should be 18 or above age to subscribe for a membership.");
        }
    }
}