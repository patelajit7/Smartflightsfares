using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class RequestedItinerary
    {
        public int Id { get; set; }
        public int PortalId { get; set; }
        public string Email { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int TripType { get; set; }
        public DateTime Departure { get; set; }
        public DateTime? Return { get; set; }
        public string IP { get; set; }
        public bool SentSuccess { get; set; }
        public DateTime Created { get; set; }
    }
    public class MetaClicks
    {
        public int Id { get; set; }
        public int PortalId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int TripType { get; set; }
        public DateTime Departure { get; set; }
        public DateTime? Return { get; set; }
        public string IP { get; set; }
        public int AffiliateId { get; set; }
        public DateTime Created { get; set; }
    }
}
