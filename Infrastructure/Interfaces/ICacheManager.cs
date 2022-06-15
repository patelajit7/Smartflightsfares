using Infrastructure.HelpingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ICacheManager
    {
        void Set(string _key, AirContext _airContext);
        AirContext Get(string _key);
    }
}
