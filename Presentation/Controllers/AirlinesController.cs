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
    public class AirlinesController : Controller
    {
        [Route("airlines/ha")]
        public ActionResult HawaiianAirline()
        {
            
            FlightSearch model = Utility.GetDefaultSearch();
            
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);            
            return isMobileDevice ? View("~/views/airlines/mobile/hawaiianairline.cshtml", model) : View(model);

        }
        [Route("airlines/nk")]
        public ActionResult SpiritAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/spiritairline.cshtml",  model) : View(model);

        }
        [Route("airlines/ua")]
        public ActionResult UnitedAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/unitedairline.cshtml",  model) : View(model);

        }
        [Route("airlines/aa")]
        public ActionResult AmericanAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/americanairline.cshtml",  model) : View(model);

        }
        [Route("airlines/dl")]
        public ActionResult DeltaAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/deltaairline.cshtml",  model) : View(model);

        }
        [Route("airlines/f9")]
        public ActionResult FrontierAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/frontierairline.cshtml",  model) : View(model);

        }



        [Route("airlines/ac")]
        public ActionResult AirCanadaAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/aircanadaairline.cshtml",  model) : View(model);

        }



        [Route("airlines/ws")]
        public ActionResult WestJetAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/westjetairline.cshtml",  model) : View(model);

        }

        [Route("airlines/b6")]
        public ActionResult JetBlueAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/jetblueairline.cshtml",  model) : View(model);

        }

        [Route("airlines/as")]
        public ActionResult AlaskaAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/alaskaairline.cshtml",  model) : View(model);

        }

        [Route("airlines/ek")]
        public ActionResult EmiratesAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/emiratesairline.cshtml",  model) : View(model);

        }

        [Route("airlines/wn")]
        public ActionResult SouthwestAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/southwestairline.cshtml",  model) : View(model);

        }

        [Route("airlines/tk")]
        public ActionResult TurkishAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/turkishairline.cshtml",  model) : View(model);

        }

        [Route("airlines/ey")]
        public ActionResult EtihadAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/etihadairline.cshtml",  model) : View(model);

        }

        [Route("airlines/lh")]
        public ActionResult LufthansaAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/lufthansaairline.cshtml",  model) : View(model);

        }

        [Route("airlines/am")]
        public ActionResult AeromexicoAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/aeromexicoairline.cshtml",  model) : View(model);

        }

        [Route("airlines/klm")]
        public ActionResult KlmAirline()
        {
            FlightSearch model = Utility.GetDefaultSearch();

            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? View("~/views/airlines/mobile/klmairline.cshtml", model) : View(model);

        }
    }
}