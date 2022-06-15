using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel;
using Business;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Presentation.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Infrastructure.Entities;
using Configration;

namespace Presentation.Controllers
{
    [CampaignFilter]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            FlightSearch model = Utility.GetDefaultSearch();     

            return Utility.GetDeviceType(Request.UserAgent) ? View("~/views/home/mobile/index.cshtml", model) : View(model);
        }

        [Route("about-us")]
        public ActionResult AboutUs()
        {
            return Utility.GetDeviceType(Request.UserAgent) ? View("~/views/home/mobile/aboutus.cshtml") : View();
        }
        [Route("contact-us")]
        public ActionResult ContactUs()
        {
            return Utility.GetDeviceType(Request.UserAgent) ? View("~/views/home/mobile/contactus.cshtml") : View();
        }

        [Route("privacy-policy")]
        public ActionResult privacypolicy()
        {
            return Utility.GetDeviceType(Request.UserAgent) ? View("~/views/home/mobile/privacypolicy.cshtml") : View();
        }
        [Route("website-terms-of-use")]
        public ActionResult websiteterm()
        {
            return Utility.GetDeviceType(Request.UserAgent) ? View() : View();
        }
        [Route("payment-security")]
        public ActionResult paymentsecurity()
        {
            return Utility.GetDeviceType(Request.UserAgent) ? View("~/views/home/mobile/paymentsecurity.cshtml") : View();
        }
        [Route("terms-condition")]
        public ActionResult termcondition()
        {
            return Utility.GetDeviceType(Request.UserAgent) ? View("~/views/home/mobile/termcondition.cshtml") : View();
        }
    }
}