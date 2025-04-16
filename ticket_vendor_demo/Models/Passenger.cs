using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticket_vendor_demo.Models
{
    public class Passenger
    {
        public int PassengerID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsGuest { get; set; }
    }
}