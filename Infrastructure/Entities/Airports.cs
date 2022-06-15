using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class Airports : ContentBase
    {
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string CityCode { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string RegionCode { get; set; }
        public int LoctionId { get; set; }
        public int Priority { get; set; }
        public int PriorityIndex { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string CurrencyCode { get; set; }
    }
}
