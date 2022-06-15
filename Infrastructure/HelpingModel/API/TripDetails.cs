using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class TripDetails
    {
        public List<Segments> OutBoundSegment { get; set; }
        public List<Segments> InBoundSegment { get; set; }
    }
}
