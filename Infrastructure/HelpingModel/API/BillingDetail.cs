using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class BillingDetail
    {
        [DataType(DataType.Text)]
        public string CCHolderName { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string CardNumber { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string CVVNumber { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public int CardType { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.EmailAddress)]
        public string EmailConfirm { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        [DataType(DataType.Text)]
        public string StateName { get; set; }
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string BillingPhone { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string ContactPhone { get; set; }
        public bool IsPrimaryCard { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string AreaCode { get; set; }
        public string CountryCode { get; set; }
    }
}
