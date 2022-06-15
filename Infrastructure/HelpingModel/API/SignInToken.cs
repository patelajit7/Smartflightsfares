using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class SignInToken
    {
        public string SessionId { get; set; }
        public int SeqNumber { get; set; }
        public string Token { get; set; }
        public DateTime SessionTime { get; set; }
        public SignInToken()
        {
            this.SessionTime = DateTime.Now.AddMinutes(14);
        }
        public bool IsSessionAvailable
        {
            get
            {
                return this.SessionTime > DateTime.Now ? true : false;
            }
        }
    }
}
