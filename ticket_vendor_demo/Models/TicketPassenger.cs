using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticket_vendor_demo.Models
{
    public class TicketPassenger
    {
        public int TicketPassengerID { get; set; }
        public int PassengerID { get; set; }
        public int TicketID { get; set; }
        public string QRCodeData { get; set; }
    }
}