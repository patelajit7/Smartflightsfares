using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Configration;
using Infrastructure.HelpingModel;

namespace Presentation.Controllers
{
    public class WebModeController : Controller
    {
        [Route("web/mode/{id}")]
        public ActionResult Index(string id)
        {
            try
            {
                string ip = Utility.GetClientIP(System.Web.HttpContext.Current);
                ViewBag.IP = ip;
                WebSiteMode mebSiteMode = Utility.PortalSettings.WebSiteMode.Where(x => x.Guid.Equals(id, StringComparison.OrdinalIgnoreCase) && x.Enabled == true && x.GetIPs().Contains(ip)).FirstOrDefault<WebSiteMode>();
                if (mebSiteMode != null)
                {
                    ViewBag.WebSiteMode = "Web site mode on successfully.";
                    Session["webmode"] = true;
                    
                }
                else
                {
                    ViewBag.WebSiteMode = "Web site mode un-able to on.";
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Web Site Mode:Exception:" + ex);
            }
            return View();
        }
    }
}
