using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MVCApp.CustomeValidation
{
    public class MailValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(value.ToString());

            return match.Success ? true : false;
        }
    }
}