using MVCApp.CustomeValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MVCApp.Models
{
    public class Customer
    {
        [Required]
        public string Salutation { get; set; }

        public string Title { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Forname { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }

        [Required]
        public string Region { get; set; }
   
        [Required]
        [CountryValidation(ErrorMessage = "Enter a valide country")]
        public string Country { get; set; }

        [Required]
        // [DataType(DataType.EmailAddress)] isn't a good validation
        [MailValidation(ErrorMessage = "Enter a valide e-mail")]
        public string Mail { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Smartphone { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [AdultValidation(ErrorMessage = "Min. age is 18")]
        public DateTime Birthday { get; set; }

        [Required]
        [CheckboxValidation(ErrorMessage = "You must agree to our terms & conditions")]
        public bool CheckBoxTermsAndConditions { get; set; }

        [Display(Name = "Signature")]
        // required attribute is unnecessary because an empty signature pad 
        // has still a dataUri, so SignatureBase64String has always a value
        [SignatureValidation(ErrorMessage = "Please sign the form")]
        public string SignatureBase64String { get; set; }
    }
}