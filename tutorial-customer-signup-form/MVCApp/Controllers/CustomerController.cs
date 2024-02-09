using MVCApp.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using DataLibrary.BusinessLogic;

namespace MVCApp.Controllers
{
    public class CustomerController : Controller
    {
        // shows the default page -> customer singup form
        public ActionResult SignUp()
        {
            // get all countries & iso code for the view
            List<string> countries = Country.GetAllCountries();
            if (countries.Count > 0)
            {
                this.Session["Countries"] = countries;
                ViewBag.CountryList = countries;
            }

            return View();
        }

        // displays all customers that are signed up
        public ActionResult ViewCustomers()
        {
            var data = CustomerProcessor.LoadCustomers();
            List<Customer> customers = new List<Customer>();

            foreach (var row in data)
            {
                customers.Add(new Customer
                {
                    Salutation = row.Salutation,
                    Title = row.Title,
                    Surname = row.Surname,
                    Forname = row.Forname,
                    Street = row.Street,
                    Zip = row.Zip,
                    Region = row.Region,
                    Country = row.Country,
                    Mail = row.Mail,
                    Phone = row.Phone,
                    Smartphone = row.Smartphone,
                    Birthday = row.Birthday,
                });
            }
            
            return View(customers);
        }

        // saves customer to database if the model is valid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(Customer model)
        {
            // refresh viewbag data
            ViewBag.CountryList = this.Session["Countries"];

            // SignUp.cshtml validation is client sided & can be bypassed
            // this if-statement also validates model on the server side
            if (ModelState.IsValid)
            {
                int recordsCreated = CustomerProcessor.CreateCustomer(model.Salutation, model.Surname, model.Forname, model.Street, model.Zip, model.Region, model.Country, model.Mail, model.Birthday, model.Title, model.Phone, model.Smartphone);

                return View("Success");
            }

            return View();
        }
    }
}