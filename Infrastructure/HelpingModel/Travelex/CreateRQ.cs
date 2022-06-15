using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.Travelex
{
    public class CreateRQ
    {
        public int BookingId { get; set; }
        public DateTime Depart { get; set; }
        public DateTime? Return { get; set; }
        public int NoTravel { get; set; }
        public string Countery { get; set; }
        public string State { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string AreaCode { get; set; }
        public string CountryCode { get; set; }
        public string BillingPhone { get; set; }
        public string Email { get; set; }
        public string CCHolderName { get; set; }
        public string cardType { get; set; }
        public string InsuTokenNo { get; set; }
        public string CardNumber { get; set; }
        public int CardExpiryMonth { get; set; }
        public int CardExpiryYear { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PolicyTotalPrice{ get; set; }
        public decimal PolicyPPaxPrice { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public List<CreateTravlerDetailRQ> Travelers { get; set; }
        }

    public class CreateTravlerDetailRQ
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public decimal Fare { get; set; }
    }
    public class CreateBillingDetails
    {
        public int TransactionId { get; set; }
        public string CCHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CVVNumber { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public int CardType { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string BillingPhone { get; set; }
        public string ContactPhone { get; set; }
        public bool IsPrimaryCard { get; set; }
    }

    public class TravelexAirlines
    {
        public string AirlineName { get; set; }
        public string Code { get; set; }
    }
}
