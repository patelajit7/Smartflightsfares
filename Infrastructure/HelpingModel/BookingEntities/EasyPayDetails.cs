using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.BookingEntities
{
    public class EasyPayDetails
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int AgentId { get; set; }
        public int PortalId { get; set; }
        public string ClientIP { get; set; }
        public string Doc { get; set; }
        public string CIN { get; set; }
        public string UserID { get; set; }
        public int PayStatus { get; set; }
        public string SubMerchantId { get; set; }        
        public decimal PayValue { get; set; }
        public DateTime? Paydate { get; set; }
        public string PaymentType { get; set; }
        public decimal ValueFixed { get; set; }
        public decimal ValueVar { get; set; }
        public decimal ValueTax { get; set; }
        public decimal ValueTransf { get; set; }
        public DateTime? DateTransf { get; set; }
        public bool? IsMailSent { get; set; }
        public bool? isReceiptSent { get; set; }
        public long TransactionId { get; set; }
        public DateTime Created { get; set; }  
        public string Url { get; set; }
        public DateTime CacheExpiryTime { get; set; }
        public string UserEmail { get; set; }
    }
}
