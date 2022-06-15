using Database;
using Common;
using Infrastructure;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.BlueribbonbagsAPI;
using Infrastructure.HelpingModel.BookingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class BlueBagRibbon
    {
      public static BaggageInsurances PurchaseService(BookingDetail _bookingDetails)
        {
            BaggageInsurances baggageInsurances = null;
            try
            {
                if (_bookingDetails != null)
                {
                    baggageInsurances = new BaggageInsurances()
                    {
                        BookingId = _bookingDetails.Transaction.Id,
                        ProductId = (int)_bookingDetails.BagInsuranc.BagInsuranceType,
                        ProductName = _bookingDetails.BagInsuranc.BagInsuranceType.ToString(),
                        TotalPrice = _bookingDetails.BagInsuranc.TotalPrice,
                    };
                    Airlines airlines = Utility.BaggageAirlines.Where<Airlines>(o =>!string.IsNullOrEmpty(o.AirlineIATACode) && o.AirlineIATACode.Equals(_bookingDetails.Contract.ValidatingCarrier.Code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                    if (airlines != null)
                    {
                        #region Complete RQ
                        PurchaseServiceRQ purchaseService = new PurchaseServiceRQ();
                        purchaseService.ProductCode = _bookingDetails.BagInsuranc.BagInsuranceType.ToString();
                        purchaseService.IsInternational = true;
                        if (Utility.Settings.TravelBagAPI.IsBillingLatter)
                        {
                            if (Utility.Settings.TravelBagAPI.IsLiveEnvironment)
                            {
                              purchaseService.UserLogin = Utility.Settings.TravelBagAPI.ProductionEnvironent.User;
                              purchaseService.UserPassword = Utility.Settings.TravelBagAPI.ProductionEnvironent.Credential;
                            }
                            else
                            {
                                purchaseService.UserLogin = Utility.Settings.TravelBagAPI.TestEnvironent.User;
                                purchaseService.UserPassword = Utility.Settings.TravelBagAPI.TestEnvironent.Credential;
                            }
                         
                        }

                        purchaseService.ReplaceServiceNumberWithCRN = false;
                        purchaseService.FlightDetails = Utility.GetBRBFlightDetails(_bookingDetails.Contract.TripDetails);

                        DateTime dptDate = _bookingDetails.Contract.TripDetails.OutBoundSegment[0].Departure;
                        TimeSpan dptTime = _bookingDetails.Contract.TripDetails.OutBoundSegment[0].DepartureTime;
                        dptDate = new DateTime(dptDate.Year, dptDate.Month, dptDate.Day, dptTime.Hours, dptTime.Minutes, 0);
                        purchaseService.DepartureDt = dptDate.ToString("MM/dd/yyyy hh:mm:ss tt");
                        if (_bookingDetails.Contract.TripType == TripType.ROUNDTRIP)
                        {
                            dptDate = _bookingDetails.Contract.TripDetails.InBoundSegment[_bookingDetails.Contract.TripDetails.InBoundSegment.Count - 1].Arrival;
                            dptTime = _bookingDetails.Contract.TripDetails.InBoundSegment[_bookingDetails.Contract.TripDetails.InBoundSegment.Count - 1].ArrivalTime;
                            dptDate = new DateTime(dptDate.Year, dptDate.Month, dptDate.Day, dptTime.Hours, dptTime.Minutes, 0);
                        }
                        else
                        {
                            dptDate = _bookingDetails.Contract.TripDetails.OutBoundSegment[_bookingDetails.Contract.TripDetails.OutBoundSegment.Count - 1].Arrival;
                            dptTime = _bookingDetails.Contract.TripDetails.OutBoundSegment[_bookingDetails.Contract.TripDetails.OutBoundSegment.Count - 1].ArrivalTime;
                            dptDate = new DateTime(dptDate.Year, dptDate.Month, dptDate.Day, dptTime.Hours, dptTime.Minutes, 0);
                        }

                        purchaseService.LastArrivalDt = dptDate.ToString("MM/dd/yyyy hh:mm: ss tt");


                        if (!Utility.Settings.TravelBagAPI.IsBillingLatter)
                        {
                            purchaseService.CheckoutInfo = new Checkoutinfo()
                            {
                                CreditCardNumber = _bookingDetails.BillingDetails.CardNumber.Replace(" ", ""),
                                CreditCardExpirationDate = string.Format("{0}/{1}", _bookingDetails.BillingDetails.ExpiryMonth <= 9 ? string.Format("0{0}", _bookingDetails.BillingDetails.ExpiryMonth.ToString()) : _bookingDetails.BillingDetails.ExpiryMonth.ToString(), _bookingDetails.BillingDetails.ExpiryYear.ToString().Substring(2, 2)),
                                CreditCardCVV = _bookingDetails.BillingDetails.CVVNumber,
                                FirstName = _bookingDetails.BillingDetails.CCHolderName,
                                LastName = _bookingDetails.BillingDetails.CCHolderName,
                                Address = _bookingDetails.BillingDetails.AddressLine1,
                                City = _bookingDetails.BillingDetails.City,
                                ZipCode = _bookingDetails.BillingDetails.ZipCode,
                                StateCode = _bookingDetails.BillingDetails.State,
                                StateName = _bookingDetails.BillingDetails.State,
                                CountryCode = _bookingDetails.BillingDetails.City,
                                CountryName = _bookingDetails.BillingDetails.City,
                            };
                        }
                        List<Passengerlist> passengerlist = new List<Passengerlist>();
                        #region Travellers
                        int counter = 1;
                        foreach (var item in _bookingDetails.Travellers)
                        {

                            Passengerlist passenger = new Passengerlist()
                            {
                                OrderSequence = counter,
                                LastName = item.LastName,
                                FirstName = item.FirstName,
                                Email = _bookingDetails.BillingDetails.Email,
                                AirlineId = airlines.AirlineId.ToString(),
                                AirlineConfirmationNumber = _bookingDetails.Transaction.PNR //? why not transaction id

                            };
                            passengerlist.Add(passenger);
                            counter++;
                        }
                        #endregion
                        purchaseService.PassengerList = passengerlist;
                        #endregion

                        BlueBagResponse<PurchaseServiceRS> _response = BlubagPurchaseService(purchaseService);

                       Task.Factory.StartNew(()=> Utility.Logger.Info(string.Format("BookingId:{0}|{1}",_bookingDetails.Transaction.Id,Newtonsoft.Json.JsonConvert.SerializeObject(_response))));
                        if (_response != null)
                        {
                            float _totalPrice = Convert.ToSingle(baggageInsurances.TotalPrice);
                            string _serviceNumber = _response.Data != null ? _response.Data.ServiceNumber : "";
                            bool _status = _response.Status;
                            string _statusCode = _response.StatusCode;
                            string _errors = string.Join("|", _response.Errors.Select(s => s.ErrorMessage));

                            baggageInsurances.Status = _status;
                            baggageInsurances.ServiceNumber = _serviceNumber;
                            baggageInsurances.TotalPrice = _totalPrice>0? (decimal)_totalPrice : (decimal)baggageInsurances.TotalPrice;
                            baggageInsurances.StatusCode = _statusCode;
                            baggageInsurances.ErrorMessage = _errors;
                            Task.Factory.StartNew(()=> OptProcedure.SaveBagInsurance(_totalPrice, _serviceNumber, _status, _statusCode, _errors, _bookingDetails.Transaction.Id, (int)_bookingDetails.BagInsuranc.BagInsuranceType, _bookingDetails.BagInsuranc.BagInsuranceType.ToString(), Utility.ConnString));
                        }
                    }
                    else
                    {
                        Utility.Logger.Info("Baggage Insurance Not Call Dut to Validate carrier.|Booking Id:" + _bookingDetails.Transaction.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.BlueBagRibbon.SaveBagInsurance()|EXception:", ex.ToString());
            }
            return baggageInsurances;
        }

        #region private
        private static BlueBagResponse<PurchaseServiceRS> BlubagPurchaseService(PurchaseServiceRQ _purchaseService)
        {
            BlueBagResponse<PurchaseServiceRS> response = null;
            try
            {
                response = RESTClient.BlubagPurchaseService(_purchaseService);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.BlueBagRibbon.BlubagPurchaseService|Exception:" + ex.ToString());
            }
            return response;

        }

        #endregion
    }
}
