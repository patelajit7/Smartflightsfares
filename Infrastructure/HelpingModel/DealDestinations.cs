using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class DealDestinations
    {
        public int Id { get; set; }
        public int PortalID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string DestinationName { get; set; }
        public string DestinationCode { get; set; }
        public string StateCountry { get; set; }
        public string URL { get; set; }
        public bool IsDomestic { get; set; }
        public string BannerTitle { get; set; }
        public string AboutDestination { get; set; }
        public string TopTouristAttractions { get; set; }
        public string BestSeason  { get; set; }
        public string MajorAirport { get; set; }
        public string WhyChoose { get; set; }
        public string AltTag { get; set; }
        public string DealGuid { get; set; }
        public string DealFrom { get; set; }
        public string DealTo { get; set; }
        public string Extra { get; set; }
    }
}
