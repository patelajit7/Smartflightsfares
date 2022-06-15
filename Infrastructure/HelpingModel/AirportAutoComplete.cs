using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
  public class AirportAutoComplete
    {
      public bool IsMultiAirport { get; set; }
      public int TreePosition { get; set; }
      public string AutoSuggestion { get; set; }
      public string Code { get; set; }
      public string Name { get; set; }
      public string Country { get; set; }
    }
}
