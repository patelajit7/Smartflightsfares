using Common;
using System;
using System.Net;
using System.Net.Http;
using Configration;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Infrastructure.HelpingModel;
using Newtonsoft.Json;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.BlueribbonbagsAPI;
using System.Collections.Generic;
using Infrastructure.HelpingModel.Deals;
using Infrastructure.HelpingModel.Travelex;
using System.IO;
using System.Text;

namespace Business
{
    public class RESTClient
    {
        public static async Task<Response<Availability>> GetAvailability(FlightSearch _request)
        {
            Response<Availability> responseWrp = null;
            try
            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    using (var client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                        client.BaseAddress = new Uri(Utility.Settings.TravelAPI.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelAPI.AuthoriseToken.Header, Utility.Settings.TravelAPI.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelAPI.SearchRestClientTimeOut);
                        HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync(Utility.Settings.TravelAPI.SearchAction, _request);
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            responseWrp = await httpResponseMessage.Content.ReadAsAsync<Response<Availability>>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.CallRestService|Exception:" + ex.ToString());
            }

            return responseWrp;
        }

        public static async Task<Response<Contract>> CheckAvailability(Contract _contract)
        {
            Response<Contract> responseWrp = null;
            try
            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    using (var client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                        client.BaseAddress = new Uri(Utility.Settings.TravelAPI.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelAPI.AuthoriseToken.Header, Utility.Settings.TravelAPI.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelAPI.CheckAvailRestClientTimeOut);
                        HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync(Utility.Settings.TravelAPI.CheckAvailabilityAction, _contract);
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            responseWrp = await httpResponseMessage.Content.ReadAsAsync<Response<Contract>>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.CheckAvailability|Exception:" + ex.ToString());
            }

            return responseWrp;
        }


