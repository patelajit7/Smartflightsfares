using Business;
using Common;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class ShareUtility
    {
        /// <summary>
        /// Render View To String
        /// </summary>
        /// <param name="context">ControllerContext</param>
        /// <param name="viewName">string</param>
        /// <param name="model">object</param>
        /// <returns>string</returns>
        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            try
            {
                if (string.IsNullOrEmpty(viewName))
                    viewName = context.RouteData.GetRequiredString("action");

                var viewData = new ViewDataDictionary(model);

                using (var sw = new System.IO.StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                    var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("SHARE-UTILITY:" + viewName + "|MODEL:" + JsonConvert.SerializeObject(model));
                Utility.Logger.Error("Exception: " + ex.ToString(), "Presentation.Controllers", "SharedUtility", "RenderViewToString");
                var sw = new System.IO.StringWriter();
                return sw.GetStringBuilder().ToString();
            }
        }
        public static FlightSearch GetFlightSearch(HttpRequestBase httpReq)
        {
            FlightSearch Search = null;
            try
            {
                Search = Utility.GetAffiliateSearch(httpReq.QueryString, httpReq);
                if (Search != null)
                {
                    Search.SearchGuidId = Utility.GetGuid();
                    Search.PortalId = Utility.PortalSettings.PortalId;
                    Search.AffiliateId = Search.AffiliateId == 0 ? Utility.PortalSettings.PortalId : Search.AffiliateId;
                    Search.IsMobileDevice = Utility.GetDeviceType(httpReq.UserAgent);
                    Search.IP = Utility.GetClientIP(System.Web.HttpContext.Current);
                    CampaignMasters campaign = null;//CamapignInfo.GetCampaign(httpReq);
                    if (campaign != null)
                    {
                        Search.AffiliateId = campaign.AffiliateId;
                        Search.UtmCampaign = campaign.UtmCampaign;
                        Search.UtmMedium = campaign.UtmMedium;
                        Search.UtmSource = campaign.UtmSource;
                        Search.UtmTerm = campaign.UtmTerm;
                        Search.UtmContent = campaign.UtmContent;
                        Search.UtmKeyword = campaign.UtmKeyword;
                        Search.ClickedId = campaign.ClickedId;
                    }
                    AirContext context = new AirContext()
                    {
                        IsRequestCompleted = false,
                        Search = Search
                    };
                    Utility.SetAirContextCache(Search.SearchGuidId, context);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Presentation.Controllers.SharedUtility.GetFlightSearch |Exception: " + ex.ToString()); ;
            }
            return Search;
        }
        public static CurrencyMaster GetCurrency(HttpRequestBase _request)
        {
            string crnCookieName = "crnCookie";
            CurrencyMaster currency = new CurrencyMaster() { CurrencyPrice = 1, CurrencyType = "USD", CurrencySymbol = "$" };
            try
            {
                if (Utility.Settings.CurrencySeting.IsCurrencyEnable)
                {
                    if (_request.Cookies[crnCookieName] != null)
                    {
                        currency = JsonConvert.DeserializeObject<CurrencyMaster>(_request.Cookies[crnCookieName].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("ShareUtility.GetCurrency|Exception:" + ex.ToString());
            }
            return currency;
        }
        public static void SetCurrencyCookies(Currency _currency, string tfnCookieName, HttpRequestBase httpRequestBase)
        {
            try
            {
                string campaignStr = JsonConvert.SerializeObject(_currency);
                HttpCookie httpCookie = new HttpCookie(tfnCookieName);
                if (httpRequestBase.Cookies[tfnCookieName] != null)
                {
                    httpRequestBase.Cookies.Remove(tfnCookieName);
                    HttpContext.Current.Request.Cookies.Remove(tfnCookieName);
                }
                var cookie = new HttpCookie(tfnCookieName, campaignStr)
                {
                    Expires = DateTime.Now.AddYears(1)
                };
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("ShareUtility.SetCurrencyCookies|Exception:" + ex.ToString());
            }
        }
        public static string GetCurrencySymbol(string code)
        {
            string response = "$";
            try
            {
                List<Currency> currencies = CurrencyManager.GetCurrency();
                if (currencies != null && currencies.Count > 0)
                {
                    Currency currency = currencies.Where(o => o.CurrencyType.Equals(code)).FirstOrDefault();
                    if (currency != null)
                    {
                        response = currency.CurrencySymbol;
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("ShareUtility.GetCurrencySymbol|Exception:", ex.ToString()));

            }
            return response;


        }
        public static string GetCurrencyCodeFromCookies(HttpRequestBase _request)
        {
            string response = "USD";
            try
            {
                string crnCookieName = "crnCookie";

                if (_request.Cookies[crnCookieName] != null && !string.IsNullOrEmpty(_request.Cookies[crnCookieName].Value))
                {
                   CurrencyMaster currency = JsonConvert.DeserializeObject<CurrencyMaster>(_request.Cookies[crnCookieName].Value);
                    if (currency != null)
                    {
                        response = currency.CurrencyType;
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("ShareUtility.GetCurrencyCodeFromCookies|Exception:" + ex.ToString());
            }
            return response;
        }
    }
}