using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.BookingEntities
{
    public class Flights
    {
        public int TransactionId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public int TripType { get; set; }
        public DateTime DeptDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int TotalPaxCount { get; set; }
        public int AdultCount { get; set; }
        public int SeniorCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public int InfantLapCount { get; set; }
        public Int64 OutBoundFlightDuration { get; set; }
        public Int64 InBoundFlightDuration { get; set; }
        public string FareType { get; set; }
        public int ContractType { get; set; }
        public bool IsDomestic { get; set; }
    }
}
