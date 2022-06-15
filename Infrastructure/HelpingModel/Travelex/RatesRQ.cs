using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.HelpingModel.Travelex
{
    public class RatesRQ
    {
        public string BookingGuid { get; set; }
        public DateTime Depart { get; set; }
        public DateTime? Return { get; set; }
        public int NoTravel { get; set; }
        public string ResidenceCountery { get; set; }
        public string ResidenceState { get; set; }
        public string CardNumber { get; set; }
        public int CardExpiryMonth { get; set; }
        public int CardExpiryYear { get; set; }
        public List<RatesTravlerDetailRQ> Travelers { get; set; }
    }

    public class RatesTravlerDetailRQ
    {
        public DateTime DOB { get; set; }
        public decimal Fare { get; set; }
    }
}


    
