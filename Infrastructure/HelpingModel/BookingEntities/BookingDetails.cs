using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.HelpingModel.API;

namespace Infrastructure.HelpingModel.BookingEntities
{
    public class BookingDetails
    {
        public Transactions Transaction { get; set; }
        public Flights Flight { get; set; }
        public List<FlightSegments> Segments { get; set; }
        public List<PriceDetails> PriceDetail { get; set; }
        public List<Travellers> Travellers { get; set; }
        public BillingDetails BillingDetails { get; set; }
        public BaggageInsurances BaggageInsurances { get; set; }
        public TravelInsurance TravelInsurance { get; set; }
        public Coupon CouponDetails { get; set; }
    }
}
