using System;
using System.ComponentModel.DataAnnotations;

namespace MVCApp.CustomeValidation
{
    public class AdultValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);

            return dateTime <= DateTime.Now.AddYears(-18);
        }
    }
}