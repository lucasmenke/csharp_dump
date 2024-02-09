using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Salutation { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string Forname { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Smartphone { get; set; }
        public DateTime Birthday { get; set; }
    }
}
