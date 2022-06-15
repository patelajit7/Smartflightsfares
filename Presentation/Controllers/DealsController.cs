using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Entities;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel;
using Common;
using Business;

namespace Presentation.Controllers
{
    public class DealsController : Controller
    {
        [Route("deals/flight-under-49")]
        public ActionResult FlightUnder49()
        {            
            FlightSearch model = Utility.GetDefaultSearch();            
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            
            return isMobileDevice ? View(model) : View(model);

        }

        [Route("deals/international-flight-under-199")]
        public ActionResult InternationFlightUnder199()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View(model) : View(model);

        }

        [Route("deals/domestic-flight-under-99")]
        public ActionResult DomesticFlightUnder99()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View(model) : View(model);

        }

        [Route("deals/top-flight-deals")]
        public ActionResult TopFlightDeals()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View(model) : View(model);

        }

        [Route("deals/flight-to-india")]
        public ActionResult IndiaFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/indiaflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-amsterdam")]
        public ActionResult AmsterdamFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/amsterdamflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-aruba")]
        public ActionResult ArubaFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/arubaflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-bali")]
        public ActionResult BaliFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/baliflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-cancun")]
        public ActionResult CancunFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/cancunflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-dubai")]
        public ActionResult DubaiFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/dubaiflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-italy")]
        public ActionResult ItalyFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/italyflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-japan")]
        public ActionResult JapanFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/japanflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-london")]
        public ActionResult LondonFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/londonflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-sydney")]
        public ActionResult SydneyFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/sydneyflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-sanfrancisco")]
        public ActionResult SanfranciscoFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/sanfranciscoflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-spain")]
        public ActionResult SpainFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/deals/mobile/spainflight.cshtml", model) : View(model);

        }

        [Route("deals/flight-to-toronto")]
        public ActionResult TorontoFlight()
        {
            FlightSearch model = Utility.GetDefaultSearch();
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View(model) : View(model);

        }
       
    }

}