using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class FareDetails
    {
        public int PaxCount { get; set; }
        public TravellerPaxType PaxType { get; set; }
        public string GDSPaxType { get; set; }
        public float ActualBaseFare { get; set; }
        public float BaseFare { get; set; }
        public float Tax { get; set; }
        public float TotalFare { get; set; }
        public float Markup { get; set; }
        public float SupplierFee { get; set; }
        public float Discount { get; set; }
        public bool IsSellInsurance { get; set; }
        public float InsuranceAmount { get; set; }
        public bool IsSellBaggageInsurance { get; set; }
        public float BaggageInsuranceAmount { get; set; }
        public string FareBaseCode { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public bool IsExtendedCancellation { get; set; }
        public float ExtendedCancellationAmount { get; set; }
        public float BookingFee { get; set; }
        public float TotalFareV2
        {
            get
            {
                return ((BaseFare + Markup + SupplierFee + BookingFee + Tax + (IsExtendedCancellation == true ? ExtendedCancellationAmount : 0) + (IsSellInsurance == true ? InsuranceAmount : 0) + (IsSellBaggageInsurance == true ? BaggageInsuranceAmount : 0)) - Discount) * PaxCount;
            }
        }
        public float TotalFarePPax
        {
            get
            {
                return ((BaseFare + Markup + SupplierFee + Tax + (IsExtendedCancellation == true ? ExtendedCancellationAmount : 0) + (IsSellInsurance == true ? InsuranceAmount : 0) + (IsSellBaggageInsurance == true ? BaggageInsuranceAmount : 0)) - Discount);
            }
        }

        public float BaseFarePPax
        {
            get
            {
                return BaseFare +  (IsSellInsurance == true ? InsuranceAmount : 0) + (IsExtendedCancellation == true ? ExtendedCancellationAmount : 0) + (IsSellBaggageInsurance == true ? BaggageInsuranceAmount : 0);
            }
        }
        public float TotalBaseFare
        {
            get
            {
                return (BaseFare + (IsSellInsurance == true ? InsuranceAmount : 0) + (IsExtendedCancellation == true ? ExtendedCancellationAmount : 0) + (IsSellBaggageInsurance == true ? BaggageInsuranceAmount : 0)) * PaxCount;
            }
        }
        public float TaxPPax
        {
            get
            {
                return ((Tax + Markup + SupplierFee + BookingFee));
            }
        }
        public float TotalTax
        {
            get
            {
                return ((Tax+ Markup+ SupplierFee)) * PaxCount;
            }
        }
    }
}
