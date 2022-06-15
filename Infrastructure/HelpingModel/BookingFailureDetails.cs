using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class BookingFailureDetails
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string BookingStatus { get; set; }
        public string OldPrice { get; set; }
        public string NewPrice { get; set; }
        public string Action { get; set; }
    }
}
