using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class KeyValueData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsSelected { get; set; }
        public float MinPrice { get; set; }
    }
}
