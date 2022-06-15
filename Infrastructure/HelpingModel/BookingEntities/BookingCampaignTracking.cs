using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.BookingEntities
{
   public class BookingCampaignTracking
    {
        public int BookingId { get; set; }
        public int AffiliateId { get; set; }
        public string SearchGuid { get; set; }
        public string UtmSource { get; set; }
        public string UtmMedium { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmTerm { get; set; }
        public string UtmContent { get; set; }
        public string UtmKeyword { get; set; }
        public string ClickedId { get; set; }
        public string UtmPublisher { get; set; }
        public string UtmPublisherId { get; set; }
        public string UtmChannelId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int TripType { get; set; }
        public bool IsMobile { get; set; }
    }
}
