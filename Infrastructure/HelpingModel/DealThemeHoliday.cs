using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
   public class DealThemeHoliday
    {
        public int Id { get; set; }
        public int PortalID { get; set; }
        public int DealThemeType { get; set;}
        public string Name { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Body { get; set; }
        public string AltTag { get; set; }
        public string DealGuid { get; set; }
        public string DealFrom { get; set; }
        public string DealTo { get; set; }
    }
}
