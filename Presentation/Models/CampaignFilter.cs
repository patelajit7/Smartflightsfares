using System.Web;
using System.Web.Mvc;
using Common;
using Infrastructure.HelpingModel;

namespace Presentation.Models
{
    public class CampaignFilter: ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            CampaignMasters campaign = null;//CamapignInfo.GetCampaign(filterContext.HttpContext.Request);
            if (campaign != null)
            {
                filterContext.Controller.ViewBag.TollFreeNumber = campaign.TollFreeNumber;
                filterContext.Controller.ViewBag.AffiliateId = campaign.AffiliateId;
            }
            else
            {
                filterContext.Controller.ViewBag.TollFreeNumber = Utility.PortalSettings.PortalDetails.DefaultTollFreeNumber;
                filterContext.Controller.ViewBag.AffiliateId = Utility.PortalSettings.PortalId;
            }
            base.OnActionExecuted(filterContext);
        }

        
    }
}