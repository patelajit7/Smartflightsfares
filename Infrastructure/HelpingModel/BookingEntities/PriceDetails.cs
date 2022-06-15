using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.BookingEntities
{
    public class PriceDetails
    {
        public int TransactionId { get; set; }
        public string FareBaseCode { get; set; }
        public int PaxType { get; set; }
        public int Currency { get; set; }
        public int PaxCount { get; set; }
        public decimal BaseFare { get; set; }
        public decimal Tax { get; set; }
        public decimal Markup { get; set; }
        public decimal SupplierFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public bool IsSellInsurance { get; set; }
        public decimal InsuranceAmount { get; set; }
        public bool IsSellBaggageInsurance { get; set; }
        public decimal BaggageInsuranceAmount { get; set; }
        public bool IsExtendedCancellation { get; set; }
        public decimal ExtendedCancellationAmount { get; set; }
        public decimal BookingFee { get; set; }
        public decimal GetTotal
        {
            get
            {
                return (this.BaseFare + this.Tax + this.Markup + this.SupplierFee + (this.IsSellInsurance ? this.InsuranceAmount : 0)  + (this.IsSellBaggageInsurance ? this.BaggageInsuranceAmount : 0) + (this.IsExtendedCancellation ? this.ExtendedCancellationAmount : 0) + this.BookingFee - this.Discount) * this.PaxCount;
            }
        }
    }
}
