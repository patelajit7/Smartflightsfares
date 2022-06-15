using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
   public class Searches:ContentBase
    {
        public string Origin { get; set; }
        public string OriginAirportCityCode { get; set; }
        public string OriginCountryCode { get; set; }
        public string Destination { get; set; }
        public string DestinationAirportCityCode { get; set; }
        public string DestinationCountryCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int AdultCount { get; set; }
        public int SeniorCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantSeatCount { get; set; }
        public int InfantLapCount { get; set; }
        public int TripType { get; set; }
        public int Cabin { get; set; }
        public string PreferredCarrier { get; set; }
        public bool IsDirectFlight { get; set; }
        public string ClientIp { get; set; }
        public string UTMSource { get; set; }
        public string UTMCampaign { get; set; }
        public string FlightGuid { get; set; }
        public string AppServer { get; set; }
       public string UTMAffiliate { get; set; }
    }
}
