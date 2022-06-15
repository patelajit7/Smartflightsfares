using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Business;
using Infrastructure.HelpingModel;
using Infrastructure;

namespace Presentation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Utility.LoadApplicationConfigartion(HttpContext.Current);
            BundleTable.EnableOptimizations = Utility.Settings.EnableBundling;
            if (Utility.Settings.IncompletBookingScheduleEnable)
            {
                SchedulerBusiness.Start();
            }
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string absolutePath = HttpContext.Current.Request.Url.AbsolutePath.Trim();
            string lowercaseURL = (Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + absolutePath);
            if (Regex.IsMatch(lowercaseURL, @"[A-Z]"))
            {
                PermanentRedirect(lowercaseURL.ToLower() + HttpContext.Current.Request.Url.Query);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception lastError = Server.GetLastError();
                if(lastError!=null)
                {
                    HttpContext context = HttpContext.Current;
                    string error = lastError.ToString();
                    string referrer = context.Request.UrlReferrer != null ? HttpContext.Current.Request.UrlReferrer.ToString() : "unknown";
                    if (!string.IsNullOrEmpty(referrer) && !referrer.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                    {
                        Utility.Logger.Error(string.Format("APPLICATION ERROR LAST APP ERROR|Message:{0}|Referrer:{1}", error, referrer));
                    }
                    else
                    {
                        Utility.Logger.Error("APPLICATION_ERROR LAST ERROR:" + error);
                    }

                    if (HttpContext.Current.IsCustomErrorEnabled)
                        ShowCustomErrorPage(lastError);
                }
               
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(ex.ToString());
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    Utility.Logger.Error(ex.Message);
                }
                if (ex.InnerException != null)
                {
                    Utility.Logger.Error(ex.InnerException.ToString());
                }
            }


        }
        private void ShowCustomErrorPage(Exception exception)
        {
            try
            {
                bool isLogError = false;
                HttpException httpException = exception as HttpException;
                if (httpException == null)
                {
                    httpException = new HttpException(500, "Internal Server Error", exception);
                }
                if (httpException.Message != null)
                {
                    isLogError = true;
                    Utility.Logger.Error("MESSAGE:" + httpException.Message);
                }
                if (!isLogError && httpException.InnerException != null)
                {
                    Utility.Logger.Error("INNER EXCEPTION:" + httpException.InnerException);
                }
                Response.Clear();
                switch (httpException.GetHttpCode())
                {
                    case 403:
                        Server.ClearError();
                        Response.Redirect("/access-denied");
                        break;
                    case 404:
                        Server.ClearError();
                        Response.Redirect("/404-page-not-found");
                        break;
                    case 500:
                        Server.ClearError();
                        Response.Redirect("/500-internal-server-error");
                        break;
                    default:
                        Server.ClearError();
                        Response.Redirect(string.Format("error/{0}", httpException.GetHttpCode()));
                        break;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("ShowCustomeErrorPage|" + ex.ToString());
            }
        }
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("X-Powered-By");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("Server");
        }
        private void PermanentRedirect(string url)
        {
            Response.Clear();
            Response.Status = "301 Moved Permanently";
            Response.AddHeader("Location", url);
            Response.End();
        }
        private void TemporarilyRedirect(string url)
        {
            Response.Clear();
            Response.Status = "302 Moved Temporarily";
            Response.AddHeader("Location", url);
            Response.End();
        }
        void Application_End(object sender, EventArgs e)
        {
            try
            {
                BookingInformation.SaveFlightSearches(true);
                IncompletBookingBusiness.SaveIncompletBookingsInDB();
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Global.Application_End" + ex.ToString());
            }
        }
    }
}
