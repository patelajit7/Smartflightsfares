using Common;
using Infrastructure.HelpingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Infrastructure.Interfaces;
namespace Business
{
    public class CurrencyManager
    {
        private static readonly object syncPoolRoot = new Object();
        private static List<Currencies> Currencies { get; set; }
        private static int SESSION_TIME_MINUTE = 60;

        public static List<Currency> GetCurrency()
        {
            List<Currency> currencies = null;
            try
            {
                lock (syncPoolRoot)
                {
                    if (Currencies != null && Currencies.Count() > 0)
                    {
                        List<Currencies> remCurrency = null;
                        foreach (Currencies item in Currencies)
                        {
                            if (item.Expiry > DateTime.UtcNow)
                            {
                                currencies = item.Currency;
                                break;
                            }
                            else
                            {
                                if (remCurrency == null)
                                {
                                    remCurrency = new List<Currencies>();
                                }
                                remCurrency.Add(item);
                            }
                        }
                        if (remCurrency != null && remCurrency.Count() > 0)
                        {
                            foreach (Currencies item in remCurrency)
                            {
                                Currencies.Remove(item);
                            }
                        }

                        if (currencies == null)
                        {
                            Currencies newCurrencies = FetchCurrencyFromDB();
                            if (newCurrencies != null)
                            {
                                if (Currencies == null)
                                {
                                    Currencies = new List<Currencies>();
                                }
                                Currencies.Add(newCurrencies);
                                currencies = newCurrencies.Currency;
                            }
                        }
                    }
                    else
                    {
                        Currencies newCurrencies = FetchCurrencyFromDB();
                        if (newCurrencies != null)
                        {
                            if (Currencies == null)
                            {
                                Currencies = new List<Currencies>();
                            }
                            Currencies.Add(newCurrencies);
                            currencies = newCurrencies.Currency;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.CurrencyManager.GetCurrency|Exception:" + ex.ToString());
            }
            return currencies;
        }

        private static Currencies FetchCurrencyFromDB()
        {
            Currencies currencies = null;
            try
            {
                List<Currency> currencyList = Utility.DatabaseService.ExecuteQuery<Currency>("CurrenciesGet", null);
                if (currencyList != null && currencyList.Count > 0)
                {
                    currencies = new Currencies() { Currency = currencyList, Expiry = DateTime.UtcNow.AddMinutes(SESSION_TIME_MINUTE) };
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.CurrencyManager.FetchCurrencyFromDB|Exception:" + ex.ToString());
            }
            return currencies;
        }
    }
}
