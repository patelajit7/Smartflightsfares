using Infrastructure.HelpingModel.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.Travelex
{
   public class InsuraneTravel
    {
        public string Guid { get; set; }
        public List<Traveller> Travellers { get; set; }
        public BillingDetail BillingDetails { get; set; }
        public TravelerInsurance TravelerInsurance { get; set; }
    }
}
