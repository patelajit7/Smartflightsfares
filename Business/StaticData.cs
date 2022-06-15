using Common;
using Configration;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Business
{
   public class StaticData
    {
        public static List<SelectListItem> GetCabin()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Economy", Value = CabinType.Economy.ToString() });
            items.Add(new SelectListItem { Text = "Premium Economy", Value = CabinType.PremiumEconomy.ToString() });
            items.Add(new SelectListItem { Text = "Business", Value = CabinType.Business.ToString() });
            items.Add(new SelectListItem { Text = "First", Value = CabinType.First.ToString() });
            return items;
        }
        public static List<SelectListItem> GetSearchEngineCabins(int _cabin = 2)
       {
           List<SelectListItem> items = new List<SelectListItem>();
           items.Add(new SelectListItem { Text = "Economy", Value = ((int)CabinType.EconomyCoach).ToString(), Selected = _cabin == (int)CabinType.EconomyCoach });
           items.Add(new SelectListItem { Text = "Business", Value = ((int)CabinType.Business).ToString(), Selected = _cabin == (int)CabinType.Business });
           items.Add(new SelectListItem { Text = "First", Value = ((int)CabinType.First).ToString(), Selected = _cabin == (int)CabinType.First });
           return items;
       }
       public static List<SelectListItem> GetTitles(int _title = 0)
       {
           List<SelectListItem> items = new List<SelectListItem>();
           items.Add(new SelectListItem { Text = "Title", Value = "0", Selected = _title == (int)TravellerTitleType.None });
           items.Add(new SelectListItem { Text = "Mr.", Value = ((int)TravellerTitleType.MR).ToString(), Selected = _title == (int)TravellerTitleType.MR });
           items.Add(new SelectListItem { Text = "Ms.", Value = ((int)TravellerTitleType.MS).ToString(), Selected = _title == (int)TravellerTitleType.MS });
           items.Add(new SelectListItem { Text = "Mrs.", Value = ((int)TravellerTitleType.MRS).ToString(), Selected = _title == (int)TravellerTitleType.MRS });

           return items;
       }
       public static List<SelectListItem> GetGenders(int _gender = 0)
       {
           List<SelectListItem> items = new List<SelectListItem>();
           items.Add(new SelectListItem { Text = "Gender", Value = "0", Selected = _gender == 0 });
           items.Add(new SelectListItem { Text = "Male", Value = "1", Selected = _gender == 1 });
           items.Add(new SelectListItem { Text = "Female", Value = "2", Selected = _gender == 2 });
           return items;
       }
       public static List<SelectListItem> GetMonth(int _month = 0)
       {
           List<SelectListItem> items = new List<SelectListItem>();
           items.Add(new SelectListItem { Text = "Month", Value = "0", Selected = _month == 0 });
           items.Add(new SelectListItem { Text = "JAN", Value = "1", Selected = _month == 1 });
           items.Add(new SelectListItem { Text = "FEB", Value = "2", Selected = _month == 2 });
           items.Add(new SelectListItem { Text = "MAR", Value = "3", Selected = _month == 3 });
           items.Add(new SelectListItem { Text = "APR", Value = "4", Selected = _month == 4 });
           items.Add(new SelectListItem { Text = "MAY", Value = "5", Selected = _month == 5 });
           items.Add(new SelectListItem { Text = "JUN", Value = "6", Selected = _month == 6 });
           items.Add(new SelectListItem { Text = "JUL", Value = "7", Selected = _month == 7 });
           items.Add(new SelectListItem { Text = "AUG", Value = "8", Selected = _month == 8 });
           items.Add(new SelectListItem { Text = "SEP", Value = "9", Selected = _month == 9 });
           items.Add(new SelectListItem { Text = "OCT", Value = "10", Selected = _month == 10 });
           items.Add(new SelectListItem { Text = "NOV", Value = "11", Selected = _month == 11 });
           items.Add(new SelectListItem { Text = "DEC", Value = "12", Selected = _month == 12 });
           return items;
       }
       public static List<SelectListItem> GetPaymentMethod(int _cardType = 0)
       {
           List<SelectListItem> items = new List<SelectListItem>();
           items.Add(new SelectListItem { Text = "Select payment method", Value = "0", Selected = _cardType == (int)PaymentMethod.None });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.Visa), Value = ((int)PaymentMethod.Visa).ToString(), Selected = _cardType == (int)PaymentMethod.Visa });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.MasterCard), Value = ((int)PaymentMethod.MasterCard).ToString(), Selected = _cardType == (int)PaymentMethod.MasterCard });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.AmericanExpress), Value = ((int)PaymentMethod.AmericanExpress).ToString(), Selected = _cardType == (int)PaymentMethod.AmericanExpress });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.DinersClub), Value = ((int)PaymentMethod.DinersClub).ToString(), Selected = _cardType == (int)PaymentMethod.DinersClub });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.Discover), Value = ((int)PaymentMethod.Discover).ToString(), Selected = _cardType == (int)PaymentMethod.Discover });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.Electron), Value = ((int)PaymentMethod.Electron).ToString(), Selected = _cardType == (int)PaymentMethod.Electron });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.Maestro), Value = ((int)PaymentMethod.Maestro).ToString(), Selected = _cardType == (int)PaymentMethod.Maestro });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.BCCard), Value = ((int)PaymentMethod.BCCard).ToString(), Selected = _cardType == (int)PaymentMethod.BCCard });
           items.Add(new SelectListItem { Text = Utility.GetEnumDescription(PaymentMethod.JCB), Value = ((int)PaymentMethod.JCB).ToString(), Selected = _cardType == (int)PaymentMethod.JCB });

           return items;
       }
       public static List<SelectListItem> GetCreditCardExpityYear()
       {
           List<SelectListItem> items = new List<SelectListItem>();
           items.Add(new SelectListItem { Text = "Year", Value = "0", Selected = true });
           int year = DateTime.Now.Year;
           for (int i = year; i <= year + 18; i++)
           {
               items.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

           }
           return items;
       }

       public static List<SelectListItem> GetCountries()
       {
           List<SelectListItem> items = new List<SelectListItem>();

           foreach (Country item in Utility.Settings.Country)
           {
               if (item.Code.Equals("US", StringComparison.OrdinalIgnoreCase))
               {
                   items.Add(new SelectListItem { Text = item.Name, Value = item.Code, Selected = true });
               }
               else
               {
                   items.Add(new SelectListItem { Text = item.Name, Value = item.Code });
               }

           }

           return items;
       }
       public static List<SelectListItem> GetStates(string _countryCode)
       {
           List<SelectListItem> items = new List<SelectListItem>();
           List<State> states = Utility.GetStates(_countryCode);
           foreach (State item in states)
           {
               items.Add(new SelectListItem { Text = item.Name, Value = item.Code });
           }
           return items;
       }
    }
}
