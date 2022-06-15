using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class Availability
    {
        /// <summary>
        /// Only use in client applivation
        /// </summary>
        public List<Contract> Contracts { get; set; }
        public AirMatrixMain AirlineMatrixMain { get; set; }
        public List<Airline> UniqueAirlineList { get; set; }
        public FilterAirports FilterAirports { get; set; }
        /// <summary>
        /// Only use in client applivation
        /// </summary>
        public ContractFacets Factes    {get;set;}
        public bool IsResuestFromDeepLink { get; set; }
    }
}
