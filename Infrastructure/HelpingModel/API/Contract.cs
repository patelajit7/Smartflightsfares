using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class Contract
    {
        public string SearchGuid { get; set; }
        public int ContractId { get; set; }
        public ProviderType Provider { get; set; }
        public GDSType GDSType { get; set; }
        public string Origin { get; set; }
        public string OriginCityName { get; set; }
        public string Destination { get; set; }
        public string DestinationCityName { get; set; }
        public string OriginSearch { get; set; }
        public string DestinationSearch { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local, Representation = BsonType.String)]
        public DateTime DepartureDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local, Representation = BsonType.String)]
        public DateTime ArrivalDate { get; set; }
        public TripDetails TripDetails { get; set; }
        public Airline ValidatingCarrier { get; set; }
        public string FareType { get; set; }
        public TripType TripType { get; set; }
        public bool IsRefundable { get; set; }
        public int Adult { get; set; }
        public int Senior { get; set; }
        public int Child { get; set; }
        public int InfantOnSeat { get; set; }
        public int InfantOnLap { get; set; }
        public string FareBasisCode { get; set; }
        public FareDetails AdultFare { get; set; }
        public FareDetails ChildFare { get; set; }
        public FareDetails InfantOnSeatFare { get; set; }
        public FareDetails InfantOnLapFare { get; set; }
        public FareDetails SeniorFare { get; set; }
        public float TotalMarkup { get; set; }
        public float TotalSupplierFee { get; set; }
        public float TotalBaseFare { get; set; }
        public float TotalTax { get; set; }
        public float TotalGDSFareV2 { get; set; }
        public int EnginePriority { get; set; }
        public string Contractkey { get; set; }
        public string DatesKey { get; set; }



        public string PricingSource { get; set; }

        //public int MaxNoOfStopsInContract { get; set; }
        public int MaxStopOutbound { get; set; }
        public int MaxStopInbound { get; set; }
        public bool IsMultipleAirlineContract { get; set; }
        public int MinSeatAvailableForContract { get; set; }
        public bool IsPhoneOnly { get; set; }
        //Actual/NearBy/Flex
        public ContractType ContractType { get; set; }

        public TimeSpan TotalOutBoundFlightDuration { get; set; }
        public TimeSpan TotalInBoundFlightDuration { get; set; }
        public int AffiliateId { get; set; }
        public SignInToken AmadeusSessionToken { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public float TotalBookingFee { get; set; }
        public float FareDifference { get; set; }
        public int BaggageQuantity { get; set; }
        public bool IsGhostBooking { get; set; }
        public BaggageInformation BaggageDetails { get; set; }
        public Contract()
        {
            this.ContractType = ContractType.Actual;
        }
        public ContractTripProExtension TripProExt { get; set; }
        public ContractMystiflyExtension MystiflyExt { get; set; }

        public AmaduesSelfServiceExtension AmaduesSelfServiceExtension { get; set; }

        //only Use client App
        public int GetStopType()
        {
            int response = 0;
            if (MaxStopOutbound == 0 && MaxStopInbound == 0)
            {
                response = 0;
            }
            else 
            {
                response = 1;
            }
            
            //if (MaxStopOutbound == 0 && (MaxStopInbound == 0 || MaxStopInbound == 1))
            //{
            //    response = 0;
            //}
            //else
            //{
            //    response = 1;
            //}
            //switch (MaxNoOfStopsInContract)
            //{
            //    case 0:
            //        response = 0;
            //        break;
            //    case 1:
            //        response = 1;
            //        break;
            //    default:
            //        response = 2;
            //        break;
            //}
            return response;
        }
        
        /// <summary>
        /// Get Total  pax
        /// </summary>
        /// <returns></returns>
        public float GetTotalPax()
        {
            return (this.Adult + this.Senior + this.Child + this.InfantOnLap + this.InfantOnSeat);

        }
    }



    #region Contract Extension
    public class ContractTripProExtension
    {
        public string ItineraryId { get; set; }
    }
    public class ContractMystiflyExtension
    {
        public string BookingKey { get; set; }
        public bool IsPassportMandatory { get; set; }
        public DateTime? TktTimeLimit { get; set; }
    }
    #endregion

    #region AmaduesSelfService Extension class
    public class AmaduesSelfServiceExtension
    {
        public string Source { get; set; }
        public List<FeesExtension> Fees { get; set; }
        public List<AmaduesSelfServiceTravelerPricing> TravelerPricing { get; set; }
    }
    public class FeesExtension
    {
        public string Amount { get; set; }
        public string Type { get; set; }
    }
    public class AmaduesSelfServiceTravelerPricing
    {
        public string travelerId { get; set; }
        public string fareOption { get; set; }
        public string travelerType { get; set; }
        public AmaduesSelfServicePrice price { get; set; }
        public List<AmaduesSelfServiceFareDetailsBySegment> fareDetailsBySegment { get; set; }

    }
    public class AmaduesSelfServicePrice
    {
        public string currency { get; set; }
        public float total { get; set; }
        [JsonProperty("base")]
        public float basePrice { get; set; }
        public List<FeesExtension> fees { get; set; }
        public float grandTotal { get; set; }
    }
    public class AmaduesSelfServiceFareDetailsBySegment
    {
        public string segmentId { get; set; }
        public string cabin { get; set; }
        public string fareBasis { get; set; }
        public string @class { get; set; }
        public AmaduesSelfServiceIncludedCheckedBags includedCheckedBags { get; set; }
        public string brandedFare { get; set; }
    }
    public class AmaduesSelfServiceIncludedCheckedBags
    {
        public int quantity { get; set; }
    }
    #endregion

    public class BaggageInformation
    {
        public BaggageTripType OutboundBaggage { get; set; }
        public BaggageTripType InboundBaggage { get; set; }
    }
    public class BaggageTripType
    {
        public BaggageInfoType PesonalItem { get; set; }
        public BaggageInfoType CarryOn { get; set; }
        public BaggageInfoType Checkin { get; set; }
    }
    public class BaggageInfoType
    {
        public bool IsAllowed { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
