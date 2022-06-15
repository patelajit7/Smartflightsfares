using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class BookingDetail
    {
        public Transaction Transaction { get; set; }
        public FlightSearch FlightSearch { get; set; }
        public Contract Contract { get; set; }
        public BillingDetail BillingDetails { get; set; }
        public List<Traveller> Travellers { get; set; }
        /// <summary>
        /// in case price increase and booking status BookWithHigherPriceChanged
        /// </summary>
        public float PriceIncrease { get; set; }
        public BagInsuranc BagInsuranc { get; set; }
        public TravelerInsurance TravelerInsurance { get; set; }
        public Coupon CouponDetails { get; set; }
        public ExtendedCancellation ExtendedCancellation { get; set; }
        public CurrencyType Currency { get; set; }
        public float CurrencyConversion { get; set; }
        public string CurrencyCode { get; set; }

    }
    public class Coupon
    {
        public string CouponCode { get; set; }
        public string CouponMessage { get; set; }
        public bool Status { get; set; }
        public float TotalAmount { get; set; }
    }
    public class BagInsuranc
    {
        public BagInsuranceType BagInsuranceType { get; set; }
        public decimal PPaxPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class TravelerInsurance
    {
        public bool IsTravelProtected { get; set; }
        public decimal PPaxPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string CardType { get; set; }
        public bool Error { get; set; }
        public string Warning { get; set; }

    }
    public class ExtendedCancellation
    {
        public bool IsExtendedCancellation { get; set; }
        public decimal PPaxPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
