using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.BookingEntities
{
    public class TravelInsurance
    {
        public int  BookingId { get; set; }
        public string PolicyNumber{ get; set; }
        public string RefNumber{ get; set; }
        public string GroupNumber{ get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PPaxPrice { get; set; }
        public decimal ProcessingFee { get; set; }
        public decimal Commission { get; set; }
        public string Warnings { get; set; }
    }
}
