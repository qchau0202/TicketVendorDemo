using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ticket_vendor_demo.Models
{
    public class Ticket
    {
        public int TicketID { get; set; }
        public int PassengerID { get; set; }
        public int TrainID { get; set; }
        public string SeatNumber { get; set; }
    }
}