using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.BookingEntities
{
    public class Travellers
    {
        public int TransactionId { get; set; }
        public int PaxOrderId { get; set; }
        public int PaxType { get; set; }
        public int Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string Email { get; set; }
        public string TicketsNo { get; set; }

    }
}
