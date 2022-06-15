using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class ContractFilterRQ
    {
        public string Guid { get; set; }
        public int Tab { get; set; }
        public int PageNumber { get; set; }
        public List<AirlineFilter> Airlines { get; set; }
        public List<AirportFilter> DepartureAirports { get; set; }
        public List<AirportFilter> ReturnAirports { get; set; }
        public List<int> Stops { get; set; }
        public PriceFilter Price { get; set; }
        public MaxMinValue OutboundDepTime { get; set; }
        public MaxMinValue OutboundArrTime { get; set; }
        public MaxMinValue InboundDepTime { get; set; }
        public MaxMinValue InboundArrTime { get; set; }
        public MaxMinValue OutboundDuration { get; set; }
        public MaxMinValue InboundDuration { get; set; }
        public ApplyMatrixFilter ApplyMatrixFilter { get; set; }

    }
    public class AirlineFilter
    {
        public string Airline { get; set; }
        public bool IsMultiAirline { get; set; }
    }   

    public class AirportFilter
    {
        public string Airport { get; set; }
    }
    public class PriceFilter
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }
    public class MaxMinValue
    {
        public TimeSpan Min { get; set; }
        public TimeSpan Max { get; set; }
    }
    public class ApplyMatrixFilter
    {
        public List<StopContractTypeFilter> StopContractTypes { get; set; }
        public bool IsAirlineClicked { get; set; }
    }
    public class StopContractTypeFilter
    {
        public ContractType ContractType { get; set; }
       public StopsType StopsType { get; set; }
    }
}
