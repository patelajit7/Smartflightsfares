using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class FlightBookingRS
    {
    
        public int TransactionId { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public string TranGuid { get; set; }
        public string PNR { get; set; }
        public float FareDifference { get; set; }
        public Contract Contract { get; set; }
    }
}
