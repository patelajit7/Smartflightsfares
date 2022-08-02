using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class Airline
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsLowcost { get; set; }
        public float MinPrice { get; set; }
        public bool IsMultiAirline { get; set; }
    }
    public class Airport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsLowcost { get; set; }
        public float MinPrice { get; set; }
    }
    public class FilterAirports
    {
        public List<Airport> DepartAirports { get; set; }
        public List<Airport> ReturnAirports { get; set; }
    }
}
