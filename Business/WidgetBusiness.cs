using Common;
using Infrastructure.HelpingModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class WidgetBusiness
    {
        public static WidgetData GetWidgetdetail(NameValueCollection queryString)
        {
            WidgetData response = null;
            try
            {
                if (queryString != null)
                {
                    string fontColor = string.Empty;
                    if (!string.IsNullOrEmpty(queryString["wfc"]))
                        fontColor = queryString["wfc"].Trim().ToLower();
                    else
                        fontColor = "fff";// TODO default Color

                    string bgColor = string.Empty;
                    if (!string.IsNullOrEmpty(queryString["wbgc"]))
                        bgColor = queryString["wbgc"].Trim().ToLower();
                    else
                        bgColor = "fae22a";// TODO default Color

                    string buttonColor = string.Empty;
                    if (!string.IsNullOrEmpty(queryString["wbtc"]))
                        buttonColor = queryString["wbtc"].Trim().ToLower();
                    else
                        buttonColor = "ff690f";// TODO default Color

                    string buttonFontColor = string.Empty;
                    if (!string.IsNullOrEmpty(queryString["wbtfc"]))
                        buttonFontColor = queryString["wbtfc"].Trim().ToLower();
                    else
                        buttonFontColor = "fff";// TODO default Color

                    string utmSource = null;
                    if (!string.IsNullOrEmpty(queryString["utm_source"]))
                        utmSource = queryString["utm_source"].Trim();

                    string utmMeduim = null;
                    if (!string.IsNullOrEmpty(queryString["utm_medium"]))
                        utmMeduim = queryString["utm_medium"].Trim();

                    bool isHotel = false;
                    bool isFlight = false;
                    if (!string.IsNullOrEmpty(queryString["wdf"]))
                    {
                        string[] wd = queryString["wdf"].Trim().Split(',');
                        if (wd.Contains("fh"))
                        {
                            isHotel = true;
                            isFlight = true;
                        }
                        if (wd.Contains("f"))
                        {
                            isFlight = true;
                        }
                        if (wd.Contains("h"))
                        {
                            isHotel = true;
                        }
                    }

                    response = new WidgetData()
                    {
                        FontColor = fontColor,
                        ButtonColor = buttonColor,
                        ButtonFontColor = buttonFontColor,
                        BGColor = bgColor,
                        UtmSource = utmSource,
                        UtmMeduim=utmMeduim,
                        IsHotel = isHotel,
                        IsFlight = isFlight
                    };
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("QUERY STRING:GetWidgetdetail:{0}|Exception:{1}", queryString != null ? queryString.ToString() : "", ex.Message));
            }
            return response;
        }
        public static void IsValidWidgetTracking(WidgetData widgetData)
        {
            if (!string.IsNullOrEmpty(widgetData.UtmSource))
            {
                widgetData.IsValid = true;
            }
            else
            {
                widgetData.IsValid = false;
            }
        }
    }
}
