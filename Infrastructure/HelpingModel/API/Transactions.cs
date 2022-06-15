using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Guid { get; set; }
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
        public Transaction()
        {
            this.BookedOn = DateTime.UtcNow;
        }

    }
}
