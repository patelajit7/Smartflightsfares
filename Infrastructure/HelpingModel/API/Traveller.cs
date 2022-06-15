using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class Traveller
    {
        public int PaxOrderId { get; set; }
        public int PaxType { get; set; }
        public int Title { get; set; }
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [DataType(DataType.Text)]
        public string MiddleName { get; set; }
        [DataType(DataType.Text)]
        public string LastName { get; set; }
        public int Gender { get; set; }
        [DataType(DataType.PhoneNumber)]
        public int? DOBDay { get; set; }
        public int DOBMonth { get; set; }
        [DataType(DataType.PhoneNumber)]
        public int? DOBYear { get; set; }
        public string PassportNumber { get; set; }
        public string PassportIssuingCountry { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
    }
}
