using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class Segments
    {
        public int Id { get; set; }
        public bool IsReturn { get; set; }
        public bool IsDepartDateHighlight { get; set; }
        public bool IsOriginHighlight { get; set; }
        public bool IsDestinationHighlight { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
        public Airline MarketingCarrier { get; set; }
        public Airline OperatingCarrier { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public string StopOverTime { get; set; }
        public string OutTerminal { get; set; }
        public string InTerminal { get; set; }
        public string EquipmentType { get; set; }
        public string FlightNumber { get; set; }
        public string CnxType { get; set; }
        public string FareBasis { get; set; }
        public string Class { get; set; }
        public string PrevClass { get; set; }
        //GDS Cabin
        public string Cabin { get; set; }
        //WizFairTickets Cabin
        public CabinType CabinType { get; set; }
        public string Origin { get; set; }
        public string OriginCity { get; set; }
        public string Destination { get; set; }
        public string DestinationCity { get; set; }
        public TimeSpan? FlightDuration { get; set; }
        public string CompanyFranchiseDetails { get; set; }
        public int AvailableSeats { get; set; }
        public int NoOfStops { get; set; }
        public bool IsSoldOut { get; set; }
        public string AirlineLocator { get; set; }
        public string SegmentStatus { get; set; }
        // public List<TechnicalStop> TechnicalStop { get; set; }
        public SegmentTripProExtension SegmentTripProExt { get; set; }
        public SegmentAmeduesSelfServiceExtension SegmentASSExtension { get; set; }
    }

    #region Segment
    public class SegmentTripProExtension
    {
        public string BaggageAllowance { get; set; }
        public string BaggageInfoUrl { get; set; }
    }
    #endregion
    public class SegmentAmeduesSelfServiceExtension
    {
        public string SegmentId { get; set; }
        public string Number { get; set; }
        public ASSExtensionOperating ASSExtensionOperating { get; set; }
        public string CarrierCode { get; set; }
        public string AirCraftCode { get; set; }
        public int BaggageQuantity { get; set; }
    }
    public class ASSExtensionOperating
    {
        public string carrierCode { get; set; }
    }
}
