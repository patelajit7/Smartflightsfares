using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class PortalStaticData
    {
        public List<CampaignMasters> Campaigns { get; set; }
        public List<DealAirlines> AirlinesData { get; set; }
        public List<DealDestinations> Destinations { get; set; }
        public List<DealThemeHoliday> DealThemeHoliday { get; set; }
        public List<FAQs> FAQs { get; set; }
    }
}
