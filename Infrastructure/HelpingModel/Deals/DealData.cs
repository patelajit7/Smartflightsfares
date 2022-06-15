using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.Deals
{
    public class DealData
    {
        public string Airline { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int TripType { get; set; }
        public int CabinType { get; set; }
        public int Pax { get; set; }
        public DateTime Departure { get; set; }
        public DateTime? Retun { get; set; }
        public float BaseFare { get; set; }
        public float Tax { get; set; }
        public float Markup { get; set; }
        public float BookingFees { get; set; }
        public float Total { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime SearchDateTime { get; set; }
    }
}
