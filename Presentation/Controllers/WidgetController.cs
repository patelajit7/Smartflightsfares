using Business;
using Common;
using Infrastructure.Entities;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class WidgetController : Controller
    {
        
        public ActionResult Index()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            try
            {
                string ip = Utility.GetClientIP(System.Web.HttpContext.Current);
                IATAGeoLocation iATAGeoLocation = null;
                List<Task> lstTasks = new List<Task>
                        {
                            Task.Factory.StartNew(()=>{ iATAGeoLocation = Business.GeoLocation.GetAirportCode(ip); })
                        };
                Task.WaitAll(lstTasks.ToArray());
                string fromCode = iATAGeoLocation.IATACode;
                var cityAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(fromCode, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.PriorityIndex).FirstOrDefault();
                if (cityAirport != null)
                {
                    string sug = string.Format("{0} - {1}, {2}, {3}", cityAirport.AirportCode, cityAirport.AirportName, cityAirport.City, cityAirport.CountryName);
                    model.OriginSearch = sug;
                    model.Origin = cityAirport.AirportCode;
                    model.OriginAirportName = cityAirport.AirportName;
                    model.OriginCountry = cityAirport.CountryCode;
                }
            }
            catch {  }
            return View(model);
        }
    }
}