using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class ContractFacets
    {
        public FlightSearch Search { get; set; }
        public int TotalContract { get; set; }
        public TripType TripType { get; set; }
        public float MaxPrice{get;set;}
        public float MinPrice{get;set;}

        public float ActualMinPrice { get; set; }
        public float NearbyMinPrice { get; set; }
        public float AlternameMinPrice { get; set; }
        public float PhoneOnlyMinPrice { get; set; }
        public DepartureArrival Origin { get; set; }
        public DepartureArrivalTime OutboundDepTime { get; set; }
        public DepartureArrivalTime OutboundArrTime { get; set; }
        public DepartureReturnDuration OutboundDuration { get; set; }

        public DepartureArrival Return { get; set; }
        public DepartureArrivalTime InboundDepTime { get; set; }
        public DepartureArrivalTime InboundArrTime { get; set; }
        public DepartureReturnDuration InboundDuration { get; set; }

        public List<KeyValueData> Stops{get;set;}
        public ContractFacets(){
            this.Stops = new List<KeyValueData>();
        }
    }

    public class DepartureArrival
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
    public class DepartureArrivalTime
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class DepartureReturnDuration
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }
    
}
