using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.BookingEntities
{
    public class Transactions
    {
        public int Id { get; set; }
        public string PNR { get; set; }
        public string ReferenceNumber { get; set; }
        public int GDS { get; set; }
      
        public int ProviderId { get; set; }
        public int PortalId { get; set; }
        public int BookingType { get; set; }

        public int BookingStatus { get; set; }
        public int BookingSubStatus { get; set; }

        public int BookingSourceType { get; set; }
        public int AgentId { get; set; }
        public int AgentLead { get; set; }
        public int UserId { get; set; }
        public DateTime BookedOn { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyPrice { get; set; }
        public bool IsFailedBooking { get; set; }
        public string BookingFailedErrorMessage { get; set; }
        public Transactions()
        {
            this.BookedOn = DateTime.UtcNow;
        }
    }
}
