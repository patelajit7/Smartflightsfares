using Common;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class GeoLocation
    {
        public static Response<Location> GetGeoLoaction(string _ip)
        {
            Response<Location> response = null;
            Location location = null;
            try
            {
                JObject json = GetApiResponse(_ip);
                if (json != null)
                {
                    if (json.GetValue("success") != null && ((JToken)json.GetValue("success")).ToObject<bool>() == false)
                    {
                        response = new Response<Location>() { TransactionStatus = new TransactionStatus() { IsSuccess = false, Error = new Error() { Code = null, Description = "Invalid response" } } };
                    }
                    else
                    {
                        location = new Location();
                        JToken token = null;
                        token = json.GetValue("ip");
                        location.IP = token != null ? token.ToObject<String>() : null;

                        token = json.GetValue("continent_code");
                        location.ContinentCode = token != null ? token.ToObject<String>() : null;

                        token = json.GetValue("continent_name");
                        location.ContinentName = token != null ? token.ToObject<String>() : null;

                        token = json.GetValue("country_code");
                        location.CountryCode = token != null ? token.ToObject<String>() : null;

                        token = json.GetValue("country_name");
                        location.CountryName = token != null ? token.ToObject<String>() : null;

                        token = json.GetValue("latitude");
                        location.Latitude = token != null ? token.ToObject<String>() : null;

                        token = json.GetValue("longitude");
                        location.Longitude = token != null ? token.ToObject<String>() : null;
                        response = new Response<Location>()
                        {
                            Result = location,
                            TransactionStatus = new TransactionStatus() { IsSuccess = true, Error = null }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                response = new Response<Location>() { TransactionStatus = new TransactionStatus() { Error = new Error() { Code = "505", Description = "Internal Error" } } };
                Utility.Logger.Error("GeoLocation.GetGeoLoaction|EX:" + ex.ToString());
            }
            return response;
        }

        private static JObject GetApiResponse(string _ip)
        {
            string url = string.Format("{0}{1}?access_key={2}", Utility.PortalSettings.GeoLocationAPI.Domain, _ip, Utility.PortalSettings.GeoLocationAPI.APIKey);
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/json";
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            string result = string.Empty;
            result = responseStream.ReadToEnd();
            JObject response = JObject.Parse(result);
            response.Add("status", (int)webresponse.StatusCode);
            webresponse.Close();
            return response;
        }
        public static IATAGeoLocation GetAirportCode(string _ip)
        {
            IATAGeoLocation response = null;
            try
            {
                string url = string.Format("https://www.travelpayouts.com/whereami?locale=en&callback=useriata&ip={0}", _ip);
                using (var handler = new System.Net.Http.WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    using (var client = new System.Net.Http.HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                        client.BaseAddress = new Uri(Utility.Settings.TravelAPI.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelAPI.AuthoriseToken.Header, Utility.Settings.TravelAPI.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelAPI.SearchRestClientTimeOut);
                        HttpResponseMessage httpResponseMessage = client.GetAsync(url).Result;
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                            if (result != null)
                            {
                                result = result.ToString().Replace("useriata(", "").Replace(")", "");
                                IataResponse js = Newtonsoft.Json.JsonConvert.DeserializeObject<IataResponse>(result);
                                if (js != null)
                                {
                                    string[] geoloc = !string.IsNullOrEmpty(js.coordinates) ? js.coordinates.Split(':') : null;
                                    response = new IATAGeoLocation() { IATACode = js.iata };
                                    if(geoloc!=null && geoloc.Count() > 1)
                                    {
                                        response.Latitude = geoloc[1];
                                        response.Longitude = geoloc[0];
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("GeoLocation.GetAirportCode|EX:" + ex.ToString());
            }
            return response;
        }
        public static string GetCurrencyFromIP(string _ip)
        {
            string response = "USD";
            try
            {
                if (_ip.Equals("::1"))
                {
                    _ip = "103.61.253.133";
                }
                string url = string.Format("https://ipapi.co/{0}/currency", _ip);
                using (var handler = new System.Net.Http.WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    using (var client = new System.Net.Http.HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                        client.BaseAddress = new Uri(Utility.Settings.TravelAPI.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelAPI.AuthoriseToken.Header, Utility.Settings.TravelAPI.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelAPI.SearchRestClientTimeOut);
                        HttpResponseMessage httpResponseMessage = client.GetAsync(url).Result;
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrEmpty(result))
                            {
                                response = result;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("GeoLocation.GetAirportCode|EX:" + ex.ToString());
            }
            return response;
        }
    }
    public class IataResponse
    {
        public string iata { get; set; }
        public string name { get; set; }
        public string country_name { get; set; }
        public string coordinates { get; set; }
    }
}
