using Infrastructure.HelpingModel.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class PromoCodeResponse
    {
        public Statuspromo Status { get; set; }
        public float Result { get; set; }
    }
    public class Statuspromo
    {
        public bool IsSuccess { get; set; }
        public Error Error { get; set; }
    }
}
