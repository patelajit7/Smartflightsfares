using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaremartPortal.Infrastructure.Entities
{
    public class FAQs : ContentBase
    {
        public int PortalId { get; set; }
        public string AirlineCode { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}