        public static Response<FlightBookingRS> BookFlight(BookingDetail _bookingDetail)
        {
            Response<FlightBookingRS> responseWrp = null;
            try
            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        client.BaseAddress = new Uri(Utility.Settings.TravelAPI.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelAPI.AuthoriseToken.Header, Utility.Settings.TravelAPI.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelAPI.BookingRestClientTimeOut);

                        HttpResponseMessage response = client.PostAsJsonAsync(Utility.Settings.TravelAPI.BookingAction, _bookingDetail).Result;

                        if (response!=null && response.IsSuccessStatusCode)
                        {
                            responseWrp = response.Content.ReadAsAsync<Response<FlightBookingRS>>().Result;
                        }
                        else
                        {
                            string booking = JsonConvert.SerializeObject(_bookingDetail)+"|"+ JsonConvert.SerializeObject(response);
                            Utility.Logger.Info(string.Format("STATUS|BookFlight|RQ:{0}", booking));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string booking = JsonConvert.SerializeObject(_bookingDetail);
                Utility.Logger.Error(string.Format("RESTClient.BookFlight|RQ:{0}|Exception:{1}", booking ,ex.ToString()));
            }
            return responseWrp;
        }
        public static BlueBagResponse<PurchaseServiceRS> BlubagPurchaseService(PurchaseServiceRQ _purchaseService)
        {
            BlueBagResponse<PurchaseServiceRS> responseWrp = null;
            try

            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        HttpResponseMessage response = null;
                        if (Utility.Settings.TravelBagAPI.IsLiveEnvironment)
                        {
                            client.BaseAddress = new Uri(Utility.Settings.TravelBagAPI.ProductionEnvironent.ApiPath);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelBagAPI.ProductionEnvironent.RequestHeaderReferrer);
                            client.DefaultRequestHeaders.Add(Utility.Settings.TravelBagAPI.ProductionEnvironent.AuthoriseToken.Header, Utility.Settings.TravelBagAPI.ProductionEnvironent.AuthoriseToken.Value);
                            client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelBagAPI.ProductionEnvironent.RestClientTimeOut);
                            response = client.PostAsJsonAsync(Utility.Settings.TravelBagAPI.ProductionEnvironent.PurchaseService, _purchaseService).Result;
                        }
                        else
                        {
                            client.BaseAddress = new Uri(Utility.Settings.TravelBagAPI.TestEnvironent.ApiPath);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelBagAPI.TestEnvironent.RequestHeaderReferrer);
                            client.DefaultRequestHeaders.Add(Utility.Settings.TravelBagAPI.TestEnvironent.AuthoriseToken.Header, Utility.Settings.TravelBagAPI.TestEnvironent.AuthoriseToken.Value);
                            client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelBagAPI.TestEnvironent.RestClientTimeOut);
                            response = client.PostAsJsonAsync(Utility.Settings.TravelBagAPI.TestEnvironent.PurchaseService, _purchaseService).Result;
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            responseWrp = response.Content.ReadAsAsync<BlueBagResponse<PurchaseServiceRS>>().Result;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.BlubagPurchaseService|Exception:" + ex.ToString());
            }
            return responseWrp;
        }

        public static HomeDeals GetDeals(DealRQ _dealRequest)
        {
            HomeDeals responseWrp = null;
            try
            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        client.BaseAddress = new Uri(Utility.Settings.TravelAPI.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelAPI.AuthoriseToken.Header, Utility.Settings.TravelAPI.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelAPI.SearchRestClientTimeOut);

                        HttpResponseMessage response = client.PostAsJsonAsync(Utility.Settings.DealAPI.DMSAction, _dealRequest).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            responseWrp = response.Content.ReadAsAsync<HomeDeals>().Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.GetDeals|Exception:" + ex.ToString());
            }
            return responseWrp;
        }

        public static RatesRS TravelexGetRate(string _purchaseService)
        {
            RatesRS responseWrp = null;
            try

            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        HttpResponseMessage response = null;

                        client.BaseAddress = new Uri(Utility.Settings.TravelexInsuranceAPI.TestEnvironent.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelBagAPI.TestEnvironent.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelBagAPI.TestEnvironent.AuthoriseToken.Header, Utility.Settings.TravelBagAPI.TestEnvironent.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelBagAPI.TestEnvironent.RestClientTimeOut);
                        //response = client.PostAsJsonAsync(Utility.Settings.TravelBagAPI.TestEnvironent.PurchaseService, _purchaseService).Result;
                        string endPoint = string.Format("{0}?{1}", Utility.Settings.TravelexInsuranceAPI.TestEnvironent.GetRate, _purchaseService);
                        response = client.GetAsync(endPoint).Result;

                        if (response.IsSuccessStatusCode)
                        {

                            string xml = response.Content.ReadAsStringAsync().Result;
                            byte[] byteArray = Encoding.UTF8.GetBytes(xml);
                            MemoryStream stream = new MemoryStream(byteArray);
                            StreamReader reader = new StreamReader(stream);
                            responseWrp = Utility.GetFileDeserialize<RatesRS>(reader);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.TravelexGetRate|Exception:" + ex.ToString());
            }
            return responseWrp;
        }
        public static PayLinkRS TravelexGetPaymentConfiguration(string _purchaseService)
        {
            PayLinkRS responseWrp = null;
            try

            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        HttpResponseMessage response = null;

                        client.BaseAddress = new Uri(Utility.Settings.TravelexInsuranceAPI.TestEnvironent.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelBagAPI.TestEnvironent.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelBagAPI.TestEnvironent.AuthoriseToken.Header, Utility.Settings.TravelBagAPI.TestEnvironent.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelBagAPI.TestEnvironent.RestClientTimeOut);
                        //response = client.PostAsJsonAsync(Utility.Settings.TravelBagAPI.TestEnvironent.PurchaseService, _purchaseService).Result;
                        string endPoint = string.Format("{0}?{1}", Utility.Settings.TravelexInsuranceAPI.TestEnvironent.GetPaymentConfiguration, _purchaseService);
                        response = client.GetAsync(endPoint).Result;

                        if (response.IsSuccessStatusCode)
                        {

                            string xml = response.Content.ReadAsStringAsync().Result;
                            byte[] byteArray = Encoding.UTF8.GetBytes(xml);
                            MemoryStream stream = new MemoryStream(byteArray);
                            StreamReader reader = new StreamReader(stream);
                            responseWrp = Utility.GetFileDeserialize<PayLinkRS>(reader);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.TravelexGetPaymentConfiguration|Exception:" + ex.ToString());
            }
            return responseWrp;
        }
        public static RatesRS TravelexCreatePolicy(string _purchaseService)
        {
            RatesRS responseWrp = null;
            try

            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        HttpResponseMessage response = null;

                        client.BaseAddress = new Uri(Utility.Settings.TravelexInsuranceAPI.TestEnvironent.ApiPath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.TravelBagAPI.TestEnvironent.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.TravelBagAPI.TestEnvironent.AuthoriseToken.Header, Utility.Settings.TravelBagAPI.TestEnvironent.AuthoriseToken.Value);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.TravelBagAPI.TestEnvironent.RestClientTimeOut);
                        //response = client.PostAsJsonAsync(Utility.Settings.TravelBagAPI.TestEnvironent.PurchaseService, _purchaseService).Result;
                        string endPoint = string.Format("{0}?{1}", Utility.Settings.TravelexInsuranceAPI.TestEnvironent.CreatePolicy, _purchaseService);
                        response = client.GetAsync(endPoint).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string xml = response.Content.ReadAsStringAsync().Result;
                            byte[] byteArray = Encoding.UTF8.GetBytes(xml);
                            MemoryStream stream = new MemoryStream(byteArray);
                            StreamReader reader = new StreamReader(stream);
                            responseWrp = Utility.GetFileDeserialize<RatesRS>(reader);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.TravelexCreatePolicy|Exception:" + ex.ToString());
            }
            return responseWrp;
        }
        public static PromoCodeResponse GetPromocodeDetails(PromoCodeRequest _promoRequest)
        {
            PromoCodeResponse responseWrp = null;
            try
            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        client.BaseAddress = new Uri(Utility.Settings.UserProfileAPI.UserProfileURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.DealAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.DealAPI.AuthoriseToken.Header, Utility.Settings.UserProfileAPI.UserProfileAuthoriseToken);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.DealAPI.RestClientTimeOut);
                        HttpResponseMessage response = client.PostAsJsonAsync("/reward/v1/coupon-detail", _promoRequest).Result;                      
                        if (response.IsSuccessStatusCode)
                        {
                            responseWrp = response.Content.ReadAsAsync<PromoCodeResponse>().Result; ;// response.Content.ReadAsAsync<Response<PromoCodeResponse>>().Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.GetPromocodeDetails|Exception:" + ex.ToString());
            }
            return responseWrp;
           
        }
        public static void ApplyPromocodeDetails(PromoCodeRequest _promoRequest)
        {
            dynamic responseWrp = null;
            try
            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                   
                    using (var client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        client.BaseAddress = new Uri(Utility.Settings.UserProfileAPI.UserProfileURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.DealAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.DealAPI.AuthoriseToken.Header, Utility.Settings.UserProfileAPI.UserProfileAuthoriseToken);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.DealAPI.RestClientTimeOut);
                        HttpResponseMessage response = client.PostAsJsonAsync("/reward/v1/apply-coupon", _promoRequest).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            responseWrp = response.Content.ReadAsAsync<dynamic>().Result; ;// response.Content.ReadAsAsync<Response<PromoCodeResponse>>().Result;
                            Utility.Logger.Error("RESTClient.ApplyPromocodeDetails|Response:" + responseWrp.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.GetDeals|Exception:" + ex.ToString());
            }
            

        }
        public static string RemovePromocodeDetails(PromoCodeRequest _promoRequest)
        {
            string responseWrp = "";
            try
            {
                using (var handler = new WebRequestHandler())
                {
                    handler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        client.BaseAddress = new Uri(Utility.Settings.UserProfileAPI.UserProfileURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Referrer = new Uri(Utility.Settings.DealAPI.RequestHeaderReferrer);
                        client.DefaultRequestHeaders.Add(Utility.Settings.DealAPI.AuthoriseToken.Header, Utility.Settings.UserProfileAPI.UserProfileAuthoriseToken);
                        client.Timeout = new TimeSpan(0, 0, Utility.Settings.DealAPI.RestClientTimeOut);
                        HttpResponseMessage response = client.PostAsJsonAsync("/reward/v1/remove-coupon", _promoRequest).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            responseWrp = response.Content.ReadAsStringAsync().Result;// response.Content.ReadAsAsync<Response<PromoCodeResponse>>().Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RESTClient.RemovePromocodeDetails|Exception:" + ex.ToString());
            }
            return responseWrp;
        }
    }
}
