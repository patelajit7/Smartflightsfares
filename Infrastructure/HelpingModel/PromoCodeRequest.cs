using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
   public class PromoCodeRequest
    {
        public int PortalId { get; set; }
        public string CouponCode { get; set; }
        public int BookingId { get; set; }
        public float BookingAmount { get; set; }
    }
}
