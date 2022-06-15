using Common;
using Database;
using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class AirBusiness
    {
        public void SearchFlight(FlightSearch _search)
        {
            try
            {
                if (!string.IsNullOrEmpty(_search.PreferredCarrier))
                {
                    Airlines airline = Utility.Airlines.Where<Airlines>(o => (_search.PreferredCarrier.Length == 2 && o.Code.Equals(_search.PreferredCarrier, StringComparison.OrdinalIgnoreCase)) || o.Name.Equals(_search.PreferredCarrier, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                    if (airline != null)
                    {
                        _search.PreferredCarrier = airline.Code;
                    }
                }

                _search.SearchDateTime = DateTime.UtcNow;
                //Task.Factory.StartNew(() => {
                //    Utility.FlightSearches.Add(_search);
                //    BookingInformation.SaveFlightSearches();
                //    });

                Response<Availability> response = RESTClient.GetAvailability(_search).Result;
                if (response != null && response.TransactionStatus != null && response.TransactionStatus.IsSuccess && response.Result != null && response.Result.Contracts.Count > 0)
                {
                    AirContext context = Utility.GetAirContextCache(_search.SearchGuidId);
                    if (context != null)
                    {
                        context.IsRequestCompleted = true;
                        //if (response != null && response.Result != null && response.Result.AirlineMatrixMain != null && response.Result.AirlineMatrixMain.AirlineMatrixList != null)
                        //{
                        //    response.Result.AirlineMatrixMain.AirlineMatrixList = response.Result.AirlineMatrixMain.AirlineMatrixList.OrderBy(o => o.GetMinimumAmount()).ToList<AirlineMatrixColumn>();
                        //}
                        context.Availability = response.Result;
                        Utility.SetAirContextCache(_search.SearchGuidId, context);
                    }
                    else
                    {
                        Utility.Logger.Info("AIRBUSINESS.SEARCHFLIGHT| SEARCH GUID NOT FOUND|Guid:" + _search.SearchGuidId);
                    }
                }
                else
                {
                    AirContext context = Utility.GetAirContextCache(_search.SearchGuidId);
                    context.IsRequestCompleted = true;
                    context.Availability = response != null ? response.Result : null;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("AirBusiness.SearchFlight|Exception:" + ex.ToString());
            }
        }

        public void CheckAvailavility(string id, int _contractId)
        {
            AirContext context = null;
            Contract contract = null;
            try
            {
                context = Utility.GetAirContextCache(id);
                if (context != null)
                {
                    if (context != null && context.Availability != null && context.Availability.Contracts != null && context.Availability.Contracts.Count() > 0)
                    {
                        contract = context.Availability.Contracts.Where(o => o.ContractId == _contractId).FirstOrDefault();
                        if (contract != null)
                        {
                            Response<Contract> response = RESTClient.CheckAvailability(contract).Result;
                            if (Utility.PortalSettings.IsCheckAvailability)
                            {
                                if (response != null && response.TransactionStatus != null && response.TransactionStatus.IsSuccess)
                                {
                                    context.Status = BookingStatus.InProgress;
                                    context.IsRequestCompleted = true;
                                    context.SelectedContract = contract;
                                }
                                else
                                {
                                    context.Status = BookingStatus.SoldOutOrUnavailable;
                                    context.IsRequestCompleted = true;
                                    response.Result.BookingStatus = BookingStatus.SoldOutOrUnavailable;
                                    context.SelectedContract = response.Result;
                                }
                                Utility.SetAirContextCache(context.Search.SearchGuidId, context);
                            }
                            else
                            {
                                context.IsRequestCompleted = true;
                                Utility.SetAirContextCache(context.Search.SearchGuidId, context);
                            }
                            if (response == null || (response != null && response.TransactionStatus == null) || (response != null && response.TransactionStatus != null && !response.TransactionStatus.IsSuccess))
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        BookingProcedures.LogsContractSoldout(context.SelectedContract, context.Search, Utility.ConnString);
                                    }
                                    catch (Exception ex)
                                    {
                                        Utility.Logger.Error("LogsContractSoldout|" + ex.ToString());
                                    }
                                });
                            }

                        }
                        else
                        {
                            Utility.Logger.Info("AIRBUSINESS.CHECKAVAILAVILITY|SEARCH GUID NOT FOUND|Guid:" + id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Status = BookingStatus.InProgress;
                context.IsRequestCompleted = true;
                context.SelectedContract = contract;
                Utility.SetAirContextCache(context.Search.SearchGuidId, context);
                Utility.Logger.Error("AirBusiness.CheckAvailavility|Exception:" + ex.ToString());
            }
        }

        public void BookFlight(BookingDetail _bookingDetail)
        {
            try
            {
                if (_bookingDetail != null)
                {
                    Response<FlightBookingRS> response = RESTClient.BookFlight(_bookingDetail);
                    if (response == null)
                    {
                        response = RESTClient.BookFlight(_bookingDetail);
                    }
                    AirContext context = Utility.GetAirContextCache(_bookingDetail.Contract.SearchGuid);
                    if (context != null)
                    {
                        if (response != null && response.Result != null)
                        {
                            #region Remove the cash data for incomplete booking
                            try
                            {
                                string key = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", _bookingDetail.Contract.SearchGuid, context.Search.Origin, context.Search.Destination, context.Search.Departure.ToString("ddMMyyyy"), context.Search.TripType, context.SelectedContract.ContractId);
                                Task.Factory.StartNew(() =>
                                {
                                    IncompleteBookingContext _bookingContext = Utility.GetIncompleteBookingCache(Utility.Settings.IncompletBookingKey);
                                    if (_bookingContext != null && _bookingContext.IncompleteBookings != null && _bookingContext.IncompleteBookings.Count > 0)
                                    {
                                        var bookedItem = _bookingContext.IncompleteBookings.Where(o => o.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                        if (bookedItem != null)
                                        {
                                            _bookingContext.IncompleteBookings.Remove(bookedItem);
                                            Utility.SetIncompleteBookingCache(Utility.Settings.IncompletBookingKey, _bookingContext);
                                        }
                                    }
                                });
                            }
                            catch { }
                            #endregion

                            context.IsRequestCompleted = true;
                            context.FlightBookingRS = response.Result;
                            Utility.SetAirContextCache(_bookingDetail.Contract.SearchGuid, context);

                        }
                        else
                        {
                            _bookingDetail.Contract.BookingStatus = BookingStatus.SoldOutOrUnavailable;
                            context.IsRequestCompleted = true;
                            response = new Response<FlightBookingRS>()
                            {
                                Result = new FlightBookingRS()
                                {
                                    Contract = _bookingDetail.Contract,
                                    BookingStatus = BookingStatus.SoldOutOrUnavailable
                                },
                                TransactionStatus = new TransactionStatus() { IsSuccess = false, Error = new Error() { Code = "404", Description = "Soldout" } }
                            };
                            context.FlightBookingRS = response.Result;
                            Utility.SetAirContextCache(_bookingDetail.Contract.SearchGuid, context);
                        }
                        BookingLogsSoldoutPriceChange(response, _bookingDetail);
                    }
                    else
                    {
                        Utility.Logger.Info("BOOKING CONTEXT NULL|GUID:" + _bookingDetail.Contract.SearchGuid);
                    }
                }
                else
                {
                    Utility.Logger.Info("BOOK FLIGHT| BookingDetail=NULL");
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("AirBusiness.BookFlight|Exception:" + ex.ToString());
            }
        }
        private static void PrepareTripStopsDetails(int _stopCount, ContractFacets _ontractFacets)
        {
            StopType stopType = StopType.Multi_Stop1 | StopType.Multi_Stop2 | StopType.Multi_Stop3 | StopType.Multi_Stop4 | StopType.Multi_Stop5;
            if (!_ontractFacets.Stops.Exists(x => x.Key.Equals(_stopCount.ToString())))
            {
                if (Enum.GetName(typeof(StopType), Convert.ToByte(_stopCount)) != null)
                {
                    _ontractFacets.Stops.Add(new KeyValueData { Key = _stopCount.ToString(), Value = Utility.GetEnumDescription((StopType)_stopCount), IsSelected = true });
                }
                else
                {
                    if (stopType == StopType.Multi_Stops)
                    {
                        _ontractFacets.Stops.Add(new KeyValueData { Key = _stopCount.ToString(), Value = Utility.GetEnumDescription(StopType.Multi_Stops), IsSelected = true });
                    }
                }
            }

        }
        public static void PrepareFilter(Availability _availability, ContractFacets _ontractFacets)
        {
            try
            {
                _ontractFacets.TripType = _ontractFacets.Search.TripType;
                _ontractFacets.Origin = new DepartureArrival()
                {
                    Origin = _ontractFacets.Search.Origin,
                    Destination = _ontractFacets.Search.Destination
                };
                _ontractFacets.MinPrice = _availability.Contracts.Min(c => c.AdultFare.TotalFarePPax);
                _ontractFacets.MaxPrice = _availability.Contracts.Max(c => c.AdultFare.TotalFarePPax);
                var stops = _availability.Contracts.Select(o => o.GetStopType()).Distinct();
                if (stops != null)
                {
                    foreach (int item in stops)
                    {
                        float minPrice = 0;
                        try
                        {
                            minPrice = _availability.Contracts.Where(o => o.GetStopType() == item).Min(o => o.AdultFare.TotalFarePPax);
                        }
                        catch { }

                        StopType stopType = StopType.Multi_Stop1 | StopType.Multi_Stop2 | StopType.Multi_Stop3 | StopType.Multi_Stop4 | StopType.Multi_Stop5;
                        if (!_ontractFacets.Stops.Exists(x => x.Key.Equals(item.ToString())))
                        {
                            if (Enum.GetName(typeof(StopType), Convert.ToByte(item)) != null)
                            {
                                _ontractFacets.Stops.Add(new KeyValueData { Key = item.ToString(), Value = Utility.GetEnumDescription((StopType)item), IsSelected = true, MinPrice = minPrice });
                            }
                            else
                            {
                                if (stopType == StopType.Multi_Stops)
                                {
                                    _ontractFacets.Stops.Add(new KeyValueData { Key = item.ToString(), Value = Utility.GetEnumDescription(StopType.Multi_Stops), IsSelected = true, MinPrice = minPrice });
                                }
                            }
                        }
                    }
                }

                TimeSpan minTime = _availability.Contracts.Min(c => c.TripDetails.OutBoundSegment[0].DepartureTime);
                TimeSpan maxTime = _availability.Contracts.Max(c => c.TripDetails.OutBoundSegment[0].DepartureTime);
                _ontractFacets.OutboundDepTime = new DepartureArrivalTime() { Min = (int)minTime.TotalMinutes, Max = (int)maxTime.TotalMinutes };

                minTime = _availability.Contracts.Min(c => c.TripDetails.OutBoundSegment[c.TripDetails.OutBoundSegment.Count - 1].ArrivalTime);
                maxTime = _availability.Contracts.Max(c => c.TripDetails.OutBoundSegment[c.TripDetails.OutBoundSegment.Count - 1].ArrivalTime);
                _ontractFacets.OutboundArrTime = new DepartureArrivalTime() { Min = (int)minTime.TotalMinutes, Max = (int)maxTime.TotalMinutes };

                minTime = _availability.Contracts.Min(o => o.TotalOutBoundFlightDuration);
                maxTime = _availability.Contracts.Max(o => o.TotalOutBoundFlightDuration);
                _ontractFacets.OutboundDuration = new DepartureReturnDuration() { Min = (int)minTime.TotalMinutes, Max = (int)maxTime.TotalMinutes };

                if (_ontractFacets.Search.TripType == TripType.ROUNDTRIP)
                {
                    _ontractFacets.Return = new DepartureArrival()
                    {
                        Origin = _ontractFacets.Search.Destination,
                        Destination = _ontractFacets.Search.Origin
                    };
                    minTime = _availability.Contracts.Min(c => c.TripDetails.InBoundSegment[0].DepartureTime);
                    maxTime = _availability.Contracts.Max(c => c.TripDetails.InBoundSegment[0].DepartureTime);
                    _ontractFacets.InboundDepTime = new DepartureArrivalTime() { Min = (int)minTime.TotalMinutes, Max = (int)maxTime.TotalMinutes };

                    minTime = _availability.Contracts.Min(c => c.TripDetails.InBoundSegment[c.TripDetails.InBoundSegment.Count - 1].ArrivalTime);
                    maxTime = _availability.Contracts.Max(c => c.TripDetails.InBoundSegment[c.TripDetails.InBoundSegment.Count - 1].ArrivalTime);
                    _ontractFacets.InboundArrTime = new DepartureArrivalTime() { Min = (int)minTime.TotalMinutes, Max = (int)maxTime.TotalMinutes };


                    minTime = _availability.Contracts.Min(o => o.TotalInBoundFlightDuration);
                    maxTime = _availability.Contracts.Max(o => o.TotalInBoundFlightDuration);
                    _ontractFacets.InboundDuration = new DepartureReturnDuration() { Min = (int)minTime.TotalMinutes, Max = (int)maxTime.TotalMinutes };
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("AirBusiness.PrepareFilter|Exception:" + ex.ToString());
            }
        }

        public static bool IsAirlineExist(string _code, bool _isMultiAirline, List<AirlineFilter> _airlines)
        {
            if (_airlines.Where(o => o.Airline.Equals(_code, StringComparison.OrdinalIgnoreCase) && o.IsMultiAirline == _isMultiAirline).FirstOrDefault() != null)
            {
                return true;
            }
            // && o.IsMultiAirline == _isMultiAirline
            return false;
        }
        public static bool IsAirportExist(string _code, List<AirportFilter> _airports)
        {
            if (_airports.Where(o => o.Airport.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null)
            {
                return true;
            }
            return false;
        }
        public static Response<List<Contract>> FilterContracts(AirContext _context, ContractFilterRQ _request, out int totalResult, bool _isFirstLoad = false)
        {
            Response<List<Contract>> response = null;
            totalResult = 0;
            try
            {
                List<Contract> listContracts = null;
                List<Contract> matFiltrContracts = null;
                int pageSize = Utility.Settings.ListingPageSize;
                int skip = (_request.PageNumber - 1) * pageSize;


                switch (_request.Tab)
                {
                    case 0:/*All flights*/
                        listContracts = _context.Availability.Contracts.Where(o => o.ContractType == ContractType.Actual && !o.IsPhoneOnly
                            && (_request.Stops == null || (_request.Stops != null && _request.Stops.Count == 0) || (_request.Stops != null && _request.Stops.Contains(o.GetStopType())))
                            && (_request.Price == null || (_request.Price != null && o.AdultFare.TotalFarePPax >= _request.Price.Min && o.AdultFare.TotalFarePPax <= _request.Price.Max))
                            && (_request.OutboundDepTime == null || (o.TripDetails.OutBoundSegment[0].DepartureTime >= _request.OutboundDepTime.Min && o.TripDetails.OutBoundSegment[0].DepartureTime <= _request.OutboundDepTime.Max))
                            && (_request.OutboundArrTime == null || (o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime >= _request.OutboundArrTime.Min && o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime <= _request.OutboundArrTime.Max))
                            && (_request.OutboundDuration == null || (o.TotalOutBoundFlightDuration >= _request.OutboundDuration.Min && o.TotalOutBoundFlightDuration <= _request.OutboundDuration.Max))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP
                            && (_request.InboundDepTime == null || (o.TripDetails.InBoundSegment[0].DepartureTime >= _request.InboundDepTime.Min && o.TripDetails.InBoundSegment[0].DepartureTime <= _request.InboundDepTime.Max))
                            && (_request.InboundArrTime == null || (o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime >= _request.InboundArrTime.Min && o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime <= _request.InboundArrTime.Max))
                            && (_request.InboundDuration == null || (o.TotalInBoundFlightDuration >= _request.InboundDuration.Min && o.TotalInBoundFlightDuration <= _request.InboundDuration.Max))
                            ))
                            && (_request.Airlines == null || (_request.Airlines != null && IsAirlineExist(o.ValidatingCarrier.Code, o.IsMultipleAirlineContract, _request.Airlines)))
                            && (_request.DepartureAirports == null || (_request.DepartureAirports != null && IsAirportExist(o.TripDetails.OutBoundSegment[0].Origin, _request.DepartureAirports)))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP && (_request.ReturnAirports == null || (_request.ReturnAirports != null && IsAirportExist(o.TripDetails.InBoundSegment[0].Origin, _request.ReturnAirports)))))).ToList<Contract>();
                        if (listContracts != null && _request.Airlines == null && _request.Stops == null)
                        {
                            totalResult = listContracts.Count;
                            if (listContracts != null && listContracts.Count != 0)
                            {
                                listContracts = listContracts.Skip(skip).Take(pageSize).OrderBy(o => o.AdultFare.TotalFareV2).ToList<Contract>();
                            }
                            if ((_request.PageNumber == 1 || _request.PageNumber == 0) && _request.OutboundDepTime == null && _request.InboundDepTime == null)
                            {
                                //Contract contract = listContracts[0];
                                //List<Contract> contracts = _context.Availability.Contracts.Where(o => (o.IsPhoneOnly || (o.ContractType == ContractType.Flexi || o.ContractType == ContractType.NearBy || o.ContractType == ContractType.NearByFlexi)) && o.AdultFare.TotalFareV2 < contract.AdultFare.TotalFareV2).OrderBy(o => o.AdultFare.TotalFareV2).ToList<Contract>();

                                //if (contracts != null && contracts.Count > 0)
                                //{
                                //    //if ((listContracts.Count == 0 || (contract.AdultFare.TotalFareV2 + Utility.Settings.MarginActualWithAlternateNearBy) < listContracts[0].AdultFare.TotalFareV2))
                                //    //{
                                //    //    listContracts.Insert(0, contract);
                                //    //    totalResult = totalResult + 1;
                                //    //}
                                //    //foreach (var item in contracts.OrderBy(o => o.AdultFare.TotalFareV2).Take(2).OrderByDescending(o => o.AdultFare.TotalFareV2))
                                //    //{
                                //    //    listContracts.Insert(0, item);
                                //    //    totalResult = totalResult + 1;
                                //    //}
                                //}
                                Contract contract = _context.Availability.Contracts.Where(o => o.IsPhoneOnly || (o.ContractType == ContractType.Flexi || o.ContractType == ContractType.NearBy || o.ContractType == ContractType.NearByFlexi)).OrderBy(o => o.AdultFare.TotalFareV2).FirstOrDefault<Contract>();
                                if (contract != null)
                                {
                                    if ((listContracts.Count == 0 || (contract.AdultFare.TotalFareV2 + Utility.Settings.MarginActualWithAlternateNearBy) < listContracts[0].AdultFare.TotalFareV2))
                                    {
                                        listContracts.Insert(0, contract);
                                        totalResult = totalResult + 1;
                                    }
                                }                                
                            }


                        }
                        else if (listContracts != null && listContracts.Count != 0)
                        {
                            totalResult = listContracts.Count;
                            listContracts = listContracts.Skip(skip).Take(pageSize).OrderBy(o => o.AdultFare.TotalFareV2).ToList<Contract>();
                        }
                        break;
                    case 1:/*Shortest direct flights*/
                        listContracts = _context.Availability.Contracts.Where(o => o.ContractType == ContractType.Actual && !o.IsPhoneOnly
                             && (_request.Stops == null || (_request.Stops != null && _request.Stops.Count == 0) || (_request.Stops != null && _request.Stops.Contains(o.GetStopType())))
                            && (_request.Price == null || (_request.Price != null && o.AdultFare.TotalFarePPax >= _request.Price.Min && o.AdultFare.TotalFarePPax <= _request.Price.Max))
                            && (_request.OutboundDepTime == null || (o.TripDetails.OutBoundSegment[0].DepartureTime >= _request.OutboundDepTime.Min && o.TripDetails.OutBoundSegment[0].DepartureTime <= _request.OutboundDepTime.Max))
                            && (_request.OutboundArrTime == null || (o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime >= _request.OutboundArrTime.Min && o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime <= _request.OutboundArrTime.Max))
                            && (_request.OutboundDuration == null || (o.TotalOutBoundFlightDuration >= _request.OutboundDuration.Min && o.TotalOutBoundFlightDuration <= _request.OutboundDuration.Max))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP
                            && (_request.InboundDepTime == null || (o.TripDetails.InBoundSegment[0].DepartureTime >= _request.InboundDepTime.Min && o.TripDetails.InBoundSegment[0].DepartureTime <= _request.InboundDepTime.Max))
                            && (_request.InboundArrTime == null || (o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime >= _request.InboundArrTime.Min && o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime <= _request.InboundArrTime.Max))
                            && (_request.InboundDuration == null || (o.TotalInBoundFlightDuration >= _request.InboundDuration.Min && o.TotalInBoundFlightDuration <= _request.InboundDuration.Max))
                            ))
                            && (_request.Airlines == null || (_request.Airlines != null && IsAirlineExist(o.ValidatingCarrier.Code, o.IsMultipleAirlineContract, _request.Airlines)))
                            && (_request.DepartureAirports == null || (_request.DepartureAirports != null && IsAirportExist(o.TripDetails.OutBoundSegment[0].Origin, _request.DepartureAirports)))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP && (_request.ReturnAirports == null || (_request.ReturnAirports != null && IsAirportExist(o.TripDetails.InBoundSegment[0].Origin, _request.ReturnAirports)))))).ToList<Contract>();
                        if (listContracts != null)
                        {
                            totalResult = listContracts.Count;
                            listContracts = listContracts.Skip(skip).Take(pageSize).OrderBy(o => (o.TotalOutBoundFlightDuration + o.TotalInBoundFlightDuration)).ToList<Contract>();
                        }
                        break;
                    case 2:/*Near by flights*/
                        listContracts = _context.Availability.Contracts.Where(o => (o.ContractType == ContractType.NearBy || o.ContractType == ContractType.NearByFlexi) && !o.IsPhoneOnly
                             && (_request.Stops == null || (_request.Stops != null && _request.Stops.Count == 0) || (_request.Stops != null && _request.Stops.Contains(o.GetStopType())))
                            && (_request.Price == null || (_request.Price != null && o.AdultFare.TotalFarePPax >= _request.Price.Min && o.AdultFare.TotalFarePPax <= _request.Price.Max))
                            && (_request.OutboundDepTime == null || (o.TripDetails.OutBoundSegment[0].DepartureTime >= _request.OutboundDepTime.Min && o.TripDetails.OutBoundSegment[0].DepartureTime <= _request.OutboundDepTime.Max))
                            && (_request.OutboundArrTime == null || (o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime >= _request.OutboundArrTime.Min && o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime <= _request.OutboundArrTime.Max))
                            && (_request.OutboundDuration == null || (o.TotalOutBoundFlightDuration >= _request.OutboundDuration.Min && o.TotalOutBoundFlightDuration <= _request.OutboundDuration.Max))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP
                            && (_request.InboundDepTime == null || (o.TripDetails.InBoundSegment[0].DepartureTime >= _request.InboundDepTime.Min && o.TripDetails.InBoundSegment[0].DepartureTime <= _request.InboundDepTime.Max))
                            && (_request.InboundArrTime == null || (o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime >= _request.InboundArrTime.Min && o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime <= _request.InboundArrTime.Max))
                            && (_request.InboundDuration == null || (o.TotalInBoundFlightDuration >= _request.InboundDuration.Min && o.TotalInBoundFlightDuration <= _request.InboundDuration.Max))
                            ))
                            && (_request.Airlines == null || (_request.Airlines != null && IsAirlineExist(o.ValidatingCarrier.Code, o.IsMultipleAirlineContract, _request.Airlines)))
                            && (_request.DepartureAirports == null || (_request.DepartureAirports != null && IsAirportExist(o.TripDetails.OutBoundSegment[0].Origin, _request.DepartureAirports)))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP && (_request.ReturnAirports == null || (_request.ReturnAirports != null && IsAirportExist(o.TripDetails.InBoundSegment[0].Origin, _request.ReturnAirports)))))).ToList<Contract>();
                        if (listContracts != null)
                        {
                            totalResult = listContracts.Count;
                            listContracts = listContracts.Skip(skip).Take(pageSize).OrderBy(o => o.AdultFare.TotalFareV2).ToList<Contract>();
                        }
                        break;
                    case 3:/*Flexi flights*/
                        listContracts = _context.Availability.Contracts.Where(o => (o.ContractType == ContractType.Flexi || o.ContractType == ContractType.NearByFlexi) && !o.IsPhoneOnly
                           && (_request.Stops == null || (_request.Stops != null && _request.Stops.Count == 0) || (_request.Stops != null && _request.Stops.Contains(o.GetStopType())))
                            && (_request.Price == null || (_request.Price != null && o.AdultFare.TotalFarePPax >= _request.Price.Min && o.AdultFare.TotalFarePPax <= _request.Price.Max))
                            && (_request.OutboundDepTime == null || (o.TripDetails.OutBoundSegment[0].DepartureTime >= _request.OutboundDepTime.Min && o.TripDetails.OutBoundSegment[0].DepartureTime <= _request.OutboundDepTime.Max))
                            && (_request.OutboundArrTime == null || (o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime >= _request.OutboundArrTime.Min && o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime <= _request.OutboundArrTime.Max))
                            && (_request.OutboundDuration == null || (o.TotalOutBoundFlightDuration >= _request.OutboundDuration.Min && o.TotalOutBoundFlightDuration <= _request.OutboundDuration.Max))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP
                            && (_request.InboundDepTime == null || (o.TripDetails.InBoundSegment[0].DepartureTime >= _request.InboundDepTime.Min && o.TripDetails.InBoundSegment[0].DepartureTime <= _request.InboundDepTime.Max))
                            && (_request.InboundArrTime == null || (o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime >= _request.InboundArrTime.Min && o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime <= _request.InboundArrTime.Max))
                            && (_request.InboundDuration == null || (o.TotalInBoundFlightDuration >= _request.InboundDuration.Min && o.TotalInBoundFlightDuration <= _request.InboundDuration.Max))
                            ))
                            && (_request.Airlines == null || (_request.Airlines != null && IsAirlineExist(o.ValidatingCarrier.Code, o.IsMultipleAirlineContract, _request.Airlines)))
                            && (_request.DepartureAirports == null || (_request.DepartureAirports != null && IsAirportExist(o.TripDetails.OutBoundSegment[0].Origin, _request.DepartureAirports)))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP && (_request.ReturnAirports == null || (_request.ReturnAirports != null && IsAirportExist(o.TripDetails.InBoundSegment[0].Origin, _request.ReturnAirports)))))).ToList<Contract>();
                        if (listContracts != null)
                        {
                            totalResult = listContracts.Count;
                            listContracts = listContracts.Skip(skip).Take(pageSize).OrderBy(o => o.AdultFare.TotalFareV2).ToList<Contract>();
                        }
                        break;
                    case 5:/*Phone only flights*/
                        listContracts = _context.Availability.Contracts.Where(o => (o.IsPhoneOnly)
                           && (_request.Stops == null || (_request.Stops != null && _request.Stops.Count == 0) || (_request.Stops != null && _request.Stops.Contains(o.GetStopType())))
                            && (_request.Price == null || (_request.Price != null && o.AdultFare.TotalFarePPax >= _request.Price.Min && o.AdultFare.TotalFarePPax <= _request.Price.Max))
                            && (_request.OutboundDepTime == null || (o.TripDetails.OutBoundSegment[0].DepartureTime >= _request.OutboundDepTime.Min && o.TripDetails.OutBoundSegment[0].DepartureTime <= _request.OutboundDepTime.Max))
                            && (_request.OutboundArrTime == null || (o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime >= _request.OutboundArrTime.Min && o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime <= _request.OutboundArrTime.Max))
                            && (_request.OutboundDuration == null || (o.TotalOutBoundFlightDuration >= _request.OutboundDuration.Min && o.TotalOutBoundFlightDuration <= _request.OutboundDuration.Max))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP
                            && (_request.InboundDepTime == null || (o.TripDetails.InBoundSegment[0].DepartureTime >= _request.InboundDepTime.Min && o.TripDetails.InBoundSegment[0].DepartureTime <= _request.InboundDepTime.Max))
                            && (_request.InboundArrTime == null || (o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime >= _request.InboundArrTime.Min && o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime <= _request.InboundArrTime.Max))
                            && (_request.InboundDuration == null || (o.TotalInBoundFlightDuration >= _request.InboundDuration.Min && o.TotalInBoundFlightDuration <= _request.InboundDuration.Max))
                            ))
                            && (_request.Airlines == null || (_request.Airlines != null && IsAirlineExist(o.ValidatingCarrier.Code, o.IsMultipleAirlineContract, _request.Airlines)))
                            && (_request.DepartureAirports == null || (_request.DepartureAirports != null && IsAirportExist(o.TripDetails.OutBoundSegment[0].Origin, _request.DepartureAirports)))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP && (_request.ReturnAirports == null || (_request.ReturnAirports != null && IsAirportExist(o.TripDetails.InBoundSegment[0].Origin, _request.ReturnAirports)))))).ToList<Contract>();
                        if (listContracts != null)
                        {
                            totalResult = listContracts.Count;
                            listContracts = listContracts.Skip(skip).Take(pageSize).OrderBy(o => o.AdultFare.TotalFareV2).ToList<Contract>();
                        }
                        break;
                    default:
                        break;
                }

                if (_request.ApplyMatrixFilter != null)
                {
                    matFiltrContracts = new List<Contract>();
                    foreach (var item in _request.ApplyMatrixFilter.StopContractTypes)
                    {
                        Contract contractfltr = _context.Availability.Contracts.Where(o => o.ContractType == item.ContractType
                        && (_request.Tab != (int)Tabs.FlexFlights || (_request.Tab == (int)Tabs.FlexFlights && (o.ContractType == ContractType.Flexi || o.ContractType == ContractType.NearByFlexi)))
                        && (_request.Tab != (int)Tabs.NearbyFlights || (_request.Tab == (int)Tabs.NearbyFlights && (o.ContractType == ContractType.NearBy)))
                        && (_request.Price == null || (_request.Price != null && o.AdultFare.TotalFarePPax >= _request.Price.Min && o.AdultFare.TotalFarePPax <= _request.Price.Max))
                        && (_request.OutboundDepTime == null || (o.TripDetails.OutBoundSegment[0].DepartureTime >= _request.OutboundDepTime.Min && o.TripDetails.OutBoundSegment[0].DepartureTime <= _request.OutboundDepTime.Max))
                            && (_request.OutboundArrTime == null || (o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime >= _request.OutboundArrTime.Min && o.TripDetails.OutBoundSegment[o.TripDetails.OutBoundSegment.Count - 1].ArrivalTime <= _request.OutboundArrTime.Max))
                            && (_request.OutboundDuration == null || (o.TotalOutBoundFlightDuration >= _request.OutboundDuration.Min && o.TotalOutBoundFlightDuration <= _request.OutboundDuration.Max))
                            && (o.TripType == TripType.ONEWAY || (o.TripType == TripType.ROUNDTRIP
                            && (_request.InboundDepTime == null || (o.TripDetails.InBoundSegment[0].DepartureTime >= _request.InboundDepTime.Min && o.TripDetails.InBoundSegment[0].DepartureTime <= _request.InboundDepTime.Max))
                            && (_request.InboundArrTime == null || (o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime >= _request.InboundArrTime.Min && o.TripDetails.InBoundSegment[o.TripDetails.InBoundSegment.Count - 1].ArrivalTime <= _request.InboundArrTime.Max))
                            && (_request.InboundDuration == null || (o.TotalInBoundFlightDuration >= _request.InboundDuration.Min && o.TotalInBoundFlightDuration <= _request.InboundDuration.Max))
                            ))
                        && ((int)item.StopsType == o.GetStopType())
                        && ((_request.Airlines == null || _request.Airlines.Count == 0) || (_request.Airlines != null && o.ValidatingCarrier.Code.Equals(_request.Airlines[0].Airline, StringComparison.OrdinalIgnoreCase) && o.IsMultipleAirlineContract == _request.Airlines[0].IsMultiAirline))
                        ).OrderBy(o => o.AdultFare.TotalFareV2).FirstOrDefault();
                        if (contractfltr != null)
                        {
                            if (_request.ApplyMatrixFilter.IsAirlineClicked && listContracts != null && listContracts.Count > 0)
                            {
                                if (contractfltr.AdultFare.TotalFarePPax < listContracts[0].AdultFare.TotalFarePPax)
                                {
                                    matFiltrContracts.Add(contractfltr);
                                }
                            }
                            else
                            {
                                matFiltrContracts.Add(contractfltr);
                            }
                        }
                    }
                }


                if (matFiltrContracts != null && matFiltrContracts.Count > 0)
                {
                    Contract matContract = matFiltrContracts.OrderBy(o => o.AdultFare.TotalFareV2).FirstOrDefault();
                    if (listContracts != null && listContracts.Count > 0)
                    {
                        listContracts.Remove(matContract);
                        listContracts.Insert(0, matContract);
                    }
                    else if (listContracts != null && listContracts.Count == 0)
                    {
                        totalResult = 1;
                        listContracts.Add(matContract);
                    }
                }
                if (listContracts != null && listContracts.Count > 0)
                {
                    response = new Response<List<Contract>>()
                    {
                        TransactionStatus = new TransactionStatus() { IsSuccess = true },
                        Result = listContracts,

                    };
                }
                else
                {
                    response = new Response<List<Contract>>()
                    {
                        TransactionStatus = new TransactionStatus() { IsSuccess = false, Error = new Error() { Code = "404", Description = "Contract not found." } },
                        Result = null
                    };
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("FilterContracts:Exception:" + ex.ToString());
            }
            return response;
        }

        public static void PreparePaymentModel(BookingDetail _bookingDetail, AirContext _context)
        {
            try
            {
                if (_context.FlightBookingRS != null && _context.FlightBookingRS.BookingStatus == BookingStatus.BookWithHigherPriceChanged)
                {
                    _bookingDetail.Contract = _context.FlightBookingRS.Contract;
                    _bookingDetail.BillingDetails = _context.BookingDetailRQ.BillingDetails;
                    _bookingDetail.Travellers = _context.BookingDetailRQ.Travellers;
                    _bookingDetail.FlightSearch = _context.BookingDetailRQ.FlightSearch;
                    _bookingDetail.PriceIncrease = _context.FlightBookingRS.FareDifference;
                    _bookingDetail.BagInsuranc = _context.BookingDetailRQ.BagInsuranc != null ? _context.BookingDetailRQ.BagInsuranc : Utility.GetBagInsuranceType((int)BagInsuranceType.NONE, (_bookingDetail.Contract.Adult + _bookingDetail.Contract.Child + _bookingDetail.Contract.InfantOnLap + _bookingDetail.Contract.InfantOnSeat + _bookingDetail.Contract.Senior));
                    _bookingDetail.TravelerInsurance = _context.BookingDetailRQ.TravelerInsurance != null ? _context.BookingDetailRQ.TravelerInsurance : null;
                    _bookingDetail.CouponDetails = _context.BookingDetailRQ != null && _context.BookingDetailRQ.CouponDetails != null ? _context.BookingDetailRQ.CouponDetails : null;

                }
                else
                {
                    _bookingDetail.Contract = _context.SelectedContract;
                    _bookingDetail.FlightSearch = _context.Search;
                    _bookingDetail.BagInsuranc = _context.BookingDetailRQ != null && _context.BookingDetailRQ.BagInsuranc != null ? _context.BookingDetailRQ.BagInsuranc : Utility.GetBagInsuranceType((int)BagInsuranceType.NONE, (_bookingDetail.Contract.Adult + _bookingDetail.Contract.Child + _bookingDetail.Contract.InfantOnLap + _bookingDetail.Contract.InfantOnSeat + _bookingDetail.Contract.Senior));
                    _bookingDetail.TravelerInsurance = _context.BookingDetailRQ != null && _context.BookingDetailRQ.TravelerInsurance != null ? _context.BookingDetailRQ.TravelerInsurance : null;
                    _bookingDetail.CouponDetails = _context.BookingDetailRQ != null && _context.BookingDetailRQ.CouponDetails != null ? _context.BookingDetailRQ.CouponDetails : null;
                    _bookingDetail.BillingDetails = new BillingDetail()
                    {
                        Country = "US",
                        State = "AK",
                        CardType = (int)PaymentMethod.None,
                    };
                    _bookingDetail.Travellers = new List<Traveller>();
                    int adults = _context.SelectedContract.Adult + _context.SelectedContract.Senior;
                    int childs = _context.SelectedContract.Child;
                    int infantOnSeat = _context.SelectedContract.InfantOnSeat;
                    int infantOnLap = _context.SelectedContract.InfantOnLap;
                    int order = 1;
                    if (adults > 0)
                    {
                        for (int i = 1; i <= adults; i++)
                        {
                            _bookingDetail.Travellers.Add(new Traveller() { PaxOrderId = order, PaxType = (int)TravellerPaxType.ADT, Title = (int)TravellerTitleType.None, DOBMonth = 0, Gender = (int)GenderType.None, FirstName = "", LastName = "", MiddleName = "" });
                            order++;
                        }
                    }
                    if (childs > 0)
                    {
                        for (int i = 1; i <= childs; i++)
                        {
                            _bookingDetail.Travellers.Add(new Traveller() { PaxOrderId = order, PaxType = (int)TravellerPaxType.CHD, Title = (int)TravellerTitleType.None, DOBMonth = 0, Gender = (int)GenderType.None, FirstName = "", LastName = "", MiddleName = "" });
                            order++;
                        }
                    }
                    if (infantOnSeat > 0)
                    {
                        for (int i = 1; i <= infantOnSeat; i++)
                        {
                            _bookingDetail.Travellers.Add(new Traveller() { PaxOrderId = order, PaxType = (int)TravellerPaxType.INS, Title = (int)TravellerTitleType.None, DOBMonth = 0, Gender = (int)GenderType.None, FirstName = "", LastName = "", MiddleName = "" });
                            order++;
                        }
                    }
                    if (infantOnLap > 0)
                    {
                        for (int i = 1; i <= infantOnLap; i++)
                        {
                            _bookingDetail.Travellers.Add(new Traveller() { PaxOrderId = order, PaxType = (int)TravellerPaxType.INL, Title = (int)TravellerTitleType.None, DOBMonth = 0, Gender = (int)GenderType.None, FirstName = "", LastName = "", MiddleName = "" });
                            order++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("PreparePaymentModel:Exception:" + ex.ToString());
            }
        }
        private static void BookingLogsSoldoutPriceChange(Response<FlightBookingRS> response, BookingDetail _bookingDetail)
        {
            try
            {
                if (response.Result.BookingStatus == BookingStatus.SoldOutOrUnavailable || response.Result.BookingStatus == BookingStatus.BookWithHigherPriceChanged)
                {
                    BookingFailureDetails bookingFailureDetails = new BookingFailureDetails()
                    {
                        Guid = _bookingDetail.FlightSearch.SearchGuidId,
                        Name = string.Format("{0} {1}", _bookingDetail.Travellers[0].FirstName, _bookingDetail.Travellers[0].LastName),
                        BookingStatus = ((int)response.Result.BookingStatus).ToString(),
                        OldPrice = string.Format("{0:0.00}", _bookingDetail.Contract.TotalGDSFareV2),
                        NewPrice = string.Format("{0:0.00}", response.Result.Contract.TotalGDSFareV2),
                        Action = null
                    };
                    //Task task = Task.Factory.StartNew(() =>
                    //{
                    //    BookingInformation.AddBookingFailureDatail(bookingFailureDetails);
                    //});
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Debug("BOOKING SOLDOUT-PRICECHANGE|BOOKINGDetails:" + JsonConvert.SerializeObject(_bookingDetail) + "|BOOKINF_RS" + JsonConvert.SerializeObject(response));
                Utility.Logger.Error("AirBusiness.BookingLogsSoldoutPriceChange|Exception:" + ex.ToString());
            }
        }
    }
}
