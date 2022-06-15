using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.Deals
{
    public class DealRequest
    {
        public string Airline { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int TripType { get; set; }
        public int CabinType { get; set; }
        public string Guid { get; set; }
   }
}
