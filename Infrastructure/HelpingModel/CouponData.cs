using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class CouponData
    {
        public int Id { get; set; }
        public string CouponCode { get; set; }
        public string CouponLabel { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public bool IsDefault { get; set; }
    }
}
    
