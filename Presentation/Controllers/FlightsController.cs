using Business;
using Common;
using Infrastructure;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.BookingEntities;
using Infrastructure.HelpingModel.Operations;
using Infrastructure.HelpingModel.BlueribbonbagsAPI;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Infrastructure.HelpingModel.Travelex;
using Configration;
using Infrastructure.Entities;

namespace Presentation.Controllers
{

    [CampaignFilter]
    public class FlightsController : Controller
    {
        [HttpPost]
        [Route("flights/getbageinsuranceprice")]
        public ActionResult GetBageInsurancePrice(string id, int bagInsuranceType)
        {
            string partialView = string.Format("~/views/flights/{0}partial/_pricedetails.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");
            try
            {
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null && context.SelectedContract != null)
                    {
                        context.BookingDetailRQ = new BookingDetail()
                        {
                            Contract = context.SelectedContract,
                            BagInsuranc = Utility.GetBagInsuranceType(bagInsuranceType, (context.SelectedContract.Adult + context.SelectedContract.Child + context.SelectedContract.InfantOnLap + context.SelectedContract.InfantOnSeat + context.SelectedContract.Senior)),
                            TravelerInsurance = context.BookingDetailRQ != null && context.BookingDetailRQ.TravelerInsurance != null ? context.BookingDetailRQ.TravelerInsurance : null,
                            CouponDetails = context.BookingDetailRQ != null && context.BookingDetailRQ.CouponDetails != null ? context.BookingDetailRQ.CouponDetails : null,
                            ExtendedCancellation = context.BookingDetailRQ != null && context.BookingDetailRQ.ExtendedCancellation != null ? context.BookingDetailRQ.ExtendedCancellation : null
                        };
                        if (context.BookingDetailRQ != null && context.BookingDetailRQ.CouponDetails != null)
                        {
                            SetCoupanAmmount(context.BookingDetailRQ.CouponDetails.TotalAmount, (context.SelectedContract.Adult + context.SelectedContract.Child + context.SelectedContract.InfantOnLap + context.SelectedContract.InfantOnSeat + context.SelectedContract.Senior), context);
                        }
                        else
                        {
                            SetCoupanAmmount(0, (context.SelectedContract.Adult + context.SelectedContract.Child + context.SelectedContract.InfantOnLap + context.SelectedContract.InfantOnSeat + context.SelectedContract.Senior), context);
                        }
                        Utility.SetAirContextCache(id, context);
                        var result = new
                        {
                            IsSuccess = true,
                            HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, context.BookingDetailRQ)
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.GetBageInsurancePrice|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("flights/getextendedcancellation")]
        public ActionResult GetExtendedCancellationPrice(string id, bool isChecked)
        {
            string partialView = string.Format("~/views/flights/{0}partial/_pricedetails.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");
            try
            {
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null && context.SelectedContract != null)
                    {
                        float totalBaseTaxPrice = context.SelectedContract.TotalBaseFare + context.SelectedContract.TotalTax;
                        float extendCancelPP = totalBaseTaxPrice > Utility.Settings.TravelExtendedCancellation.MinBookingAmount ? Utility.Settings.TravelExtendedCancellation.MaxAmount : Utility.Settings.TravelExtendedCancellation.MinAmount;
                        context.BookingDetailRQ = new BookingDetail()
                        {
                            Contract = context.SelectedContract,
                            BagInsuranc = context.BookingDetailRQ != null && context.BookingDetailRQ.BagInsuranc != null ? context.BookingDetailRQ.BagInsuranc : null,
                            TravelerInsurance = context.BookingDetailRQ != null && context.BookingDetailRQ.TravelerInsurance != null ? context.BookingDetailRQ.TravelerInsurance : null,
                            CouponDetails = context.BookingDetailRQ != null && context.BookingDetailRQ.CouponDetails != null ? context.BookingDetailRQ.CouponDetails : null,
                            ExtendedCancellation = isChecked ? new ExtendedCancellation()
                            {
                                IsExtendedCancellation = true,
                                PPaxPrice = (decimal)extendCancelPP,
                                TotalPrice = (context.SelectedContract.Adult + context.SelectedContract.Child + context.SelectedContract.InfantOnLap + context.SelectedContract.InfantOnSeat + context.SelectedContract.Senior) * (decimal)extendCancelPP
                            } : null
                        };

                        Utility.SetAirContextCache(id, context);
                        var result = new
                        {
                            IsSuccess = true,
                            HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, context.BookingDetailRQ)
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.GetBageInsurancePrice|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("flights/travelprotection")]
        public ActionResult GetTravelProtectionPrice(string id, bool isChecked)
        {
            //string partialView = string.Format("~/views/flights/{0}partial/_pricedetails.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");
            try
            {
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null && context.SelectedContract != null)
                    {
                        context.BookingDetailRQ = new BookingDetail()
                        {
                            Contract = context.SelectedContract,
                            BagInsuranc = context.BookingDetailRQ != null && context.BookingDetailRQ.BagInsuranc != null ? context.BookingDetailRQ.BagInsuranc : null,
                            TravelerInsurance = isChecked ? new TravelerInsurance() { PPaxPrice = 0, TotalPrice = 0, IsTravelProtected = true } : null,
                            CouponDetails = context.BookingDetailRQ != null && context.BookingDetailRQ.CouponDetails != null ? context.BookingDetailRQ.CouponDetails : null,
                            ExtendedCancellation = context.BookingDetailRQ != null && context.BookingDetailRQ.ExtendedCancellation != null ? context.BookingDetailRQ.ExtendedCancellation : null,
                        };

                        Utility.SetAirContextCache(id, context);
                        var result = new
                        {
                            IsSuccess = true,
                            HtmlResponse = ""
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.GetBageInsurancePrice|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("flights/add-contactdata")]
        public JsonResult SaveContactData(SaveContactData _req)
        {
            ContactDetail contactDetail = null;
            AirContext context = Utility.GetAirContextCache(_req.Guid);
            if (_req != null)
            {
                DateTime depart;
                DateTime? returnDate = null;
                string from = string.Empty, to = string.Empty;
                from = context.SelectedContract.TripDetails.OutBoundSegment[0].Origin;
                depart = context.SelectedContract.TripDetails.OutBoundSegment[0].Departure;
                if (context.SelectedContract.TripType == TripType.ROUNDTRIP)
                {
                    to = context.SelectedContract.TripDetails.InBoundSegment[0].Origin;
                    returnDate = context.SelectedContract.TripDetails.InBoundSegment[0].Departure;
                }
                else
                {
                    to = context.SelectedContract.TripDetails.OutBoundSegment[context.SelectedContract.TripDetails.OutBoundSegment.Count - 1].Destination;

                }

                contactDetail = new ContactDetail()
                {
                    AreaCode = _req.AreaCode,
                    CountryCode = _req.CountryCode,
                    IsMobile = Utility.GetDeviceType(Request.UserAgent),
                    Phone = _req.Phone,
                    Guid = _req.Guid,
                    Email = _req.Email,
                    Price = context.SelectedContract.TotalGDSFareV2,
                    Markup = context.SelectedContract.TotalMarkup,
                    Status = (int)Status.Pending,
                    IP = Utility.GetClientIP(System.Web.HttpContext.Current),
                    TripType = (int)context.Search.TripType,
                    DepartureDate = depart,
                    ReturnDate = returnDate,
                    From = from,
                    To = to,
                    PortalId = context.Search.PortalId,
                    AffiliateId = context.Search.AffiliateId
                };
            }
            return Json(new { success = true });
        }
        [Route("flights/getstates")]
        public JsonResult GetStates(string _countryCode)
        {
            JsonResult result = new JsonResult();
            result.Data = Utility.GetStates(_countryCode);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        [HttpPost]
        [Route("flights/airport-suggestion")]
        public ActionResult Suggestion(string id)
        {
            List<AirportAutoComplete> results = AutoCompleteService.GetAutoSuggestions(id);

            var result = new
            {
                TransactionStatus = results != null ? true : false,
                ResultList = results
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Route("flights/airline-suggestion/{id}")]
        public ActionResult AirlineSuggestion(string id)
        {
            List<Airline> results = AutoCompleteService.GetAirlineSuggestion(id);

            var result = new
            {
                TransactionStatus = results != null ? true : false,
                ResultList = results
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchEngine(FormCollection collection, FlightSearch Search)
        {

            if (ModelState.IsValid)
            {
                //if (!string.IsNullOrEmpty(Search.SearchGuidId))
                //{
                //    AirContext contexts = Utility.GetAirContextCache(Search.SearchGuidId);
                //    if (contexts != null && Search.GetSearchKey().Equals(contexts.Search.GetSearchKey(), StringComparison.OrdinalIgnoreCase))
                //    {
                //        return Utility.GetDeviceType(Request.UserAgent) ? RedirectToAction("Availability", new { id = Search.SearchGuidId }) : RedirectToAction("Availability", new { id = Search.SearchGuidId });
                //    }
                //}
                Search.SearchGuidId = string.IsNullOrEmpty(Search.SearchGuidId) ? Utility.GetGuid() : Search.SearchGuidId;
                Search.PortalId = Utility.PortalSettings.PortalId;
                Search.AffiliateId = Search.AffiliateId <= 0 ? Utility.PortalSettings.PortalId : Search.AffiliateId;
                Search.Origin = Search.OriginSearch.Contains("|") ? Search.OriginSearch.Trim().Split('|')[1].Trim().Substring(0, 3).ToUpper() : Search.OriginSearch.Trim().Substring(0, 3).ToUpper();
                Search.Destination = Search.DestinationSearch.Contains("|") ? Search.DestinationSearch.Trim().Split('|')[1].Trim().Substring(0, 3).ToUpper() : Search.DestinationSearch.Trim().Substring(0, 3).ToUpper();
                Utility.SetOriginDestinationDetails(Search);
                Search.IsMobileDevice = Utility.GetDeviceType(Request.UserAgent);
                Search.IP = Utility.GetClientIP(System.Web.HttpContext.Current);

                try
                {
                    if (HttpContext.Request != null && HttpContext.Request.Cookies["_up"] != null)
                    {
                        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(HttpContext.Request.Cookies["_up"].Value);
                        if (data != null && data["UserId"] != null)
                        {
                            Search.UserId = data["UserId"];
                        }
                    }
                }
                catch { }
                if (collection["DirectFlight"] != null && collection["DirectFlight"].ToString().Equals("on", StringComparison.OrdinalIgnoreCase))
                {
                    Search.IsDirectFlight = true;
                }
                CampaignMasters campaign = null;//CamapignInfo.GetCampaign(Request);
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
                Task task = Task.Factory.StartNew(() =>
                {
                    new AirBusiness().SearchFlight(Search);
                });
                bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
                return isMobileDevice ? RedirectToAction("availability", new { id = Search.SearchGuidId }) : RedirectToAction("availability", new { id = Search.SearchGuidId });
            }
            return View();
        }

        [HttpGet]
        [Route("flights/find")]
        public ActionResult Find()
        {

            HttpRequest httpReq = System.Web.HttpContext.Current.Request;
            if (httpReq != null && httpReq.Headers != null && httpReq.Headers["X-APP-Signature"] != null)
            {
                string signature = httpReq.Headers["X-APP-Signature"];
                int affliateId = Utility.IsValidSignature(signature);
                if (affliateId > 0)
                {
                    FlightSearch Search = Utility.GetSearch(HttpContext.Request.QueryString);
                    if (Search != null)
                    {
                        Search.SearchGuidId = Utility.GetGuid();
                        Search.PortalId = Utility.PortalSettings.PortalId;
                        Search.AffiliateId = affliateId;
                        AirContext context = new AirContext()
                        {
                            IsRequestCompleted = false,
                            Search = Search
                        };

                        Utility.SetAirContextCache(Search.SearchGuidId, context);
                        Task task = Task.Factory.StartNew(() =>
                        {
                            new AirBusiness().SearchFlight(Search);
                        });
                        return RedirectToAction("availability", new { id = Search.SearchGuidId });
                    }
                }
            }
            return Redirect("/");
        }

        [HttpGet]
        [Route("flights/find-result")]
        public ActionResult FindResult()
        {
            FlightSearch Search = null;
            try
            {
                string ip = Utility.GetClientIP(System.Web.HttpContext.Current);
                IATAGeoLocation iATAGeoLocation = null;
                string currency = "USD";
                List<Task> lstTasks = new List<Task>
                        {
                            Task.Factory.StartNew(()=>{ iATAGeoLocation = Business.GeoLocation.GetAirportCode(ip); })
                        };
                Task.WaitAll(lstTasks.ToArray());
                string fromCode = iATAGeoLocation.IATACode;
                var cityAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(fromCode, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.PriorityIndex).FirstOrDefault();
                currency = cityAirport != null ? cityAirport.CurrencyCode : "USD";
                if (Request.Cookies["crnCookie"] == null || (Request.Cookies["crnCookie"] != null && Request.Cookies["crnCookie"].Value == null))
                {
                    List<Currency> currencies = CurrencyManager.GetCurrency();
                    Currency currencyName = currencies != null && currencies.Count > 0 ? currencies.Where(o => o.CurrencyType.ToUpper().Contains(currency.ToUpper())).FirstOrDefault() : null;
                    if (currency != null)
                    {
                        ShareUtility.SetCurrencyCookies(currencyName, "crnCookie", Request);
                    }
                    else
                    {
                        currencyName = new Currency() { CurrencyPrice = 1.0M, CurrencySymbol = "$", CurrencyType = "USD", Id = 2 };
                        ShareUtility.SetCurrencyCookies(currencyName, "crnCookie", Request);
                    }
                }
                else
                {
                    List<Currency> currencies = CurrencyManager.GetCurrency();
                    Currency currencyName = currencies != null && currencies.Count > 0 ? currencies.Where(o => o.CurrencyType.ToUpper().Contains(ShareUtility.GetCurrencyCodeFromCookies(Request).ToUpper())).FirstOrDefault() : null;
                    if (currency != null)
                    {
                        ShareUtility.SetCurrencyCookies(currencyName, "crnCookie", Request);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            Search = ShareUtility.GetFlightSearch(HttpContext.Request);
            if (Search != null)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    new AirBusiness().SearchFlight(Search);
                });
            }
            else
            {
                return Redirect("/");
            }
            bool isMobileDevice = Utility.GetDeviceType(Request.UserAgent);
            return isMobileDevice ? RedirectToAction("availability", new { id = Search.SearchGuidId }) : RedirectToAction("availability", new { id = Search.SearchGuidId });
        }


        [HttpGet]
        [Route("flights/find-result/{from}-{to}")]
        [Route("flights/find-result/{from}-{to}/{triptype}")]
        [Route("flights/find-result/{from}-{to}/{triptype}/{cabin}")]
        [Route("flights/find-result/{from}-{to}/{triptype}/{cabin}/{airline}")]
        public ActionResult FindResults(string from, string to, string triptype, string cabin = null, string airline = null)
        {

            HttpRequest httpReq = System.Web.HttpContext.Current.Request;
            FlightSearch Search = Utility.GetAffiliateSearchRoutes(from, to, triptype, airline, cabin, HttpContext.Request.QueryString, HttpContext.Request);
            if (Search != null)
            {
                Search.SearchGuidId = Utility.GetGuid();
                Search.PortalId = Utility.PortalSettings.PortalId;
                Search.AffiliateId = Search.AffiliateId == 0 ? Utility.PortalSettings.PortalId : Search.AffiliateId;
                Search.IsMobileDevice = Utility.GetDeviceType(Request.UserAgent);
                Search.IP = Utility.GetClientIP(System.Web.HttpContext.Current);
                CampaignMasters campaign = null;// CamapignInfo.GetCampaign(Request);
                if (campaign != null)
                {
                    Search.AffiliateId = campaign.AffiliateId;
                    Search.UtmCampaign = campaign.UtmCampaign;
                    Search.UtmMedium = campaign.UtmMedium;
                    Search.UtmSource = campaign.UtmSource;
                    Search.UtmKeyword = campaign.UtmKeyword;
                    Search.ClickedId = campaign.ClickedId;
                }

                AirContext context = new AirContext()
                {
                    IsRequestCompleted = false,
                    Search = Search
                };

                Utility.SetAirContextCache(Search.SearchGuidId, context);
                Task task = Task.Factory.StartNew(() =>
                {
                    new AirBusiness().SearchFlight(Search);
                });
                return RedirectToAction("availability", new { id = Search.SearchGuidId });
            }
            return Redirect("/");
        }

        [Route("flights/search/{id}")]
        public ActionResult Search(string id)
        {
            FlightSearch search = null;
            if (Utility.IsValidGuid(id))
            {
                AirContext context = Utility.GetAirContextCache(id);
                if (context != null && context.Search != null)
                {
                    search = context.Search;
                }
                else
                {
                    return Redirect("/");
                }
            }
            else
            {
                return Redirect("/");
            }
            return View(search);
        }
        [Route("flights/issearchcomplete/{id}")]
        public ActionResult IsSearchComplete(string id)
        {

            if (Utility.IsValidGuid(id))
            {
                AirContext context = Utility.GetAirContextCache(id);
                if (context != null && context.IsRequestCompleted)
                {
                    return Json(new { IsContextExist = true, IsRequestCompleted = true, RedirectUrl = string.Format("{0}", string.Format("{0}flights/availability/{1}", Utility.PortalSettings.DomainUrl, id)) }, JsonRequestBehavior.AllowGet);
                }
                else if (context != null && !context.IsRequestCompleted)
                {
                    return Json(new { IsContextExist = true, IsRequestCompleted = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsContextExist = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }


        [Route("flights/availability/{id}")]
        public ActionResult Availability(string id)
        {
            try
            {
                Availability _availability = null;
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context == null)
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        bool isResuestFromDeepLink = false;
                        if (context.IsRequestCompleted)
                        {
                            isResuestFromDeepLink = true;
                        }


                        _availability = new Availability() { IsResuestFromDeepLink = isResuestFromDeepLink, Factes = new ContractFacets() { Search = context.Search } };
                        return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/availability.cshtml", _availability) : View(_availability);
                    }
                    int totalResult = 0;
                    Contract soldContract = null;

                    if (context != null)
                    {
                        Contract tempSoldContract = null;
                        switch (context.Status)
                        {

                            case BookingStatus.BookWithHigherPriceChanged:
                                if (context.FlightBookingRS != null)
                                {
                                    soldContract = null;
                                    context.SelectedContract = null;
                                    context.FlightBookingRS = null;
                                    context.Status = BookingStatus.None;
                                    tempSoldContract = context.Availability.Contracts.Where<Contract>(o => o.ContractId == context.FlightBookingRS.Contract.ContractId).FirstOrDefault<Contract>();
                                    context.Availability.Contracts.Remove(tempSoldContract);
                                    Utility.SetAirContextCache(id, context);
                                }
                                break;
                            case BookingStatus.SoldOutOrUnavailable:
                                soldContract = context.SelectedContract;
                                context.FlightBookingRS = null;
                                context.Status = BookingStatus.None;
                                tempSoldContract = context.Availability.Contracts.Where<Contract>(o => o.ContractId == context.SelectedContract.ContractId).FirstOrDefault<Contract>();
                                context.SelectedContract = null;
                                context.Availability.Contracts.Remove(tempSoldContract);
                                Utility.SetAirContextCache(id, context);
                                break;
                        }



                        if (context != null && context.Availability != null && context.Availability.Contracts != null && context.Availability.Contracts.Count > 0)
                        {
                            _availability = new Availability();
                            _availability.AirlineMatrixMain = context.Availability.AirlineMatrixMain;
                            _availability.UniqueAirlineList = context.Availability.UniqueAirlineList;

                            _availability.FilterAirports = context.Availability.FilterAirports;
                            _availability.Contracts = null;

                            lock (context.Availability)
                            {
                                //TODO Lock Condition

                                _availability.Factes = new ContractFacets() { Search = context.Search };
                                AirBusiness.PrepareFilter(context.Availability, _availability.Factes);
                                Response<List<Contract>> response = AirBusiness.FilterContracts(context, new ContractFilterRQ() { Tab = 0, PageNumber = 1, Stops = null }, out totalResult, true);
                                if (response != null && response.TransactionStatus != null && response.TransactionStatus.IsSuccess)
                                {
                                    _availability.Factes.TotalContract = totalResult;
                                    _availability.Contracts = response.Result;
                                }
                                if (soldContract != null)
                                {
                                    _availability.Contracts.Insert(0, soldContract);
                                }

                                _availability.Factes.ActualMinPrice = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.Actual);
                                _availability.Factes.NearbyMinPrice = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.NearBy);
                                _availability.Factes.AlternameMinPrice = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.Flexi);
                                _availability.Factes.PhoneOnlyMinPrice = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.PhoneOnly);
                            }
                            return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/availability.cshtml", _availability) : View(_availability);
                        }
                        else if (context != null && (context.Availability != null || context.Availability == null))
                        {
                            _availability = new Availability() { Factes = new ContractFacets() { Search = context.Search } };
                            return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/availability.cshtml", _availability) : View(_availability);

                        }

                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
                return Redirect("/");
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Presentation.Controllers.Air.Availbility|Guid:" + id + "|Exception:" + ex.ToString());
            }
            return View();
        }

        #region 2 Steps
        private Availability GetAvailability(string id)
        {
            Availability _availability = null;
            try
            {
                if (Utility.IsValidGuid(id))
                {
                    int totalResult = 0;
                    Contract soldContract = null;
                    AirContext context = Utility.GetAirContextCache(id);
                    Contract tempSoldContract = null;
                    if (context != null)
                    {
                        switch (context.Status)
                        {

                            case BookingStatus.BookWithHigherPriceChanged:
                                if (context.FlightBookingRS != null)
                                {
                                    soldContract = null;
                                    context.SelectedContract = null;
                                    context.FlightBookingRS = null;
                                    context.Status = BookingStatus.None;
                                    tempSoldContract = context.Availability.Contracts.Where<Contract>(o => o.ContractId == context.FlightBookingRS.Contract.ContractId).FirstOrDefault<Contract>();
                                    context.Availability.Contracts.Remove(tempSoldContract);
                                    Utility.SetAirContextCache(id, context);
                                }
                                break;
                            case BookingStatus.SoldOutOrUnavailable:
                                soldContract = context.SelectedContract;
                                context.FlightBookingRS = null;
                                context.Status = BookingStatus.None;
                                tempSoldContract = context.Availability.Contracts.Where<Contract>(o => o.ContractId == context.SelectedContract.ContractId).FirstOrDefault<Contract>();
                                context.SelectedContract = null;
                                context.Availability.Contracts.Remove(tempSoldContract);
                                Utility.SetAirContextCache(id, context);
                                break;
                        }



                        if (context != null && context.Availability != null && context.Availability.Contracts != null && context.Availability.Contracts.Count > 0)
                        {
                            _availability = new Availability();
                            _availability.AirlineMatrixMain = context.Availability.AirlineMatrixMain;
                            _availability.UniqueAirlineList = context.Availability.UniqueAirlineList;
                            _availability.FilterAirports = context.Availability.FilterAirports;
                            _availability.Contracts = null;

                            lock (context.Availability)
                            {
                                //TODO Lock Condition

                                _availability.Factes = new ContractFacets() { Search = context.Search };
                                AirBusiness.PrepareFilter(context.Availability, _availability.Factes);
                                Response<List<Contract>> response = AirBusiness.FilterContracts(context, new ContractFilterRQ() { Tab = 0, PageNumber = 1, Stops = null }, out totalResult, true);
                                if (response != null && response.TransactionStatus != null && response.TransactionStatus.IsSuccess)
                                {
                                    _availability.Factes.TotalContract = totalResult;
                                    _availability.Contracts = response.Result;
                                }
                                if (soldContract != null)
                                {
                                    _availability.Contracts.Insert(0, soldContract);
                                }

                                _availability.Factes.ActualMinPrice = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.Actual);
                                _availability.Factes.NearbyMinPrice = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.NearBy);
                                _availability.Factes.AlternameMinPrice = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.Flexi);
                                _availability.Factes.PhoneOnlyMinPrice = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.PhoneOnly);
                            }
                        }
                        else if (context != null && (context.Availability != null || context.Availability == null))
                        {
                            _availability = new Availability() { Factes = new ContractFacets() { Search = context.Search } };

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Presentation.Controllers.Air.Availbility|Guid:" + id + "|Exception:" + ex.ToString());
            }
            return _availability;
        }
        [Route("flights/issearchcompletetwosteps/{id}")]
        public ActionResult IsSearchCompleteTwoSteps(string id)
        {

            if (Utility.IsValidGuid(id))
            {
                AirContext context = Utility.GetAirContextCache(id);
                if (context != null && context.IsRequestCompleted)
                {
                    string partialView = string.Format("~/views/flights/{0}partial/_availability.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");
                    string partialViewLeft = string.Format("~/views/flights/{0}partial/_airfilter.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");

                    Availability availability = GetAvailability(id);
                    return Json(new { IsContextExist = (availability != null ? true : false), IsRequestCompleted = true, HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, availability), HtmlResponseLeft = ShareUtility.RenderViewToString(this.ControllerContext, partialViewLeft, availability) }, JsonRequestBehavior.AllowGet);
                }
                else if (context != null && !context.IsRequestCompleted)
                {
                    return Json(new { IsContextExist = true, IsRequestCompleted = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsContextExist = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }
        #endregion 2 Step


        public ActionResult ApplyContractFilter(ClientContractFilterRQ request)
        {
            try
            {
                if (request != null)
                {
                    string partialView = string.Format("~/views/flights/{0}partial/_contracts.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");
                    ContractFilterRQ filterRQ = GetContractFilterRQ(request);
                    if (filterRQ != null)
                    {
                        AirContext context = Utility.GetAirContextCache(filterRQ.Guid);
                        if (context != null)
                        {
                            int resultCount = 0;
                            Response<List<Contract>> response = AirBusiness.FilterContracts(context, filterRQ, out resultCount);
                            if (response != null && response.TransactionStatus != null && response.TransactionStatus.IsSuccess)
                            {
                                var result = new
                                {
                                    IsSuccess = true,
                                    TotalResult = resultCount,
                                    IsScroll = response.Result != null && response.Result.Count < Utility.Settings.ListingPageSize ? false : true,
                                    ErrorMessage = "",
                                    HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, response.Result)
                                };
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var result = new
                                {
                                    IsSuccess = false,
                                    TotalResult = 0,
                                    IsScroll = false,
                                    ErrorMessage = "",
                                    HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, response.Result)
                                };
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }

                var resultNotFound = new
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid request found.",
                    HtmlResponse = ""
                };
                return Json(resultNotFound, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("ApplyContractFilter|Exception:" + ex.ToString());
                var result = new
                {
                    IsSuccess = false,
                    ErrorMessage = "Internal server error.",
                    HtmlResponse = ""
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("flights/verification/{id}")]
        public ActionResult Verification(string id)
        {
            if (Utility.IsValidGuid(id))
            {
                return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/verification.cshtml", model: id) : View(model: id);
            }
            else
            {
                return Redirect("/");
            }
        }

        [Route("flights/isverificationcomplete/{id}")]
        public ActionResult IsVerificationComplete(string id)
        {

            if (Utility.IsValidGuid(id))
            {
                AirContext context = Utility.GetAirContextCache(id);
                if (context != null && context.IsRequestCompleted && context.Status == BookingStatus.InProgress)
                {
                    return Json(new { IsContextExist = true, IsRequestCompleted = true, IsSoldout = false, RedirectUrl = string.Format("{0}flights/payment/{1}", Utility.PortalSettings.DomainUrl, id) }, JsonRequestBehavior.AllowGet);
                }
                else if (context != null && context.IsRequestCompleted && context.Status == BookingStatus.SoldOutOrUnavailable)
                {
                    return Json(new { IsContextExist = true, IsRequestCompleted = true, IsSoldout = true, RedirectUrl = string.Format("{0}flights/availability/{1}", Utility.PortalSettings.DomainUrl, id) }, JsonRequestBehavior.AllowGet);
                }
                else if (context != null && !context.IsRequestCompleted)
                {
                    return Json(new { IsContextExist = true, IsRequestCompleted = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsContextExist = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }
        public ActionResult GetSelectedContract(string id, int cId)
        {
            try
            {
                if (Utility.IsValidGuid(id) && cId > 0)
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null)
                    {
                        Contract contract = context.Availability.Contracts.Where<Contract>(o => o.ContractId == cId).FirstOrDefault<Contract>();
                        if (contract != null)
                        {
                            context.SelectedContract = contract;
                            context.IsRequestCompleted = false;
                            context.BookingDetailRQ = null;
                            Utility.SetAirContextCache(id, context);
                            Task.Factory.StartNew(() =>
                            {
                                new AirBusiness().CheckAvailavility(id, cId);
                            });


                            if (!Utility.PortalSettings.IsCheckAvailability)
                            {
                                context.Status = BookingStatus.InProgress;
                                context.IsRequestCompleted = true;
                                context.SelectedContract = contract;
                                Utility.SetAirContextCache(id, context);
                            }
                            return Json(new { IsContractExist = true, RedirectUrl = string.Format("{0}flights/verification/{1}", Utility.PortalSettings.DomainUrl, id) }, JsonRequestBehavior.AllowGet);

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Flight.GetSelectedContract|Exception:" + ex.ToString());
            }
            return Json(new { IsContextExist = false }, JsonRequestBehavior.AllowGet);
        }
        [Route("flights/contract-verification/{id}-{cid}")]
        public ActionResult GetContract(string id, int cid)
        {
            try
            {
                string utmsource = string.Empty;
                if (!string.IsNullOrEmpty(id) && cid > 0)
                {
                    AirContext context = Utility.GetMediumMarketContext(id);
                    if (context != null)
                    {
                        if (context.Search != null)
                        {
                            var orignAirpor = Utility.Airports.Where(o => o.AirportCode.Equals(context.Search.Origin)).FirstOrDefault();
                            if (orignAirpor != null)
                            {
                                context.Search.OriginSearch = string.Format("{0} - {1}, {2}, {3}", orignAirpor.AirportCode, orignAirpor.AirportName, orignAirpor.City, orignAirpor.CountryName);
                                context.Search.OriginCountry = orignAirpor.CountryName;
                                context.Search.OriginAirportName = orignAirpor.AirportName;
                            }
                            var DestAirpor = Utility.Airports.Where(o => o.AirportCode.Equals(context.Search.Destination)).FirstOrDefault();
                            if (DestAirpor != null)
                            {
                                context.Search.DestinationSearch = string.Format("{0} - {1}, {2}, {3}", DestAirpor.AirportCode, DestAirpor.AirportName, DestAirpor.City, DestAirpor.CountryName);
                                context.Search.DestCountry = DestAirpor.CountryName;
                                context.Search.DestAirportName = DestAirpor.AirportName;
                            }

                        }

                        Contract contract = context.Availability.Contracts.Where<Contract>(o => o.ContractId == cid).FirstOrDefault<Contract>();
                        context.SelectedContract = contract;
                        context.IsRequestCompleted = false;
                        if (context.Search != null && string.IsNullOrEmpty(context.Search.IP))
                        {
                            context.Search.IP = Utility.GetClientIP(System.Web.HttpContext.Current);
                        }
                        Utility.SetAirContextCache(contract.SearchGuid, context);


                        Task.Factory.StartNew(() =>
                        {
                            MetaClicks click = new MetaClicks() { PortalId = context.Search.PortalId, Origin = context.Search.Origin, Destination = context.Search.Destination, TripType = (int)context.Search.TripType, Departure = context.Search.Departure, Return = context.Search.Return, AffiliateId = context.Search.AffiliateId, IP = context.Search.IP };
                            Operation.MetaClicks(click);
                        });

                        Task.Factory.StartNew(() =>
                        {
                            new AirBusiness().CheckAvailavility(contract.SearchGuid, cid);
                        });

                        if (Utility.PortalSettings.IsCheckAvailability)
                        {
                            return Redirect(string.Format("{0}flights/verification/{1}", Utility.PortalSettings.DomainUrl, contract.SearchGuid));
                        }
                        else
                        {
                            return Redirect(string.Format("{0}flights/payment/{1}", Utility.PortalSettings.DomainUrl, contract.SearchGuid));
                        }
                    }
                    else
                    {
                        Utility.Logger.Debug("SESSION DROP FROM META SEARCH");
                    }
                }
                return Redirect("/");
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Flight.GetSelectedContract|Exception:" + ex.ToString());
            }
            return Json(new { IsContextExist = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Payment(string id)
        {
            BookingDetail bookingDetail = null;
            try
            {
                bool isDevice = Utility.GetDeviceType(Request.UserAgent);
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null && context.SelectedContract != null)
                    {
                        bookingDetail = new BookingDetail();
                        //#region Travelex Insurance Production
                        //if (Utility.Settings.TravelexInsuranceAPI.IsEnable && !isDevice)
                        //{
                        //    if (TravelexBussiness.IsTravelInsurance(context.SelectedContract))
                        //    {
                        //        if (string.IsNullOrEmpty(context.TSLUrl))
                        //        {
                        //            PayLinkRS payLinkRS = TravelexBussiness.TravelexGetPaymentConfiguration();
                        //            if (payLinkRS != null && !string.IsNullOrEmpty(payLinkRS.TsepUrl))
                        //            {
                        //                context.TSLUrl = payLinkRS.TsepUrl;
                        //                ViewBag.TSL_URL = context.TSLUrl;
                        //                Utility.SetAirContextCache(id, context);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            ViewBag.TSL_URL = context.TSLUrl;
                        //        }
                        //    }
                        //}
                        //#endregion
                        AirBusiness.PreparePaymentModel(bookingDetail, context);
                        return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/payment.cshtml", bookingDetail) : View(bookingDetail);
                    }
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.Payment|Exception:" + ex.ToString());
            }
            return Redirect("/");

        }
        [HttpPost]
        public ActionResult Payment(FormCollection collection, BookingDetail model)
        {
            BookingDetail bookingDetail = null;
            if (ModelState.IsValid)
            {
                if (collection["guid"] != null)
                {
                    string guid = collection["guid"].ToString();
                    AirContext _context = Utility.GetAirContextCache(guid);
                    if (_context != null)
                    {
                        model.BillingDetails.CardNumber = model.BillingDetails.CardNumber.Replace(" ", "");
                        CurrencyMaster currencyMaster = ShareUtility.GetCurrency(Request);
                        _context.BookingDetailRQ = new BookingDetail()
                        {
                            Travellers = model.Travellers,
                            BillingDetails = model.BillingDetails,
                            FlightSearch = _context.Search,
                            Contract = _context.SelectedContract,
                            TravelerInsurance = _context.BookingDetailRQ != null ? _context.BookingDetailRQ.TravelerInsurance : null,
                            CouponDetails = _context.BookingDetailRQ != null && _context.BookingDetailRQ.CouponDetails != null ? _context.BookingDetailRQ.CouponDetails : null,
                            ExtendedCancellation = _context.BookingDetailRQ != null && _context.BookingDetailRQ.ExtendedCancellation != null ? _context.BookingDetailRQ.ExtendedCancellation : null,
                            Currency = CurrencyType.USD,
                            CurrencyCode = currencyMaster.CurrencyType,
                            CurrencyConversion = currencyMaster.CurrencyPrice
                        };
                        _context.IsRequestCompleted = false;
                        Utility.SetAirContextCache(guid, _context);

                        Task task = Task.Factory.StartNew(() =>
                        {
                            new AirBusiness().BookFlight(_context.BookingDetailRQ);
                        });
                        return RedirectToAction("reverification", new { id = guid });
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
            }
            return View(bookingDetail);
        }
        [Route("flights/reverification/{id}")]
        public ActionResult ReVerification(string id)
        {
            if (Utility.IsValidGuid(id))
            {
                return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/reverification.cshtml", model: id) : View(model: id);
            }
            else
            {
                return Redirect("/");
            }
        }

        public ActionResult ReBookHigherPrice(string id)
        {
            try
            {
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null && context.BookingDetailRQ != null)
                    {
                        context.BookingDetailRQ.Contract = context.FlightBookingRS.Contract;
                        context.IsRequestCompleted = false;
                        Utility.SetAirContextCache(id, context);
                        Task task = Task.Factory.StartNew(() =>
                        {
                            new AirBusiness().BookFlight(context.BookingDetailRQ);
                        });
                        return Json(new { IsContextExist = true, RedirectUrl = string.Format("{0}flights/reverification/{1}", Utility.PortalSettings.DomainUrl, id) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { IsContextExist = false, RedirectUrl = "/" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Flight.ReBookHigherPrice|Exception:" + ex.ToString());
            }
            return Json(new { IsContextExist = false, RedirectUrl = "/" }, JsonRequestBehavior.AllowGet);

        }

        [Route("flights/check-booking-complete/{id}")]
        public ActionResult CheckBookingComplete(string id)
        {
            try
            {
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null && context.IsRequestCompleted && context.FlightBookingRS != null && context.FlightBookingRS.BookingStatus == BookingStatus.BookingConfirmed)
                    {
                        string tguid = context.FlightBookingRS.TranGuid;
                        var result = new { IsContextExist = true, IsRequestCompleted = true, RedirectUrl = string.Format("{0}flights/confirmation/{1}", Utility.PortalSettings.DomainUrl, tguid) };
                        Transaction transaction = new Transaction() { BookedOn = DateTime.UtcNow, Id = context.FlightBookingRS.TransactionId, PNR = context.FlightBookingRS.PNR };
                        AirContext contextBook = new AirContext()
                        {
                            Availability = null,
                            Search = null,
                            SelectedContract = null,
                            Status = BookingStatus.None,
                            IsSendBookingMail = true,
                            FlightBookingRS = null,
                            BookingDetailRQ = context.BookingDetailRQ
                        };

                        if (context.Search != null && context.Search.AffiliateId != 0)
                        {
                            CampaignMasters campaign = null;// CamapignInfo.GetCampaign(Request);

                            BookingCampaignTracking tracking = new BookingCampaignTracking()
                            {
                                BookingId = context.FlightBookingRS.TransactionId,
                                AffiliateId = context.Search.AffiliateId,
                                SearchGuid = context.Search.SearchGuidId,
                                UtmCampaign = context.Search.UtmCampaign,
                                UtmMedium = context.Search.UtmMedium,
                                UtmSource = context.Search.UtmSource,
                                ClickedId = context.Search.ClickedId,
                                UtmContent = context.Search.UtmContent,
                                UtmTerm = context.Search.UtmTerm,
                                UtmKeyword = context.Search.UtmKeyword,
                                Origin = context.Search.Origin,
                                Destination = context.Search.Destination,
                                TripType = (int)context.Search.TripType,
                                IsMobile = Utility.GetDeviceType(Request.UserAgent)
                            };
                            if (campaign != null)
                            {
                                tracking.UtmCampaign = campaign.UtmCampaign;
                                tracking.UtmMedium = campaign.UtmMedium;
                                tracking.UtmSource = campaign.UtmSource;
                                tracking.ClickedId = campaign.ClickedId;
                                tracking.UtmContent = campaign.UtmContent;
                                tracking.UtmTerm = campaign.UtmTerm;
                                tracking.UtmKeyword = campaign.UtmKeyword;
                                tracking.UtmPublisher = campaign.UtmPublisher;
                                tracking.UtmPublisherId = campaign.UtmPublisherId;
                                tracking.UtmChannelId = campaign.UtmChannelId;

                            }
                            Task.Factory.StartNew(() => BookingInformation.SaveCampaignTrackings(tracking));
                        }

                        contextBook.BookingDetailRQ.Contract = context.FlightBookingRS.Contract != null ? context.FlightBookingRS.Contract : context.BookingDetailRQ.Contract;
                        contextBook.BookingDetailRQ.Transaction = transaction;
                        context.FlightBookingRS = null;
                        Utility.SetAirContextCache(tguid, contextBook);
                        try
                        {
                            Task.Factory.StartNew(() => Utility.RemoveAirContextCache(id));
                        }
                        catch (Exception ex)
                        {
                            Utility.Logger.Error("CHECKBOOKINGCMPLETE|Exception:" + ex.ToString());
                        }
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else if (context != null && context.IsRequestCompleted && context.FlightBookingRS != null && context.FlightBookingRS.BookingStatus == BookingStatus.BookWithHigherPriceChanged)
                    {

                        return Json(new { IsContextExist = true, IsRequestCompleted = true, RedirectUrl = string.Format("{0}flights/payment/{1}", Utility.PortalSettings.DomainUrl, id) }, JsonRequestBehavior.AllowGet);
                    }
                    else if (context != null && context.IsRequestCompleted && context.FlightBookingRS != null && context.FlightBookingRS.BookingStatus == BookingStatus.SoldOutOrUnavailable)
                    {
                        context.Status = BookingStatus.SoldOutOrUnavailable;
                        context.SelectedContract = context.FlightBookingRS.Contract;
                        Utility.SetAirContextCache(id, context);
                        return Json(new { IsContextExist = true, IsRequestCompleted = true, RedirectUrl = string.Format("{0}flights/availability/{1}", Utility.PortalSettings.DomainUrl, id) }, JsonRequestBehavior.AllowGet);
                    }
                    else if (context != null && !context.IsRequestCompleted)
                    {
                        return Json(new { IsContextExist = true, IsRequestCompleted = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { IsContextExist = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Flight.CheckBookingComplete|guid:{0}|Exception:", id, ex.ToString()));
            }
            return Json(new { IsContextExist = false }, JsonRequestBehavior.AllowGet);
        }

        [Route("flights/confirmation/{id}")]
        public ActionResult Confirmation(string id)
        {
            try
            {
                BookingDetails bookingDetails = null;
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null && context.BookingDetailRQ != null && context.BookingDetailRQ.Transaction != null && context.BookingDetailRQ.Transaction.Id > 0)
                    {
                        bookingDetails = BookingInformation.GetBookingDetails(context.BookingDetailRQ.Transaction.Id);

                        if (bookingDetails != null)
                        {
                            //Save User Location
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    string location = AirBusiness.GetUserLocationByIP(context.BookingDetailRQ.FlightSearch.IP);
                                    if (!string.IsNullOrEmpty(location))
                                    {
                                        Operation.UpdateBookingUserLocation(context.BookingDetailRQ.Transaction.Id, location);
                                    }
                                }
                                catch 
                                {  }                                
                            });


                            if (context.IsSendBookingMail)
                            {
                                //PurchaseInSurance(bookingDetails, context);
                                string htmlMailString = ShareUtility.RenderViewToString(this.ControllerContext, "~/views/emails/bookingreceiptdb.cshtml", bookingDetails);
                                string htmlMailStringForSelf = ShareUtility.RenderViewToString(this.ControllerContext, "~/views/emails/bookingreceiptdbself.cshtml", bookingDetails);
                                if (!string.IsNullOrEmpty(htmlMailString) && htmlMailString.Length > 1000)
                                {
                                    Task.Factory.StartNew(() =>
                                    {
                                        EmailTransaction transaction = new EmailTransaction()
                                        {
                                            EmailType = EmailType.BookingReceipt,
                                            MailBody = htmlMailString,
                                            PortalId = bookingDetails.Transaction.PortalId,
                                            MailRecipient = bookingDetails.BillingDetails.Email,
                                            TransactionId = bookingDetails.Transaction.Id

                                        };
                                        bool isMailSent = EmailHelper.SendMails(transaction);
                                        Utility.Logger.Info(string.Format("BOOKING RECEIPT|Guid:{0}| TID:{1}|{2}", id, context.BookingDetailRQ.Transaction.Id, isMailSent ? "MAIL SENT" : "UNABLE TO SENT MAIL"));
                                        context.IsSendBookingMail = false;
                                        Utility.SetAirContextCache(id, context);
                                    });
                                }
                                //////////// Send Mail To self ///////////////
                                if (!string.IsNullOrEmpty(htmlMailStringForSelf) && htmlMailStringForSelf.Length > 1000 && !string.IsNullOrEmpty(Utility.PortalSettings.SelfBookingMail))
                                {
                                    Task.Factory.StartNew(() =>
                                    {
                                        EmailTransaction transaction = new EmailTransaction()
                                        {
                                            EmailType = EmailType.SelefBooking,
                                            MailBody = htmlMailStringForSelf,
                                            PortalId = bookingDetails.Transaction.PortalId,
                                            MailRecipient = Utility.PortalSettings.SelfBookingMail,
                                            TransactionId = bookingDetails.Transaction.Id

                                        };
                                        bool isMailSent = EmailHelper.SendMails(transaction);
                                        Utility.Logger.Info(string.Format("SELF BOOKING MAIL RECEIPT|Guid:{0}| TID:{1}|{2}", id, context.BookingDetailRQ.Transaction.Id, isMailSent ? "MAIL SENT" : "UNABLE TO SENT MAIL"));

                                    });
                                }
                            }
                            ViewBag.IsFromCache = true;
                            return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/confirmation.cshtml", bookingDetails) : View(bookingDetails);
                        }
                    }
                    else
                    {
                        bookingDetails = BookingInformation.GetBookingDetailsGuid(id);
                        if (bookingDetails != null)
                        {
                            return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/confirmation.cshtml", bookingDetails) : View(bookingDetails);
                        }
                    }
                }
                else
                {
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Flight.Confirmation|id:{0}|Exception:", id, ex.ToString()));
            }
            return View();
        }

        [Route("flights/confirmationv2/{id}")]
        public ActionResult ConfirmationV2(string id)
        {
            try
            {
                BookingDetails bookingDetails = null;
                if (Utility.IsValidGuid(id))
                {
                    bookingDetails = BookingInformation.GetBookingDetailsGuid(id);
                    if (bookingDetails != null)
                    {
                        return View(bookingDetails);
                    }
                }
                else
                {
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Flight.ConfirmationV2|id:{0}|Exception:", id, ex.ToString()));
            }
            return View();
        }
        private static void PurchaseInSurance(BookingDetails bookingDetails, AirContext context)
        {
            try
            {
                List<Task> lstTasks = new List<Task>
                        {
                            Task.Factory.StartNew(()=>{
                                  if (Utility.Settings.TravelBagAPI.IsEnable && context.BookingDetailRQ.BagInsuranc != null && context.BookingDetailRQ.BagInsuranc.BagInsuranceType != BagInsuranceType.NONE)
                {
                    bookingDetails.BaggageInsurances = BlueBagRibbon.PurchaseService(context.BookingDetailRQ);
                }
                            }),
                            Task.Factory.StartNew(()=>{

                                     if (Utility.Settings.TravelexInsuranceAPI.IsEnable && context.BookingDetailRQ.TravelerInsurance != null && context.BookingDetailRQ.TravelerInsurance.IsTravelProtected && !string.IsNullOrEmpty(context.BookingDetailRQ.TravelerInsurance.Token)  )
                {
                    bookingDetails.TravelInsurance = null;// TravelexBussiness.PurchaseTravelInsurance(bookingDetails, context);
                }
                            }),

                        };
                Task.WaitAll(lstTasks.ToArray());
            }
            catch (Exception ex)
            {

                Utility.Logger.Error(string.Format("Flight.Confirmation.PurchaseInSurance|id:{0}|Exception:", context.Search.SearchGuidId, ex.ToString()));
            }

        }

        [Route("flights/tax-break-up")]
        public ActionResult TaxBreakUp()
        {
            return View();
        }

        [Route("flights/travelprotecttripget/")]
        [HttpPost]
        public ActionResult TravelProtectTripGet(InsuraneTravel model)
        {
            string partialView = string.Format("~/views/flights/{0}partial/_pricedetails.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");
            try
            {

                if (Utility.IsValidGuid(model.Guid))
                {
                    AirContext _context = Utility.GetAirContextCache(model.Guid);
                    TravelerInsurance travelerInsurance = new TravelerInsurance()
                    {
                        TotalPrice = 0,
                        PPaxPrice = 0,
                        Error = true,
                        IsTravelProtected = false,
                        Warning = string.Empty
                    };
                    if (_context != null && _context.SelectedContract != null)
                    {
                        if (Utility.Settings.TravelexInsuranceAPI.IsEnable)
                        {

                            if (model.TravelerInsurance.IsTravelProtected)
                            {
                                travelerInsurance.IsTravelProtected = true;
                                travelerInsurance.PPaxPrice = (decimal)Utility.Settings.TravelexInsuranceAPI.PPaxPrice;
                                travelerInsurance.TotalPrice = travelerInsurance.PPaxPrice * (_context.SelectedContract.Adult + _context.SelectedContract.Child + _context.SelectedContract.InfantOnLap + _context.SelectedContract.InfantOnSeat + _context.SelectedContract.Senior);
                                _context.BookingDetailRQ = new BookingDetail()
                                {
                                    Contract = _context.SelectedContract,
                                    BagInsuranc = _context.BookingDetailRQ != null && _context.BookingDetailRQ.BagInsuranc != null ? _context.BookingDetailRQ.BagInsuranc : null,
                                    TravelerInsurance = travelerInsurance
                                };
                                Utility.SetAirContextCache(model.Guid, _context);
                                var result = new
                                {
                                    IsError = travelerInsurance.Error,
                                    IsSuccess = true,
                                    WarningMsg = travelerInsurance.Warning,
                                    TotalPrice = _context.BookingDetailRQ.TravelerInsurance != null && _context.BookingDetailRQ.TravelerInsurance.IsTravelProtected && _context.BookingDetailRQ.TravelerInsurance.TotalPrice != 0 ? _context.BookingDetailRQ.TravelerInsurance.TotalPrice : 0,
                                    HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, _context.BookingDetailRQ),
                                };
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var result = new
                                {
                                    IsSuccess = true,
                                    TotalPrice = "",
                                    HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, _context.BookingDetailRQ)
                                };
                                Utility.SetAirContextCache(model.Guid, _context);

                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.TravelProtectTripGet|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }

        [Route("flights/protect-trip/{id}")]
        [HttpGet]
        public ActionResult ProtectTrip(string id)
        {
            try
            {
                BookingDetails bookingDetails = null;
                RatesRS ratesRS = null;
                PayLinkRS payLinkRS = null;
                if (Utility.IsValidGuid(id))
                {
                    bookingDetails = BookingInformation.GetBookingDetailsGuid(id);
                    if (bookingDetails != null && bookingDetails.TravelInsurance == null)
                    {
                        if (Utility.Settings.TravelexInsuranceAPI.IsEnable)
                        {
                            RatesRQ rateRQ = new RatesRQ()
                            {
                                BookingGuid = bookingDetails.Transaction.ReferenceNumber,
                                Depart = bookingDetails.Flight.DeptDate,
                                Return = bookingDetails.Flight.ReturnDate,
                                NoTravel = bookingDetails.Travellers.Count,
                                ResidenceCountery = bookingDetails.BillingDetails.Country,
                                ResidenceState = bookingDetails.BillingDetails.State,
                                CardNumber = bookingDetails.BillingDetails.CardNumber,
                                CardExpiryMonth = bookingDetails.BillingDetails.ExpiryMonth,
                                CardExpiryYear = bookingDetails.BillingDetails.ExpiryYear,
                                Travelers = new List<RatesTravlerDetailRQ>()
                            };

                            if (bookingDetails.Travellers != null && bookingDetails.Travellers.Count > 0)
                            {
                                RatesTravlerDetailRQ ratesTravlerDetailRQ = null;
                                foreach (Travellers item in bookingDetails.Travellers)
                                {

                                    ratesTravlerDetailRQ = new RatesTravlerDetailRQ()
                                    {
                                        DOB = (item.DOB ?? DateTime.Now),
                                        Fare = GetFare((TravellerPaxType)item.PaxType, bookingDetails.PriceDetail)
                                    };
                                    rateRQ.Travelers.Add(ratesTravlerDetailRQ);
                                }
                            }

                            ratesRS = null;// TravelexBussiness.TravelexGetRates(rateRQ);
                            payLinkRS = null;// TravelexBussiness.TravelexGetPaymentConfiguration();
                            return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/ProtectTrip.cshtml", Tuple.Create<BookingDetails, RatesRS, PayLinkRS>(bookingDetails, ratesRS, payLinkRS)) : View(Tuple.Create<BookingDetails, RatesRS, PayLinkRS>(bookingDetails, ratesRS, payLinkRS));
                        }

                    }
                    else
                    {
                        return Utility.GetDeviceType(Request.UserAgent) ? View("~/Views/flights/mobile/ProtectTrip.cshtml", Tuple.Create<BookingDetails, RatesRS, PayLinkRS>(bookingDetails, ratesRS, payLinkRS)) : View(Tuple.Create<BookingDetails, RatesRS, PayLinkRS>(bookingDetails, ratesRS, payLinkRS));
                    }
                }
                else
                {
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Flight.ProtectTrip|id:{0}|Exception:", id, ex.ToString()));
            }
            return View();
        }

        private decimal GetFare(TravellerPaxType paxType, List<PriceDetails> priceDetail)
        {
            decimal response = 0;
            PriceDetails priceDetails = priceDetail.Where<PriceDetails>(o => o.PaxType == (int)paxType).FirstOrDefault<PriceDetails>();
            if (priceDetails != null)
            {
                response = priceDetails.BaseFare + priceDetails.Tax + priceDetails.Markup + priceDetails.SupplierFee;
            }
            return response;
        }

        [Route("flights/protect-trip/")]
        [HttpPost]
        public ActionResult ProtectTrip(FormCollection formCollection, ProtectTrip model)
        {
            try
            {
                BookingDetails bookingDetails = null;
                if (Utility.IsValidGuid(model.Guid))
                {

                    bookingDetails = BookingInformation.GetBookingDetailsGuid(model.Guid);
                    AirContext context = new AirContext()
                    {
                        Availability = null,
                        Search = null,
                        SelectedContract = null,
                        Status = BookingStatus.None,
                        IsSendBookingMail = true,
                        FlightBookingRS = null,
                        BookingDetailRQ = null
                    };

                    if (context.IsSendBookingMail && Utility.Settings.TravelexInsuranceAPI.IsEnable)
                    {
                        CreateRQ createRQ = new CreateRQ()
                        {
                            BookingId = bookingDetails.Transaction.Id,
                            Depart = bookingDetails.Flight.DeptDate,
                            Return = bookingDetails.Flight.ReturnDate,
                            NoTravel = bookingDetails.Travellers.Count,
                            Countery = bookingDetails.BillingDetails.Country,
                            State = bookingDetails.BillingDetails.State,
                            City = bookingDetails.BillingDetails.City,
                            ZipCode = bookingDetails.BillingDetails.ZipCode,
                            AddressLine1 = bookingDetails.BillingDetails.AddressLine1,
                            AddressLine2 = bookingDetails.BillingDetails.AddressLine2,
                            BillingPhone = bookingDetails.BillingDetails.BillingPhone,
                            Email = bookingDetails.BillingDetails.Email,
                            cardType = model.CardType,
                            InsuTokenNo = model.Token,
                            CCHolderName = bookingDetails.BillingDetails.CCHolderName,
                            CardNumber = bookingDetails.BillingDetails.CardNumber,
                            CardExpiryMonth = bookingDetails.BillingDetails.ExpiryMonth,
                            CardExpiryYear = bookingDetails.BillingDetails.ExpiryYear,
                            Travelers = new List<CreateTravlerDetailRQ>(),
                            TotalAmount = model.TotalAmount,

                        };

                        if (bookingDetails.Travellers != null && bookingDetails.Travellers.Count > 0)
                        {
                            CreateTravlerDetailRQ createTravlerDetailRQ = null;
                            foreach (Travellers item in bookingDetails.Travellers)
                            {

                                createTravlerDetailRQ = new CreateTravlerDetailRQ()
                                {
                                    FirstName = item.FirstName,
                                    LastName = item.LastName,
                                    DOB = (item.DOB ?? DateTime.Now),
                                    Fare = GetFare((TravellerPaxType)item.PaxType, bookingDetails.PriceDetail)
                                };
                                createRQ.Travelers.Add(createTravlerDetailRQ);
                            }
                        }

                        bookingDetails.TravelInsurance = null;// TravelexBussiness.TravelexCreatePolicy(createRQ);
                        if (!string.IsNullOrEmpty(bookingDetails.TravelInsurance.Warnings))
                        {
                            return Json(new { IsSuccess = false, RedirectUrl = "/", Error = bookingDetails.TravelInsurance.Warnings, }, JsonRequestBehavior.AllowGet);
                        }
                        string htmlMailString = ShareUtility.RenderViewToString(this.ControllerContext, "~/views/emails/bookingreceiptdb.cshtml", bookingDetails);
                        if (!string.IsNullOrEmpty(htmlMailString) && htmlMailString.Length > 1000)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                EmailTransaction transaction = new EmailTransaction()
                                {
                                    EmailType = EmailType.BookingReceipt,
                                    MailBody = htmlMailString,
                                    PortalId = bookingDetails.Transaction.PortalId,
                                    MailRecipient = "test261947@gmail.com",
                                    TransactionId = bookingDetails.Transaction.Id
                                };
                                bool isMailSent = EmailHelper.SendMails(transaction);

                                Utility.Logger.Info(string.Format("Protect Trip RECEIPT|Guid:{0}| TID:{1}|{2}", model.Guid, model.Guid, isMailSent ? "MAIL SENT" : "UNABLE TO SENT MAIL"));
                                context.IsSendBookingMail = false;
                                Utility.SetAirContextCache(model.Guid, context);
                            });
                        }
                    }
                    return Json(new { IsSuccess = true, RedirectUrl = string.Format("{0}flights/protect-trip/{1}", Utility.PortalSettings.DomainUrl, model.Guid) }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Flight.ProtectTrip|id:{0}|Exception:", model.Guid, ex.ToString()));
            }
            return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
        }


        [Route("flights/addbookingfailure")]
        [HttpPost]
        public ActionResult AddBookingFailure(string Guid, PriceChangeAction PriceChangeAction)
        {
            try
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    BookingInformation.UpdateBookingFailureDatail(Guid, ((int)PriceChangeAction).ToString());
                });
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Flight.AddBookingFailure|guid:{0}|Action:{1}|Exception:", Guid, ((int)PriceChangeAction).ToString(), ex.ToString()));
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetContractDetails(FlightDetial flightDetial)
        {
            try
            {
                if (Utility.IsValidGuid(flightDetial.Guid) && flightDetial.ContractId > 0)
                {

                    AirContext context = Utility.GetAirContextCache(flightDetial.Guid);
                    if (context != null && context.Availability != null && context.Availability.Contracts != null)
                    {
                        Contract contract = context.Availability.Contracts.Where<Contract>(o => o.ContractId == flightDetial.ContractId).FirstOrDefault<Contract>();
                        if (contract != null)
                        {
                            return Json(new { IsContractExist = true, HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, string.Format("~/views/flights/{0}partial/_contractdetails.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : ""), contract) });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Flight.GetContractDetails|Exception:" + ex.ToString());
            }
            return Json(new { IsContextExist = false }, JsonRequestBehavior.AllowGet);
        }


        [Route("reservations")]
        public ActionResult reservations()
        {
            return Utility.GetDeviceType(Request.UserAgent) ? View() : View();
        }

        [Route("flights/save-incomplete-booking")]
        [HttpPost]
        public ActionResult SaveIncompletePayment(FormCollection collection, BookingDetail model)
        {
            try
            {
                if (model != null)
                {
                    Task task = Task.Factory.StartNew(() =>
                    {
                        if (collection["guid"] != null)
                        {
                            string guid = collection["guid"].ToString();
                            AirContext _context = Utility.GetAirContextCache(guid);
                            if (_context != null)
                            {
                                CurrencyMaster currencyMaster = ShareUtility.GetCurrency(Request);
                                IncompleteBookingContext _bookingContext = Utility.GetIncompleteBookingCache(Utility.Settings.IncompletBookingKey);
                                if (_bookingContext == null)
                                {
                                    _bookingContext = new IncompleteBookingContext() { IncompleteBookings = new List<IncompleteBooking>() };
                                }
                                if (model.BillingDetails != null && !string.IsNullOrEmpty(model.BillingDetails.CardNumber))
                                {
                                    model.BillingDetails.CardNumber = model.BillingDetails.CardNumber.Replace(" ", "");
                                }
                                string key = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", guid, _context.Search.Origin, _context.Search.Destination, _context.Search.Departure.ToString("ddMMyyyy"), _context.Search.TripType, _context.SelectedContract.ContractId);
                                if (_bookingContext != null && _bookingContext.IncompleteBookings != null && _bookingContext.IncompleteBookings.Count > 0)
                                {

                                    var item = _bookingContext.IncompleteBookings.Where(o => o.Key.Equals(key)).FirstOrDefault();
                                    if (item != null)
                                    {
                                        item.BookingDetail.Travellers = model.Travellers;
                                        item.BookingDetail.BillingDetails = model.BillingDetails;
                                        item.BookingDetail.FlightSearch = _context.Search;
                                        item.BookingDetail.Contract = _context.SelectedContract;
                                        item.BookingDetail.TravelerInsurance = _context.BookingDetailRQ != null ? _context.BookingDetailRQ.TravelerInsurance : null;
                                        item.BookingDetail.CouponDetails = _context.BookingDetailRQ != null && _context.BookingDetailRQ.CouponDetails != null ? _context.BookingDetailRQ.CouponDetails : null;
                                        item.BookingDetail.ExtendedCancellation = _context.BookingDetailRQ != null && _context.BookingDetailRQ.ExtendedCancellation != null ? _context.BookingDetailRQ.ExtendedCancellation : null;
                                        item.BookingDetail.Currency = CurrencyType.USD;
                                        item.BookingDetail.CurrencyCode = currencyMaster.CurrencyType;
                                        item.BookingDetail.CurrencyConversion = currencyMaster.CurrencyPrice;
                                    }
                                    else
                                    {
                                        IncompleteBooking bookingDetailRQ = new IncompleteBooking();
                                        bookingDetailRQ.Key = key;
                                        bookingDetailRQ.BookingDetail = new BookingDetail()
                                        {
                                            Travellers = model.Travellers,
                                            BillingDetails = model.BillingDetails,
                                            FlightSearch = _context.Search,
                                            Contract = _context.SelectedContract,
                                            TravelerInsurance = _context.BookingDetailRQ != null ? _context.BookingDetailRQ.TravelerInsurance : null,
                                            CouponDetails = _context.BookingDetailRQ != null && _context.BookingDetailRQ.CouponDetails != null ? _context.BookingDetailRQ.CouponDetails : null,
                                            ExtendedCancellation = _context.BookingDetailRQ != null && _context.BookingDetailRQ.ExtendedCancellation != null ? _context.BookingDetailRQ.ExtendedCancellation : null,
                                            Currency = CurrencyType.USD,
                                            CurrencyCode = currencyMaster.CurrencyType,
                                            CurrencyConversion = currencyMaster.CurrencyPrice
                                        };
                                        _bookingContext.IncompleteBookings.Add(bookingDetailRQ);
                                    }
                                }
                                else
                                {
                                    IncompleteBooking bookingDetailRQ = new IncompleteBooking();
                                    bookingDetailRQ.Key = key;
                                    bookingDetailRQ.BookingDetail = new BookingDetail()
                                    {
                                        Travellers = model.Travellers,
                                        BillingDetails = model.BillingDetails,
                                        FlightSearch = _context.Search,
                                        Contract = _context.SelectedContract,
                                        TravelerInsurance = _context.BookingDetailRQ != null ? _context.BookingDetailRQ.TravelerInsurance : null,
                                        CouponDetails = _context.BookingDetailRQ != null && _context.BookingDetailRQ.CouponDetails != null ? _context.BookingDetailRQ.CouponDetails : null,
                                        ExtendedCancellation = _context.BookingDetailRQ != null && _context.BookingDetailRQ.ExtendedCancellation != null ? _context.BookingDetailRQ.ExtendedCancellation : null,
                                        Currency = CurrencyType.USD,
                                        CurrencyCode = currencyMaster.CurrencyType,
                                        CurrencyConversion = currencyMaster.CurrencyPrice
                                    };
                                    _bookingContext.IncompleteBookings.Add(bookingDetailRQ);
                                }


                                Utility.SetIncompleteBookingCache(Utility.Settings.IncompletBookingKey, _bookingContext);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("SaveIncompletePayment|Exception:" + ex.ToString());
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #region private
        private MaxMinValue GetMaxMinTime(ClientMaxMinValue _req)
        {
            MaxMinValue res = null;
            try
            {
                if (_req == null || (_req != null && _req.Min == 0 && _req.Max == 0) || _req.Max == 0)
                {
                    res = null;
                }
                else
                {
                    res = new MaxMinValue()
                    {
                        Min = TimeSpan.FromMinutes(_req.Min),
                        Max = TimeSpan.FromMinutes(_req.Max)
                    };
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("GetMaxMinTime:" + ex.ToString());
            }
            return res;
        }
        private List<AirlineFilter> GetFilterAirline(List<string> _lstAirline)
        {
            List<AirlineFilter> respinse = new List<AirlineFilter>();
            try
            {
                string[] tempArr;
                foreach (string item in _lstAirline)
                {
                    tempArr = item.Split('|');
                    if (tempArr.Length > 1)
                    {
                        respinse.Add(new AirlineFilter() { Airline = tempArr[0].ToUpper(), IsMultiAirline = tempArr[1].Equals("1") ? true : false });
                    }
                    else if (tempArr.Length == 1)
                    {
                        respinse.Add(new AirlineFilter() { Airline = tempArr[0].ToUpper(), IsMultiAirline = false });
                    }

                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("GetFilterAirline|Exception:" + ex.ToString());
            }
            return respinse;
        }
        private List<AirportFilter> GetFilterAirport(List<string> _lstAirport)
        {
            List<AirportFilter> respinse = new List<AirportFilter>();
            try
            {
                string[] tempArr;
                foreach (string item in _lstAirport)
                {
                    tempArr = item.Split('|');
                    respinse.Add(new AirportFilter() { Airport = tempArr[0].ToUpper() });
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("GetFilterAirport|Exception:" + ex.ToString());
            }
            return respinse;
        }
        private List<StopContractTypeFilter> GetFilterContractType(string _contract)
        {
            List<StopContractTypeFilter> respinse = new List<StopContractTypeFilter>();
            try
            {
                string[] tempArr;
                StopContractTypeFilter stopContractTypeFilter = null;
                if (!string.IsNullOrEmpty(_contract))
                {
                    tempArr = _contract.Split('|');
                    for (int i = 0; i < tempArr.Length; i++)
                    {
                        stopContractTypeFilter = new StopContractTypeFilter();
                        string[] stopContract = tempArr[i].Split('-');
                        stopContractTypeFilter.ContractType = (ContractType)Convert.ToInt32(stopContract[0]);
                        stopContractTypeFilter.StopsType = (StopsType)Convert.ToInt32(stopContract[1]);
                        respinse.Add(stopContractTypeFilter);
                    }

                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("GetFilterContractType|Exception:" + ex.ToString());
            }
            return respinse;
        }
        private ContractFilterRQ GetContractFilterRQ(ClientContractFilterRQ _request)
        {
            ContractFilterRQ res = null;
            if (_request != null)
            {
                res = new ContractFilterRQ();
                res.Guid = _request.Guid;
                res.Stops = (_request.Stops == null || _request.Stops.Count == 0) ? null : _request.Stops;
                res.Tab = _request.Tab;
                res.PageNumber = _request.PageNumber;
                res.Airlines = (_request.Airlines == null || _request.Airlines.Count == 0) ? null : GetFilterAirline(_request.Airlines);
                res.DepartureAirports = (_request.DepartureAirports == null || _request.DepartureAirports.Count == 0) ? null : GetFilterAirport(_request.DepartureAirports);
                res.ReturnAirports = (_request.ReturnAirports == null || _request.ReturnAirports.Count == 0) ? null : GetFilterAirport(_request.ReturnAirports);
                res.Price = new PriceFilter() { Min = _request.MinPrice, Max = _request.MaxPrice };
                res.OutboundDepTime = GetMaxMinTime(_request.OutboundDepTime);
                res.OutboundArrTime = GetMaxMinTime(_request.OutboundArrTime);
                res.OutboundDuration = GetMaxMinTime(_request.OutboundDuration);
                res.InboundDepTime = GetMaxMinTime(_request.InboundDepTime);
                res.InboundArrTime = GetMaxMinTime(_request.InboundArrTime);
                res.InboundDuration = GetMaxMinTime(_request.InboundDuration);
                if (_request.ApplyMatrixFilter != null && _request.ApplyMatrixFilter.IsMatrixFilter)
                {
                    res.ApplyMatrixFilter = new Infrastructure.HelpingModel.ApplyMatrixFilter()
                    {
                        StopContractTypes = GetFilterContractType(_request.ApplyMatrixFilter.ContractType),
                        IsAirlineClicked = _request.ApplyMatrixFilter.IsAirlineClicked
                    };
                }
            }
            return res;
        }
        private string GetUniqueId()
        {
            string UniqueGuid = string.Empty;
            if (RouteData.Values["id"] != null)
            {
                UniqueGuid = Convert.ToString(RouteData.Values["id"]);
            }
            return UniqueGuid;
        }
        #endregion

        #region Coupan Code 
        [HttpPost]
        [Route("flights/applypromocode")]
        public ActionResult ApplyPromocode(string id, string promoCode)
        {
            string partialView = string.Format("~/views/flights/{0}partial/_pricedetails.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");
            try
            {
                if (Utility.IsValidGuid(id))
                {
                    AirContext context = Utility.GetAirContextCache(id);
                    if (context != null && context.SelectedContract != null)
                    {
                        PromoCodeResponse response = null;
                        Coupon CouponDetails = null;
                        if (string.IsNullOrEmpty(promoCode) == false)
                        {
                            PromoCodeRequest promocode = new PromoCodeRequest { CouponCode = promoCode, BookingAmount = context.SelectedContract.TotalGDSFareV2, PortalId = Utility.PortalSettings.PortalId };
                            response = Utility.GetPromocodeDetails(promocode, context.SelectedContract);
                            CouponDetails = new Coupon()
                            {
                                CouponCode = promoCode,
                                Status = response.Status.IsSuccess,
                                CouponMessage = response.Status.Error != null ? response.Status.Error.Description : "",
                                TotalAmount = response.Status != null && response.Status.IsSuccess == true ? response.Result : 0,
                            };
                        }
                        else
                        {
                            if (context.BookingDetailRQ.CouponDetails.CouponCode != null && context.BookingDetailRQ.CouponDetails != null)
                            {
                                PromoCodeRequest promocode = new PromoCodeRequest { CouponCode = context.BookingDetailRQ.CouponDetails.CouponCode, BookingAmount = context.SelectedContract.TotalGDSFareV2, PortalId = Utility.PortalSettings.PortalId };
                                RESTClient.RemovePromocodeDetails(promocode);
                            }
                        }
                        context.BookingDetailRQ = new BookingDetail()
                        {
                            Contract = context.SelectedContract,
                            BagInsuranc = context.BookingDetailRQ != null && context.BookingDetailRQ.BagInsuranc != null ? context.BookingDetailRQ.BagInsuranc : null,
                            TravelerInsurance = context.BookingDetailRQ != null && context.BookingDetailRQ.TravelerInsurance != null ? context.BookingDetailRQ.TravelerInsurance : null,
                            CouponDetails = CouponDetails
                        };
                        if (response != null && response.Status != null && response.Status.IsSuccess == true)
                        {
                            SetCoupanAmmount(response.Result, (context.SelectedContract.Adult + context.SelectedContract.Child + context.SelectedContract.InfantOnLap + context.SelectedContract.InfantOnSeat + context.SelectedContract.Senior), context);
                        }
                        else
                        {
                            SetCoupanAmmount(0, (context.SelectedContract.Adult + context.SelectedContract.Child + context.SelectedContract.InfantOnLap + context.SelectedContract.InfantOnSeat + context.SelectedContract.Senior), context);
                        }

                        Utility.SetAirContextCache(id, context);
                        var result = new
                        {
                            IsSuccess = true,
                            HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, context.BookingDetailRQ)
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.ApplyPromocode|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }
        public void SetCoupanAmmount(float amount, int count, AirContext context)
        {
            float perpax = amount / count;
            if (context.BookingDetailRQ.Contract.AdultFare != null)
            {
                context.BookingDetailRQ.Contract.AdultFare.Discount = perpax;
            }
            if (context.BookingDetailRQ.Contract.SeniorFare != null)
            {
                context.BookingDetailRQ.Contract.SeniorFare.Discount = perpax;
            }
            if (context.BookingDetailRQ.Contract.ChildFare != null)
            {
                context.BookingDetailRQ.Contract.ChildFare.Discount = perpax;
            }
            if (context.BookingDetailRQ.Contract.InfantOnSeatFare != null)
            {
                context.BookingDetailRQ.Contract.InfantOnSeatFare.Discount = perpax;
            }
            if (context.BookingDetailRQ.Contract.InfantOnLapFare != null)
            {
                context.BookingDetailRQ.Contract.InfantOnLapFare.Discount = perpax;
            }
        }
        #endregion

        [HttpGet]
        public ActionResult GetCurrency(string id)
        {
            try
            {
                Currency currency = new Currency() { CurrencyPrice = 1.0M, CurrencySymbol = "$", CurrencyType = "USD", Id = 2 };
                List<Currency> currencies = CurrencyManager.GetCurrency();
                if (currencies != null && currencies.Count > 0)
                {
                    currency = currencies.Where(o => o.CurrencyType.ToUpper().Contains(id.ToUpper())).FirstOrDefault();
                }
                if (currency != null)
                {
                    ShareUtility.SetCurrencyCookies(currency, "crnCookie", Request);
                    var result = new
                    {
                        IsSuccess = true,
                        data = currency
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Flight.GetCurrency|Exception:", ex.ToString()));

            }

            return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("flights/getreviews")]
        public ActionResult GetReviews()
        {
            string partialView = string.Format("~/views/home/{0}partial/_reviews.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "" : "");
            try
            {
                var result = new
                {
                    IsSuccess = true,
                    HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, Utility.Reviews)
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.GetBageInsurancePrice|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("flights/getdeals")]
        public ActionResult GetDeals()
        {
            string partialView = string.Format("~/views/home/{0}partial/_deals.cshtml", Utility.GetDeviceType(Request.UserAgent) ? "mobile/" : "");
            try
            {
                HomeDeals homeDeals = DealBusness.GetHomeDeals(Utility.GetClientIP(System.Web.HttpContext.Current));
                var result = new
                {
                    IsSuccess = true,
                    HtmlResponse = ShareUtility.RenderViewToString(this.ControllerContext, partialView, homeDeals)
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.GetDeals|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("flights/getcurrency")]
        public ActionResult GetCurrency()
        {
            try
            {
                string ip = Utility.GetClientIP(System.Web.HttpContext.Current);
                IATAGeoLocation iATAGeoLocation = null;
                string currency = "USD";
                List<Task> lstTasks = new List<Task>
                        {
                            Task.Factory.StartNew(()=>{ iATAGeoLocation = Business.GeoLocation.GetAirportCode(ip); })
                        };
                Task.WaitAll(lstTasks.ToArray());
                string fromCode = iATAGeoLocation.IATACode;
                var cityAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(fromCode, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.PriorityIndex).FirstOrDefault();
                currency = cityAirport != null ? cityAirport.CurrencyCode : "USD";
                if (Request.Cookies["crnCookie"] == null || (Request.Cookies["crnCookie"] != null && Request.Cookies["crnCookie"].Value == null))
                {
                    List<Currency> currencies = CurrencyManager.GetCurrency();
                    Currency currencyName = currencies != null && currencies.Count > 0 ? currencies.Where(o => o.CurrencyType.ToUpper().Contains(currency.ToUpper())).FirstOrDefault() : null;
                    if (currencyName != null)
                    {
                        ShareUtility.SetCurrencyCookies(currencyName, "crnCookie", Request);
                    }
                    else
                    {
                        currency = "USD";
                        currencyName = new Currency() { CurrencyPrice = 1.0M, CurrencySymbol = "$", CurrencyType = "USD", Id = 2 };
                        ShareUtility.SetCurrencyCookies(currencyName, "crnCookie", Request);
                    }
                }
                else
                {
                    List<Currency> currencies = CurrencyManager.GetCurrency();
                    Currency currencyName = currencies != null && currencies.Count > 0 ? currencies.Where(o => o.CurrencyType.ToUpper().Contains(ShareUtility.GetCurrencyCodeFromCookies(Request).ToUpper())).FirstOrDefault() : null;
                    if (currencyName != null)
                    {
                        currency = currencyName.CurrencyType;
                        ShareUtility.SetCurrencyCookies(currencyName, "crnCookie", Request);
                    }
                }
                var result = new
                {
                    IsSuccess = true,
                    HtmlResponse = currency
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.GetCurrency|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("flights/naer-by-airport")]
        public ActionResult GetNearByAirport()
        {
            try
            {
                string ip = Utility.GetClientIP(System.Web.HttpContext.Current);
                IATAGeoLocation iATAGeoLocation = null;
                List<Task> lstTasks = new List<Task>
                        {
                            Task.Factory.StartNew(()=>{ iATAGeoLocation = Business.GeoLocation.GetAirportCode(ip); })
                        };
                Task.WaitAll(lstTasks.ToArray());
                string fromCode = iATAGeoLocation.IATACode;
                var cityAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(fromCode, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.PriorityIndex).FirstOrDefault();

                if (cityAirport != null)
                {
                    string sug = string.Format("{0} - {1}, {2}, {3}", cityAirport.AirportCode, cityAirport.AirportName, cityAirport.City, cityAirport.CountryName);
                    var result = new
                    {
                        IsSuccess = true,
                        HtmlResponse = new { OriginSearch = sug, Origin = cityAirport.AirportCode, OriginAirportName = cityAirport.AirportName, OriginCountry = cityAirport.CountryCode, IsMobile = Utility.GetDeviceType(Request.UserAgent) }
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        IsSuccess = false,
                        HtmlResponse = ""
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Flight.GetNearByAirport|Exception:" + ex.ToString());
            }
            var resultNotFound = new
            {
                IsSuccess = false,
                HtmlResponse = ""
            };
            return Json(resultNotFound, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("flights/sentitinerary")]
        public bool SentItinerary(string email, string itinGuid, int ContractId)
        {
            if (email != null)
            {
                try
                {
                    if (email != null)
                    {
                        if (Utility.IsValidGuid(itinGuid) && ContractId > 0)
                        {
                            AirContext context = Utility.GetAirContextCache(itinGuid);
                            if (context != null)
                            {
                                Contract contract = context.Availability.Contracts.Where<Contract>(o => o.ContractId == ContractId).FirstOrDefault<Contract>();
                                if (contract != null)
                                {
                                    string htmlMailString = ShareUtility.RenderViewToString(this.ControllerContext, "~/views/emails/SentItineraryEmail.cshtml", contract);

                                    if (!string.IsNullOrEmpty(htmlMailString) && htmlMailString.Length > 1000)
                                    {
                                        Task.Factory.StartNew(() =>
                                        {
                                            EmailTransaction transaction = new EmailTransaction()
                                            {
                                                EmailType = EmailType.ItineryDetails,
                                                MailBody = htmlMailString,
                                                PortalId = context.Search.PortalId,
                                                MailRecipient = email

                                            };
                                            bool isMailSent = EmailHelper.SendMails(transaction);
                                            RequestedItinerary requestedItinerary = new RequestedItinerary() { PortalId = context.Search.PortalId, Origin = context.Search.Origin, Destination = context.Search.Destination, TripType = (int)context.Search.TripType, Departure = context.Search.Departure, Return = context.Search.Return, Email = email, SentSuccess = isMailSent, IP = context.Search.IP };
                                            Operation.SentItineryDetails(requestedItinerary);
                                            Utility.Logger.Info(string.Format("Itinerary details|Email:{0} | {1}", email, isMailSent ? "MAIL SENT" : "UNABLE TO SENT MAIL"));

                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.Logger.Error("SentItinerary UNABLE to Send|EXCEPTION:" + ex.ToString());
                }
            }
            return true;
        }
    }

    public class ClientContractFilterRQ
    {
        public string Guid { get; set; }
        public int Tab { get; set; }
        public int PageNumber { get; set; }
        public List<string> Airlines { get; set; }
        public List<string> DepartureAirports { get; set; }
        public List<string> ReturnAirports { get; set; }
        public List<int> Stops { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public ClientMaxMinValue OutboundDepTime { get; set; }
        public ClientMaxMinValue OutboundArrTime { get; set; }
        public ClientMaxMinValue InboundDepTime { get; set; }
        public ClientMaxMinValue InboundArrTime { get; set; }
        public ClientMaxMinValue OutboundDuration { get; set; }
        public ClientMaxMinValue InboundDuration { get; set; }
        public ApplyMatrixFilter ApplyMatrixFilter { get; set; }
    }
    public class ClientMaxMinValue
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }
    public class ApplyMatrixFilter
    {
        public string ContractType { get; set; }
        public bool IsMatrixFilter { get; set; }
        public bool IsAirlineClicked { get; set; }
    }
    public class SaveContactData
    {
        public string Guid { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string IP { get; set; }
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }

    }
    public class FlightDetial
    {
        public string Guid { get; set; }
        public int ContractId { get; set; }
    }
    public class ProtectTrip
    {
        public string Guid { get; set; }
        public string Token { get; set; }
        public string CardType { get; set; }
        public decimal TotalAmount { get; set; }
    }
}