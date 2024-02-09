using System.ComponentModel.DataAnnotations;

namespace MVCApp.CustomeValidation
{
    public class CountryValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string countryName = value.ToString().ToUpper();

            return countryName == "" ? false : true;
        }
    }
}