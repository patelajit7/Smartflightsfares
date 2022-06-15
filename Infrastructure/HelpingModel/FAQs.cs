using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class FAQs 
    {
        public int Id { get; set; }
        public int PortalId { get; set; }
        public string AirlineCode { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }       
        
    }
}
