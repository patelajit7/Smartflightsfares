using Infrastructure.HelpingModel.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class AirContext
    {
        public bool IsSendBookingMail { get; set; }
        public bool IsRequestCompleted { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CacheExpiryTime { get; set; }
        public FlightSearch Search { get; set; }
        public Availability Availability { get; set; }
        public Contract SelectedContract { get; set; }
        public BookingDetail BookingDetailRQ { get; set; }
        public FlightBookingRS FlightBookingRS { get; set; }
        public string TSLUrl { get; set; }
        public int MetaClickId { get; set; }
        public bool IsMeta { get; set; }
    }
}
