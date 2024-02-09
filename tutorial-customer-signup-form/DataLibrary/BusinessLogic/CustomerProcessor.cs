using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.DataAccess;
using DataLibrary.Models;

namespace DataLibrary.BusinessLogic
{
    public static class CustomerProcessor
    {
        // gets all required data as parameters and converts them to a sql insert query
        // methode gets called by the controller of the MVC Project
        public static int CreateCustomer(string salutation, string surname, string forname,
            string street, string zip, string region, string country, string mail, DateTime birthday, string title = "", string phone = "", string smartphone = "")
        {
            Customer customer = new Customer
            {
                Salutation = salutation,
                Title = title,
                Surname = surname,
                Forname = forname,
                Street = street,                    
                Zip = zip,
                Region = region,
                Country = country,
                Mail = mail,
                Phone = phone,
                Smartphone = smartphone,
                Birthday = birthday,                 
            };

            string sql = @"insert into dbo.Customer (Salutation, Title, Surname, Forname, Street, Zip, Region, Country, Mail, Phone, Smartphone, Birthday)
                            values (@Salutation, @Title, @Surname, @Forname, @Street, @Zip, @Region, @Country, @Mail, @Phone, @Smartphone, @Birthday);";

            return SqlDataAccess.SaveData(sql, customer);
        }

        // method returns a list of all customers
        // gets called by a controller of the MVC Project
        public static List<Customer> LoadCustomers()
        {
            string sql = "select * from dbo.Customer;";

            return SqlDataAccess.LoadData<Customer>(sql);
        }
    }
}
