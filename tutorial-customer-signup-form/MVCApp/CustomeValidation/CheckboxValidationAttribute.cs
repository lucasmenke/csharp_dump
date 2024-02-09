using System.ComponentModel.DataAnnotations;

namespace MVCApp.CustomeValidation
{
    public class CheckboxValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value != null
                && value is bool
                && (bool)value;
        }
    }
}