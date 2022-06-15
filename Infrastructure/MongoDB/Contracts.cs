using System;
using Infrastructure.HelpingModel.API;
namespace Infrastructure.MongoDB
{
    public class Contracts: MongoEntity
    {
        public string Guid { get; set; }
        public Availability Availability { get; set; }
        public FlightSearch Search { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
