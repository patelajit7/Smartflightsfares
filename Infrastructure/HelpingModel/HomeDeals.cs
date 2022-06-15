using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.Deals;

namespace Infrastructure.HelpingModel
{
   public class HomeDeals
    {
        public Response<List<DealData>> Destination { get; set; }
        public Response<List<DealData>> Domestic { get; set; }
        public Response<List<DealData>> International { get; set; }
    }
}
