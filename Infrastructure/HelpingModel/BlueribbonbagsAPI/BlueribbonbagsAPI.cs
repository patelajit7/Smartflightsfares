using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.BlueribbonbagsAPI
{

     public class Product
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
    }
    public class Airlines
    {
        public int AirlineId { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string AirlineICAOCode { get; set; }
        public string AirlineIATACode { get; set; }
    }

    public class PurchaseServiceRS
    {
        public string ServiceNumber { get; set; }
        public float TotalPrice { get; set; }
    }

    public class BlueBagResponse<T>
    {
        public T Data { get; set; }
        public List<Error> Errors { get; set; }
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public List<string> Warnings { get; set; }
    }

    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetail { get; set; }
        public int ErrorLevel { get; set; }
    }



    public class PurchaseServiceRQ
    {
        public string ProductCode { get; set; }
        public bool IsInternational { get; set; }
        public string PromoCode { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public bool ReplaceServiceNumberWithCRN { get; set; }
        public string FlightDetails { get; set; }
        public string DepartureDt { get; set; }
        public string LastArrivalDt { get; set; }
        public Checkoutinfo CheckoutInfo { get; set; }
        public List<Passengerlist> PassengerList { get; set; }
    }

    public class Checkoutinfo
    {
        public string CreditCardNumber { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string CreditCardCVV { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }

    public class Passengerlist
    {
        public int OrderSequence { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string AirlineId { get; set; }
        public string AirlineConfirmationNumber { get; set; }
    }

}
