using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.Operations
{
    public class ContactDetail
    {
        public string Guid { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public float Price { get; set; }
        public float Markup { get; set; }
        public int Status { get; set; }
        public int TripType { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int PortalId { get; set; }
        public int AffiliateId { get; set; }
        public string IP { get; set; }
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public bool IsMobile { get; set; }

    }
}
