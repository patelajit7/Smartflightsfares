using Infrastructure.HelpingModel.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class IncompleteBookingContext
    {
        public List<IncompleteBooking> IncompleteBookings { get; set; }
        public DateTime CacheExpiryTime { get; set; }
    }
    public class IncompleteBooking
    {
        public string Key { get; set; }
        public BookingDetail BookingDetail { get; set; }
    }
}
