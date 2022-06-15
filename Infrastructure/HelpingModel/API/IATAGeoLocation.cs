using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class IATAGeoLocation
    {
        public string IATACode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
