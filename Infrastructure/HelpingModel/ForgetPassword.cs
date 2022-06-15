using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.HelpingModel
{
    public class ForgetPassword
    {
        public int PortalId { get; set; }
        public string EmailId { get; set; }
    }
    public class ForgetPasswordRs
    {
        public StatusUser Status { get; set; }
        public Result Result { get; set; }
    }
}
