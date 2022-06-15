using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class FlightSearch: SearchUIExtend
    {
        public string SearchGuidId { get; set; }
        public int PortalId { get; set; }
        public int AffiliateId { get; set; }
        public TripType TripType { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime Departure { get; set; }
        public DateTime? Return { get; set; }
        public int Adult { get; set; }
        public int Senior { get; set; }
        public int Child { get; set; }
        public int InfantOnSeat { get; set; }
        public int InfantOnLap { get; set; }
        public CabinType Cabin { get; set; }
        public string PreferredCarrier { get; set; }
        public bool IsDirectFlight { get; set; }
        public string IP { get; set; }
        public int TotalPax()
        {
            return this.Adult + this.Senior + this.Child + this.InfantOnSeat + this.InfantOnLap;
        }
        public bool IsMobileDevice { get; set; }
        public int UserId { get; set; }
        public string UserAgent { get; set; }
    }
    public class SearchUIExtend
    {
        public string OriginSearch { get; set; }
        public string DestinationSearch { get; set; }
        public string OriginAirportName { get; set; }
        public string OriginCountry { get; set; }
        public string DestAirportName { get; set; }
        public string DestCountry { get; set; }
        public bool IsMetaSearch { get; set; }
        public DateTime Created { get; set; }
        /// <summary>
        /// Tracking property
        /// </summary>
        public string UtmSource { get; set; }
        public string UtmMedium { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmTerm { get; set; }
        public string UtmContent { get; set; }
        public string UtmKeyword { get; set; }
        public string ClickedId { get; set; }

        public PageBannerType PageType { get; set; }
        public int PageId { get; set; }
        public DateTime SearchDateTime { get; set; }
        public string FlexiblityQualifier { get; set; }
    }
}
