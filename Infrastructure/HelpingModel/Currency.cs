using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{
    public class Currency
    {
        public int Id { get; set; }
        public string CurrencyType { get; set; }
        public decimal CurrencyPrice { get; set; }
        public string CurrencySymbol { get; set; }
    }
    public class Currencies
    {
        public List<Currency> Currency { get; set; }
        public DateTime Expiry { get; set; }
    }
}
