using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticket_vendor_demo.Models
{
    public class Train
    {
        public int TrainID { get; set; }
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string TravelDate { get; set; }
        public string TicketType { get; set; }
        public decimal Price { get; set; }
    }
}