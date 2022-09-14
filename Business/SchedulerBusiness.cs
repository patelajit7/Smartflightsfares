using Common;
using Database;
using Infrastructure;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.BookingEntities;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class SchedulerBusiness
    {
        public static async void Start()
        {

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<SaveIncompleteBookingSchedule>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithCronSchedule(Utility.Settings.IncompletBookingScheduleTime)
            .Build();

            await scheduler.ScheduleJob(job, trigger);
            Utility.Logger.Info("Scheduler Started...");
        }
    }
    public class SaveIncompleteBookingSchedule : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            IncompletBookingBusiness.SaveIncompletBookingsInDB();
            Utility.Logger.Info("Scheduler Executed...");

            return Task.CompletedTask;
        }
    }
    public class IncompletBookingBusiness
    {
        private static readonly object objLock = new object();
        public static void SaveIncompletBookingsInDB()
        {
            lock (objLock)
            {
                IncompleteBookingContext _bookingContext = Utility.GetIncompleteBookingCache(Utility.Settings.IncompletBookingKey);
                if (_bookingContext != null && _bookingContext.IncompleteBookings != null && _bookingContext.IncompleteBookings.Count > 0)
                {
                    List<IncompleteBooking> lstSavedBookings = new List<IncompleteBooking>();
                    foreach (var item in _bookingContext.IncompleteBookings)
                    {
                        BookingDetail dbBooking = new BookingDetail()
                        {
                            Contract = item.BookingDetail.Contract,
                            Travellers = item.BookingDetail.Travellers,
                            BillingDetails = item.BookingDetail.BillingDetails,
                            FlightSearch = item.BookingDetail.FlightSearch,
                            BagInsuranc = item.BookingDetail.BagInsuranc,
                            TravelerInsurance = item.BookingDetail.TravelerInsurance,
                            ExtendedCancellation = item.BookingDetail.ExtendedCancellation,
                            Transaction = new Transaction()
                            {
                                BookedOn = DateTime.UtcNow,
                                PNR = "INPROGRESS"
                            }
                        };
                        int trnsId = SaveBookingDetails(dbBooking);
                        if (trnsId > 0)
                        {
                            lstSavedBookings.Add(item);

                            
                                //Task.Factory.StartNew(() =>
                                //{
                                //    BookingDetails bookingDetails = BookingInformation.GetBookingDetails(trnsId);
                                //    if (bookingDetails != null)
                                //    {
                                //        //string htmlMailStringForSelf = ShareUtility.RenderViewToString(this.ControllerContext, "~/views/emails/bookingreceiptdbself.cshtml", bookingDetails);
                                //        //if (!string.IsNullOrEmpty(htmlMailStringForSelf) && htmlMailStringForSelf.Length > 1000 && !string.IsNullOrEmpty(Utility.PortalSettings.SelfBookingMail))
                                //        //{
                                //        //    EmailTransaction transaction = new EmailTransaction()
                                //        //    {
                                //        //        EmailType = EmailType.BookingReceipt,
                                //        //        MailBody = htmlMailStringForSelf,
                                //        //        PortalId = bookingDetails.Transaction.PortalId,
                                //        //        MailRecipient = Utility.PortalSettings.SelfBookingMail,
                                //        //        TransactionId = bookingDetails.Transaction.Id

                                //        //    };
                                //        //    bool isMailSent = EmailHelper.SendMails(transaction);
                                //        //}
                                //    }
                                //});
                            
                            Utility.Logger.Info(string.Format("Incomplete booking saved, BookingId:{0}", trnsId));
                        }
                        else
                        {
                            Utility.Logger.Error(string.Format("Incomplete booking saved failed, RQ:{0}", JsonConvert.SerializeObject(dbBooking)));
                        }
                    }
                    if (lstSavedBookings != null && lstSavedBookings.Count > 0)
                    {
                        _bookingContext.IncompleteBookings = _bookingContext.IncompleteBookings.Except(lstSavedBookings).ToList();
                        Utility.SetIncompleteBookingCache(Utility.Settings.IncompletBookingKey, _bookingContext);
                    }
                }
            }
        }
        public static int SaveBookingDetails(BookingDetail bookingDetails)
        {
            int tid = 0;
            try
            {
                Dictionary<string, DataTable> tables = PrepareBookingDetailsTables(bookingDetails);
                if (tables != null && tables.Count == 9)
                {
                    tid = BookingProcedures.SaveBookingDetails(tables, Utility.ConnString);
                    if (tid == 0)
                    {
                        tid = BookingProcedures.SaveBookingDetails(tables, Utility.ConnString);
                    }
                    if (tid == 0)
                    {
                        Utility.Logger.Info(string.Format("BOOKING UNABLE  SAVE IN DATABASE|PNR:{0}|BOOKING INFO:{1}", bookingDetails.Transaction.PNR, JsonConvert.SerializeObject(bookingDetails)));
                    }
                    if (tid > 0)
                    {
                        //Save User Location
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                string location = AirBusiness.GetUserLocationByIP(bookingDetails.FlightSearch.IP);
                                if (!string.IsNullOrEmpty(location))
                                {
                                    Operation.UpdateBookingUserLocation(tid, location);
                                }
                            }
                            catch
                            { }
                        });
                    }
                }
                else
                {
                    Utility.Logger.Info(string.Format("BOOKING UNABLE  SAVE IN DATABASE|PNR:{0}|BOOKING INFO:{1}", bookingDetails.Transaction.PNR, JsonConvert.SerializeObject(bookingDetails)));
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Info(string.Format("BOOKING UNABLE  SAVE IN DATABASE|PNR:{0}|BOOKING INFO:{1}", bookingDetails.Transaction.PNR, JsonConvert.SerializeObject(bookingDetails)));
                Utility.Logger.Error("BOOKING UNABLE  SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
            return tid;
        }
        public static Dictionary<string, DataTable> PrepareBookingDetailsTables(BookingDetail bookingDetails)
        {
            Dictionary<string, DataTable> bookingTables = null;
            try
            {
                bookingTables = new Dictionary<string, DataTable>();
                bookingTables.Add("Bookings", BookingsDataTbl(bookingDetails));
                bookingTables.Add("Flights", FlightsDataTbl(bookingDetails));
                bookingTables.Add("Segments", FlightSegmentsDataTbl(bookingDetails));
                bookingTables.Add("Travellers", TravellersDataTbl(bookingDetails));
                bookingTables.Add("PriceDetails", FlightPriceDetailsDataTbl(bookingDetails));
                bookingTables.Add("BillingDetails", BillingDetailsDataTbl(bookingDetails));
                bookingTables.Add("BookingExtras", BookingExtrasDataTbl(bookingDetails));
                bookingTables.Add("BookingCoupons", BookingCouponsTbl(bookingDetails));
                bookingTables.Add("MetaFixedPriceDetails", MetaFixedFlightPriceDetailsDataTbl(bookingDetails));
            }
            catch (Exception ex)
            {
                string strbookingDetails = Newtonsoft.Json.JsonConvert.SerializeObject(bookingDetails);
                Utility.Logger.Info(string.Format("BOOKING DETAILs:{0}", strbookingDetails));
                Utility.Logger.Error("EasyPro.Business.SetBookingDetails", ex.ToString());
            }
            return bookingTables;
        }
        private static DataTable BookingCouponsTbl(BookingDetail bookingDetails)
        {
            DataTable bookingCoupon = new DataTable();
            try
            {
                bookingCoupon.Columns.Add("CouponCode", typeof(string));
                bookingCoupon.Columns.Add("CouponMessage", typeof(string));
                bookingCoupon.Columns.Add("Status", typeof(bool));
                bookingCoupon.Columns.Add("TotalAmount", typeof(decimal));
                PopulateBookingCoupons(bookingDetails, ref bookingCoupon);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.BookingCouponsTbl|EXception:", ex.ToString());
            }
            return bookingCoupon;
        }
        private static void PopulateBookingCoupons(BookingDetail bookingDetails, ref DataTable bookingCoupon)
        {
            try
            {
                DataRow dataRow;
                if (bookingDetails.CouponDetails != null)
                {
                    dataRow = bookingCoupon.NewRow();

                    dataRow["CouponCode"] = bookingDetails.CouponDetails.CouponCode;
                    dataRow["CouponMessage"] = bookingDetails.CouponDetails.CouponMessage;
                    dataRow["Status"] = bookingDetails.CouponDetails.Status;
                    dataRow["TotalAmount"] = bookingDetails.CouponDetails.TotalAmount;

                    bookingCoupon.Rows.Add(dataRow);
                    bookingCoupon.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateTransaction|EXception:", ex.ToString());
            }
        }

        private static DataTable BookingExtrasDataTbl(BookingDetail bookingDetails)
        {
            DataTable bookingExtras = new DataTable();
            try
            {
                bookingExtras.Columns.Add("TktTimeLimit", typeof(DateTime));
                bookingExtras.Columns.Add("IsLowcost", typeof(bool));
                PopulateBookingExtras(bookingDetails, ref bookingExtras);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.BookingsDataTbl|EXception:", ex.ToString());
            }
            return bookingExtras;
        }


        #region Private Data table Structure

        private static DataTable BookingsDataTbl(BookingDetail bookingDetails)
        {
            DataTable booking = new DataTable();
            try
            {
                booking.Columns.Add("Guid", typeof(string));
                booking.Columns.Add("PNR", typeof(string));
                booking.Columns.Add("ReferenceNumber", typeof(string));
                booking.Columns.Add("PortalId", typeof(int)).DefaultValue = bookingDetails.FlightSearch.PortalId;
                booking.Columns.Add("GDS", typeof(int));
                booking.Columns.Add("ProviderId", typeof(int));
                booking.Columns.Add("BookingType", typeof(int)).DefaultValue = (int)BookingType.Flight;
                booking.Columns.Add("BookingSourceType", typeof(int)).DefaultValue = (int)BookingSourceType.OnlineBooking;
                booking.Columns.Add("BookingStatus", typeof(int));
                booking.Columns.Add("BookingSubStatus", typeof(int)).DefaultValue = (int)BookingSubStatus.TicketAndMCOIssued;
                booking.Columns.Add("AgentId", typeof(int));
                booking.Columns.Add("AgentLead", typeof(int));
                booking.Columns.Add("UserId", typeof(int));
                booking.Columns.Add("ClientIP", typeof(string));
                booking.Columns.Add("Currency", typeof(int)).DefaultValue = (int)CurrencyType.USD;
                booking.Columns.Add("CurrencyConversion", typeof(float)).DefaultValue = 1;
                booking.Columns.Add("CurrencyCode", typeof(string)).DefaultValue = "USD";
                booking.Columns.Add("ImportTransactionType", typeof(int)).DefaultValue = 0;
                booking.Columns.Add("IsFailedBooking", typeof(bool)).DefaultValue = false;
                booking.Columns.Add("BookingFailedErrorMessage", typeof(string)).DefaultValue = string.Empty;
                PopulateBooking(bookingDetails, ref booking);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.BookingsDataTbl|EXception:", ex.ToString());
            }

            return booking;
        }

        /// <summary>
        /// This method is used to Create data table for Flights table
        /// </summary>
        /// <param name="bookingDetails">BookingDetails</param>
        /// <returns>DataTable</returns>
        private static DataTable FlightsDataTbl(BookingDetail bookingDetails)
        {
            DataTable flights = new DataTable();
            try
            {
                flights.Columns.Add("OriginCode", typeof(string));
                flights.Columns.Add("DestinationCode", typeof(string));
                flights.Columns.Add("ValAirlineCode", typeof(string));
                flights.Columns.Add("TripType", typeof(int)).DefaultValue = (int)TripType.ONEWAY;
                flights.Columns.Add("DeptDate", typeof(DateTime));
                flights.Columns.Add("ReturnDate", typeof(DateTime));
                flights.Columns.Add("PaxCount", typeof(int));
                flights.Columns.Add("AdultCount", typeof(int));
                flights.Columns.Add("SeniorCount", typeof(int));
                flights.Columns.Add("ChildCount", typeof(int));
                flights.Columns.Add("InfantOnSeatCount", typeof(int));
                flights.Columns.Add("InfantLapCount", typeof(int));
                flights.Columns.Add("OutBoundFlightDuration", typeof(Int64));
                flights.Columns.Add("InBoundFlightDuration", typeof(Int64));
                flights.Columns.Add("IsDomestic", typeof(bool));
                flights.Columns.Add("FareType", typeof(string));
                flights.Columns.Add("ContractType", typeof(int)).DefaultValue = (int)ContractType.Actual;
                PopulateFlights(bookingDetails, ref flights);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.FlightsDataTbl|EXception:", ex.ToString());
            }
            return flights;
        }

        /// <summary>
        /// This method is used to Create data table for FlightSegments table
        /// </summary>
        /// <param name="bookingDetails">BookingDetails</param>
        /// <returns>DataTable</returns>
        private static DataTable FlightSegmentsDataTbl(BookingDetail bookingDetails)
        {
            DataTable flightSegments = new DataTable();
            try
            {
                flightSegments.Columns.Add("SegmentOrder", typeof(int));
                flightSegments.Columns.Add("IsReturn", typeof(bool));
                flightSegments.Columns.Add("FlightNumber", typeof(string));
                flightSegments.Columns.Add("OptAirlineCode", typeof(string));
                flightSegments.Columns.Add("MktAirlineCode", typeof(string));
                flightSegments.Columns.Add("OriginCode", typeof(string));
                flightSegments.Columns.Add("DeptDateTime", typeof(DateTime));
                flightSegments.Columns.Add("DeptTerminal", typeof(string));
                flightSegments.Columns.Add("DestinationCode", typeof(string));
                flightSegments.Columns.Add("ArrivalDateTime", typeof(DateTime));
                flightSegments.Columns.Add("ArrivalTerminal", typeof(string));
                flightSegments.Columns.Add("EquipmentDetail", typeof(string));
                flightSegments.Columns.Add("SegmentClass", typeof(string));
                flightSegments.Columns.Add("Stops", typeof(int));
                flightSegments.Columns.Add("Cabin", typeof(int));
                flightSegments.Columns.Add("CompanyFranchiseDetails", typeof(string));
                flightSegments.Columns.Add("TechnicalStoppages", typeof(string));
                flightSegments.Columns.Add("AirlineLocator", typeof(string));
                flightSegments.Columns.Add("SegmentType", typeof(string));
                PopulateFlightSegments(bookingDetails, ref flightSegments);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.FlightsDataTbl|EXception:", ex.ToString());
            }
            return flightSegments;
        }
        private static DataTable MetaFixedFlightPriceDetailsDataTbl(BookingDetail bookingDetails)
        {
            DataTable flightPriceDetails = new DataTable();
            try
            {
                flightPriceDetails.Columns.Add("FareBaseCode", typeof(string));
                flightPriceDetails.Columns.Add("PaxType", typeof(int));
                flightPriceDetails.Columns.Add("Currency", typeof(int));
                flightPriceDetails.Columns.Add("PaxCount", typeof(int));
                flightPriceDetails.Columns.Add("BaseFare", typeof(decimal));
                flightPriceDetails.Columns.Add("Tax", typeof(decimal));
                flightPriceDetails.Columns.Add("Markup", typeof(decimal));
                flightPriceDetails.Columns.Add("SupplierFee", typeof(decimal));
                flightPriceDetails.Columns.Add("Discount", typeof(decimal));
                flightPriceDetails.Columns.Add("IsSellInsurance", typeof(bool));
                flightPriceDetails.Columns.Add("InsuranceAmount", typeof(decimal));
                flightPriceDetails.Columns.Add("TotalAmount", typeof(decimal));
                flightPriceDetails.Columns.Add("IsSellBaggageInsurance", typeof(bool));
                flightPriceDetails.Columns.Add("BaggageInsuranceAmount", typeof(decimal));
                flightPriceDetails.Columns.Add("IsExtendedCancellation", typeof(bool));
                flightPriceDetails.Columns.Add("ExtendedCancellationAmount", typeof(decimal));
                flightPriceDetails.Columns.Add("BookingFee", typeof(decimal));
                MetaFixedPopulateFlightPriceDetails(bookingDetails, ref flightPriceDetails);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.FlightPriceDetails|EXception:", ex.ToString());
            }
            return flightPriceDetails;
        }


        /// <summary>
        /// This method is used to Create data table for Travellers table
        /// </summary>
        /// <param name="bookingDetails">BookingDetails</param>
        /// <returns>DataTable</returns>
        private static DataTable TravellersDataTbl(BookingDetail bookingDetails)
        {
            DataTable travellers = new DataTable();
            try
            {
                travellers.Columns.Add("PaxOrder", typeof(int));
                travellers.Columns.Add("PaxType", typeof(int));
                travellers.Columns.Add("Title", typeof(int));
                travellers.Columns.Add("FirstName", typeof(string));
                travellers.Columns.Add("MiddleName", typeof(string));
                travellers.Columns.Add("LastName", typeof(string));
                travellers.Columns.Add("Gender", typeof(string));
                travellers.Columns.Add("DOB", typeof(DateTime));
                travellers.Columns.Add("AirlineConfirmationNo", typeof(string));
                travellers.Columns.Add("TicketNo", typeof(string));
                travellers.Columns.Add("FrequentFlyerNumber", typeof(string));
                travellers.Columns.Add("PassportNumber", typeof(string));
                travellers.Columns.Add("PassportExpireDate", typeof(DateTime));
                travellers.Columns.Add("PassportIssuedBy", typeof(string));
                travellers.Columns.Add("Email", typeof(string));
                travellers.Columns.Add("MealPreference", typeof(string));
                travellers.Columns.Add("SpecialPreference", typeof(string));
                PopulateTravellers(bookingDetails, ref travellers);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.FlightsDataTbl|EXception:", ex.ToString());
            }
            return travellers;
        }



        /// <summary>
        /// This method is used to Create data table for FlightPriceDetails table
        /// </summary>
        /// <param name="bookingDetails">BookingDetails</param>
        /// <returns>DataTable</returns>
        private static DataTable FlightPriceDetailsDataTbl(BookingDetail bookingDetails)
        {
            DataTable flightPriceDetails = new DataTable();
            try
            {
                flightPriceDetails.Columns.Add("FareBaseCode", typeof(string));
                flightPriceDetails.Columns.Add("PaxType", typeof(int));
                flightPriceDetails.Columns.Add("Currency", typeof(int));
                flightPriceDetails.Columns.Add("PaxCount", typeof(int));
                flightPriceDetails.Columns.Add("BaseFare", typeof(decimal));
                flightPriceDetails.Columns.Add("Tax", typeof(decimal));
                flightPriceDetails.Columns.Add("Markup", typeof(decimal));
                flightPriceDetails.Columns.Add("SupplierFee", typeof(decimal));
                flightPriceDetails.Columns.Add("Discount", typeof(decimal));
                flightPriceDetails.Columns.Add("IsSellInsurance", typeof(bool));
                flightPriceDetails.Columns.Add("InsuranceAmount", typeof(decimal));
                flightPriceDetails.Columns.Add("TotalAmount", typeof(decimal));
                flightPriceDetails.Columns.Add("IsSellBaggageInsurance", typeof(bool));
                flightPriceDetails.Columns.Add("BaggageInsuranceAmount", typeof(decimal));
                flightPriceDetails.Columns.Add("IsExtendedCancellation", typeof(bool));
                flightPriceDetails.Columns.Add("ExtendedCancellationAmount", typeof(decimal));
                flightPriceDetails.Columns.Add("BookingFee", typeof(decimal));
                PopulateFlightPriceDetails(bookingDetails, ref flightPriceDetails);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.FlightPriceDetails|EXception:", ex.ToString());
            }
            return flightPriceDetails;
        }
        private static void MetaFixedPopulateFlightPriceDetails(BookingDetail bookingDetails, ref DataTable flightPriceDetails)
        {
            try
            {
                if (bookingDetails.Contract.MetaFixedPrice != null && bookingDetails.Contract.MetaFixedPrice.IsFixedPrice)
                {
                    List<FareDetails> lstFareDetail = new List<FareDetails>();
                    int paxCount = 0;
                    if (bookingDetails.Contract.MetaFixedPrice.AdultFare != null && bookingDetails.Contract.Adult > 0)
                    {
                        lstFareDetail.Add(bookingDetails.Contract.MetaFixedPrice.AdultFare);
                        paxCount = paxCount + bookingDetails.Contract.Adult;
                    }
                    if (bookingDetails.Contract.MetaFixedPrice.SeniorFare != null && bookingDetails.Contract.Senior > 0)
                    {
                        lstFareDetail.Add(bookingDetails.Contract.MetaFixedPrice.SeniorFare);
                        paxCount = paxCount + bookingDetails.Contract.Senior;
                    }
                    if (bookingDetails.Contract.MetaFixedPrice.ChildFare != null && bookingDetails.Contract.Child > 0)
                    {
                        lstFareDetail.Add(bookingDetails.Contract.MetaFixedPrice.ChildFare);
                        paxCount = paxCount + bookingDetails.Contract.Child;
                    }
                    if (bookingDetails.Contract.MetaFixedPrice.InfantOnSeatFare != null && bookingDetails.Contract.InfantOnSeat > 0)
                    {
                        lstFareDetail.Add(bookingDetails.Contract.MetaFixedPrice.InfantOnSeatFare);
                        paxCount = paxCount + bookingDetails.Contract.InfantOnSeat;
                    }
                    if (bookingDetails.Contract.MetaFixedPrice.InfantOnLapFare != null && bookingDetails.Contract.InfantOnLap > 0)
                    {
                        lstFareDetail.Add(bookingDetails.Contract.MetaFixedPrice.InfantOnLapFare);
                        paxCount = paxCount + bookingDetails.Contract.InfantOnLap;
                    }

                    if (lstFareDetail != null && lstFareDetail.Count > 0)
                    {
                        bool isBaggageInsurance = false;
                        decimal baggageAmount = 0.0M;
                        if (bookingDetails.BagInsuranc != null && bookingDetails.BagInsuranc.BagInsuranceType != BagInsuranceType.NONE)
                        {
                            isBaggageInsurance = true;
                            baggageAmount = bookingDetails.BagInsuranc.PPaxPrice;
                        }
                        bool isTravelInsurance = false;
                        decimal travelAmount = 0.0M;
                        if (bookingDetails.TravelerInsurance != null && bookingDetails.TravelerInsurance.IsTravelProtected)
                        {
                            isTravelInsurance = true;
                            travelAmount = bookingDetails.TravelerInsurance.PPaxPrice;
                        }

                        bool isExtendedCancellation = false;
                        decimal ExtendedCancellationAmount = 0.0M;
                        if (bookingDetails.ExtendedCancellation != null && bookingDetails.ExtendedCancellation.IsExtendedCancellation)
                        {
                            isExtendedCancellation = true;
                            ExtendedCancellationAmount = bookingDetails.ExtendedCancellation.PPaxPrice;
                        }


                        double discountAmountPP = 0.0;
                        if (bookingDetails.CouponDetails != null && bookingDetails.CouponDetails.Status)
                        {
                            discountAmountPP = Math.Round(Convert.ToDouble(bookingDetails.CouponDetails.TotalAmount) / paxCount, 2);
                        }

                        DataRow dataRow;
                        foreach (FareDetails item in lstFareDetail)
                        {
                            dataRow = flightPriceDetails.NewRow();
                            dataRow["FareBaseCode"] = item.FareBaseCode;
                            dataRow["PaxType"] = (int)item.PaxType;
                            dataRow["Currency"] = (int)item.CurrencyType;
                            dataRow["PaxCount"] = item.PaxCount;
                            dataRow["BaseFare"] = item.BaseFare;
                            dataRow["Tax"] = item.Tax;
                            dataRow["Markup"] = item.Markup;
                            dataRow["SupplierFee"] = item.SupplierFee;
                            dataRow["Discount"] = discountAmountPP;
                            item.InsuranceAmount = Convert.ToSingle(travelAmount);
                            item.IsSellInsurance = isTravelInsurance;
                            dataRow["IsSellInsurance"] = isTravelInsurance;
                            dataRow["InsuranceAmount"] = travelAmount;
                            dataRow["TotalAmount"] = item.TotalFareV2;
                            item.BaggageInsuranceAmount = Convert.ToSingle(baggageAmount);
                            item.IsSellBaggageInsurance = isBaggageInsurance;
                            dataRow["IsSellBaggageInsurance"] = isBaggageInsurance;
                            dataRow["BaggageInsuranceAmount"] = baggageAmount;
                            item.IsExtendedCancellation = isExtendedCancellation;
                            dataRow["IsExtendedCancellation"] = isExtendedCancellation;
                            dataRow["ExtendedCancellationAmount"] = ExtendedCancellationAmount;
                            dataRow["BookingFee"] = item.BookingFee;
                            flightPriceDetails.Rows.Add(dataRow);
                            flightPriceDetails.AcceptChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateFlightPriceDetails|EXception:", ex.ToString());
            }
        }

        /// <summary>
        /// This method is used to Create data table for BillingDetails table
        /// </summary>
        /// <param name="bookingDetails">BookingDetails</param>
        /// <returns>DataTable</returns>
        private static DataTable BillingDetailsDataTbl(BookingDetail bookingDetails)
        {
            DataTable billingDetails = new DataTable();
            try
            {
                billingDetails.Columns.Add("CCHolderName", typeof(string));
                billingDetails.Columns.Add("CardNumber", typeof(string));
                billingDetails.Columns.Add("CVVNumber", typeof(string));
                billingDetails.Columns.Add("ExpiryYear", typeof(int));
                billingDetails.Columns.Add("ExpiryMonth", typeof(int));
                billingDetails.Columns.Add("CardType", typeof(int));
                billingDetails.Columns.Add("Email", typeof(string));
                billingDetails.Columns.Add("Country", typeof(string));
                billingDetails.Columns.Add("State", typeof(string));
                billingDetails.Columns.Add("ZipCode", typeof(string));
                billingDetails.Columns.Add("AddressLine1", typeof(string));
                billingDetails.Columns.Add("AddressLine2", typeof(string));
                billingDetails.Columns.Add("AddressLine3", typeof(string));
                billingDetails.Columns.Add("City", typeof(string));
                billingDetails.Columns.Add("BillingPhone", typeof(string));
                billingDetails.Columns.Add("ContactPhone", typeof(string));
                billingDetails.Columns.Add("IsPrimaryCard", typeof(string));
                billingDetails.Columns.Add("AreaCode", typeof(string));
                billingDetails.Columns.Add("CountryCode", typeof(string));
                PopulateBillingDetails(bookingDetails, ref billingDetails);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.BillingDetails|EXception:", ex.ToString());
            }
            return billingDetails;
        }




        #endregion



        #region Private to populate Data Table
        /// <summary>
        /// This method is used to set booking values for MFSPaymentDetails table
        /// </summary>
        /// <param name="context">BookingDetails</param>
        /// <param name="booking">DataTable</param>
        private static void PopulateBooking(BookingDetail bookingDetails, ref DataTable booking)
        {
            try
            {
                DataRow dataRow;
                dataRow = booking.NewRow();
                dataRow["Guid"] = Guid.NewGuid().ToString("N");
                dataRow["PNR"] = bookingDetails.Transaction.PNR;
                dataRow["ReferenceNumber"] = bookingDetails.Transaction.ReferenceNumber;
                dataRow["PortalId"] = Utility.PortalId;
                dataRow["GDS"] = bookingDetails.Transaction.GDS;
                dataRow["ProviderId"] = bookingDetails.Transaction.ProviderId;
                dataRow["BookingType"] = bookingDetails.Transaction.BookingType;
                dataRow["BookingSourceType"] = bookingDetails.Transaction.BookingSourceType;
                dataRow["BookingStatus"] = bookingDetails.Transaction.BookingStatus;
                dataRow["BookingSubStatus"] = bookingDetails.Transaction.BookingSubStatus;
                dataRow["AgentId"] = bookingDetails.Transaction.AgentId;
                dataRow["AgentLead"] = bookingDetails.Transaction.AgentLead;
                dataRow["UserId"] = bookingDetails.Transaction.UserId;
                dataRow["ClientIP"] = bookingDetails.FlightSearch.IP;
                dataRow["Currency"] = bookingDetails.Currency != CurrencyType.None ? (int)bookingDetails.Currency : (int)CurrencyType.USD;
                dataRow["CurrencyConversion"] = bookingDetails.CurrencyConversion > 0 ? Math.Round(bookingDetails.CurrencyConversion,6) : 1;
                dataRow["CurrencyCode"] = !string.IsNullOrEmpty(bookingDetails.CurrencyCode) ? bookingDetails.CurrencyCode : "USD";
                dataRow["ImportTransactionType"] = 0;
                dataRow["IsFailedBooking"] = false;
                dataRow["BookingFailedErrorMessage"] = null;
                booking.Rows.Add(dataRow);
                booking.AcceptChanges();
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateTransaction|EXception:", ex.ToString());
            }
        }
        private static void PopulateFlights(BookingDetail bookingDetails, ref DataTable flights)
        {
            try
            {
                DataRow dataRow;
                dataRow = flights.NewRow();
                dataRow["OriginCode"] = bookingDetails.Contract.Origin;
                dataRow["DestinationCode"] = bookingDetails.Contract.Destination;
                dataRow["ValAirlineCode"] = bookingDetails.Contract.ValidatingCarrier.Code;
                dataRow["TripType"] = (int)bookingDetails.Contract.TripType;
                dataRow["DeptDate"] = bookingDetails.Contract.DepartureDate;
                dataRow["ReturnDate"] = (bookingDetails.Contract.TripType == TripType.ONEWAY ? DateTime.Now.Date : bookingDetails.Contract.ArrivalDate);
                dataRow["PaxCount"] = bookingDetails.Contract.GetTotalPax();
                dataRow["AdultCount"] = bookingDetails.Contract.Adult;
                dataRow["SeniorCount"] = bookingDetails.Contract.Senior;
                dataRow["ChildCount"] = bookingDetails.Contract.Child;
                dataRow["InfantOnSeatCount"] = bookingDetails.Contract.InfantOnSeat;
                dataRow["InfantLapCount"] = bookingDetails.Contract.InfantOnLap;
                dataRow["OutBoundFlightDuration"] = bookingDetails.Contract.TotalOutBoundFlightDuration != TimeSpan.MinValue ? (Int64)bookingDetails.Contract.TotalOutBoundFlightDuration.TotalMinutes : 0;
                dataRow["InBoundFlightDuration"] = bookingDetails.Contract.TotalInBoundFlightDuration != TimeSpan.MinValue ? (Int64)bookingDetails.Contract.TotalInBoundFlightDuration.TotalMinutes : 0; dataRow["IsDomestic"] = false;
                dataRow["FareType"] = bookingDetails.Contract.FareType;
                dataRow["ContractType"] = (int)bookingDetails.Contract.ContractType;
                flights.Rows.Add(dataRow);
                flights.AcceptChanges();

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateFlights|EXception:", ex.ToString());
            }
        }
        private static void PopulateFlightSegments(BookingDetail bookingDetails, ref DataTable flightSegments)
        {

            try
            {
                DataRow dataRow;
                int i = 0;
                foreach (Segments item in bookingDetails.Contract.TripDetails.OutBoundSegment)
                {
                    dataRow = flightSegments.NewRow();
                    dataRow["SegmentOrder"] = i;
                    dataRow["IsReturn"] = false;
                    dataRow["FlightNumber"] = item.FlightNumber;
                    dataRow["OptAirlineCode"] = item.OperatingCarrier != null ? item.OperatingCarrier.Code : item.MarketingCarrier.Code;
                    dataRow["MktAirlineCode"] = item.MarketingCarrier.Code;
                    dataRow["OriginCode"] = item.Origin;
                    dataRow["DeptDateTime"] = new DateTime(item.Departure.Year, item.Departure.Month, item.Departure.Day).Add(item.DepartureTime);
                    dataRow["DeptTerminal"] = item.OutTerminal;
                    dataRow["DestinationCode"] = item.Destination;
                    dataRow["ArrivalDateTime"] = new DateTime(item.Arrival.Year, item.Arrival.Month, item.Arrival.Day).Add(item.ArrivalTime);
                    dataRow["ArrivalTerminal"] = item.InTerminal;
                    dataRow["EquipmentDetail"] = item.EquipmentType;
                    dataRow["SegmentClass"] = item.Class;
                    dataRow["Stops"] = item.NoOfStops;
                    dataRow["Cabin"] = (int)item.CabinType;
                    dataRow["CompanyFranchiseDetails"] = item.CompanyFranchiseDetails;
                    dataRow["TechnicalStoppages"] = "";
                    dataRow["AirlineLocator"] = item.AirlineLocator;
                    dataRow["SegmentType"] = string.IsNullOrEmpty(item.SegmentStatus) == true ? "" : item.SegmentStatus;
                    flightSegments.Rows.Add(dataRow);
                    flightSegments.AcceptChanges();
                    i++;
                }
                if (bookingDetails.Contract.TripType == TripType.ROUNDTRIP && bookingDetails.Contract.TripDetails.InBoundSegment != null && bookingDetails.Contract.TripDetails.InBoundSegment.Count > 0)
                {
                    foreach (Segments item in bookingDetails.Contract.TripDetails.InBoundSegment)
                    {
                        dataRow = flightSegments.NewRow();
                        dataRow["SegmentOrder"] = i;
                        dataRow["IsReturn"] = true;
                        dataRow["FlightNumber"] = item.FlightNumber;
                        dataRow["OptAirlineCode"] = item.OperatingCarrier != null ? item.OperatingCarrier.Code : item.MarketingCarrier.Code;
                        dataRow["MktAirlineCode"] = item.MarketingCarrier.Code;
                        dataRow["OriginCode"] = item.Origin;
                        dataRow["DeptDateTime"] = new DateTime(item.Departure.Year, item.Departure.Month, item.Departure.Day).Add(item.DepartureTime);
                        dataRow["DeptTerminal"] = item.OutTerminal;
                        dataRow["DestinationCode"] = item.Destination;
                        dataRow["ArrivalDateTime"] = new DateTime(item.Arrival.Year, item.Arrival.Month, item.Arrival.Day).Add(item.ArrivalTime);
                        dataRow["ArrivalTerminal"] = item.InTerminal;
                        dataRow["EquipmentDetail"] = item.EquipmentType;
                        dataRow["SegmentClass"] = item.Class;
                        dataRow["Stops"] = item.NoOfStops;
                        dataRow["Cabin"] = (int)item.CabinType;
                        dataRow["CompanyFranchiseDetails"] = item.CompanyFranchiseDetails;
                        dataRow["TechnicalStoppages"] = "";
                        dataRow["AirlineLocator"] = item.AirlineLocator;
                        dataRow["SegmentType"] = "";
                        flightSegments.Rows.Add(dataRow);
                        flightSegments.AcceptChanges();
                        i++;
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateFlightSegments|EXception:", ex.ToString());
            }
        }
        private static void PopulateTravellers(BookingDetail bookingDetails, ref DataTable travellers)
        {
            try
            {
                if (bookingDetails.Travellers != null && bookingDetails.Travellers.Count > 0)
                {
                    DataRow dataRow;
                    int i = 0;
                    foreach (Traveller item in bookingDetails.Travellers)
                    {
                        dataRow = travellers.NewRow();
                        dataRow["PaxOrder"] = i;
                        dataRow["PaxType"] = (int)item.PaxType;
                        dataRow["Title"] = item.Title;
                        dataRow["FirstName"] = item.FirstName;
                        dataRow["MiddleName"] = item.MiddleName;
                        dataRow["LastName"] = item.LastName;
                        dataRow["Gender"] = item.Gender;
                        if(item.DOBYear==null || item.DOBDay == null)
                        {
                            dataRow["DOB"] = DBNull.Value;
                        }
                        else
                        {
                            dataRow["DOB"] = new DateTime(item.DOBYear ?? 0, item.DOBMonth, item.DOBDay ?? 0);
                        }
                        
                        dataRow["AirlineConfirmationNo"] = null;
                        dataRow["TicketNo"] = null;
                        dataRow["FrequentFlyerNumber"] = null;
                        dataRow["PassportNumber"] = item.PassportNumber;
                        if (item.PassportExpiryDate == null || (item.PassportExpiryDate != null && item.PassportExpiryDate == DateTime.MinValue))
                        {
                            dataRow["PassportExpireDate"] = DBNull.Value;
                        }
                        else
                        {
                            dataRow["PassportExpireDate"] = item.PassportExpiryDate;
                        }
                        dataRow["PassportIssuedBy"] = item.PassportIssuingCountry;
                        dataRow["Email"] = null;
                        dataRow["MealPreference"] = null;
                        dataRow["SpecialPreference"] = null;
                        travellers.Rows.Add(dataRow);
                        travellers.AcceptChanges();
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateTravellers|EXception:", ex.ToString());
            }
        }
        private static void PopulateBillingDetails(BookingDetail bookingDetails, ref DataTable tblBillingDetails)
        {
            try
            {
                if (bookingDetails.BillingDetails != null)
                {
                    DataRow dataRow;
                    BillingDetail item = bookingDetails.BillingDetails;
                    dataRow = tblBillingDetails.NewRow();
                    dataRow["CCHolderName"] = item.CCHolderName;
                    dataRow["CardNumber"] = !string.IsNullOrEmpty(item.CardNumber) ? item.CardNumber.Trim() : string.Empty;
                    dataRow["CVVNumber"] = item.CVVNumber;
                    dataRow["ExpiryYear"] = item.ExpiryYear;
                    dataRow["ExpiryMonth"] = item.ExpiryMonth;
                    dataRow["CardType"] = item.CardType;
                    dataRow["Email"] = item.Email;
                    dataRow["Country"] = item.Country;
                    dataRow["State"] = (item.Country.Equals("US", StringComparison.OrdinalIgnoreCase) || item.Country.Equals("CA", StringComparison.OrdinalIgnoreCase)) ? item.State : item.StateName;
                    dataRow["ZipCode"] = item.ZipCode;
                    dataRow["AddressLine1"] = item.AddressLine1;
                    dataRow["AddressLine2"] = item.AddressLine2;
                    dataRow["AddressLine3"] = item.AddressLine3;
                    dataRow["City"] = item.City;
                    dataRow["AreaCode"] = item.AreaCode;
                    dataRow["CountryCode"] = item.CountryCode;
                    dataRow["BillingPhone"] = item.BillingPhone;
                    dataRow["ContactPhone"] = item.ContactPhone;
                    dataRow["IsPrimaryCard"] = item.IsPrimaryCard;
                    tblBillingDetails.Rows.Add(dataRow);
                    tblBillingDetails.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateBillingDetails|EXception:", ex.ToString());
            }
        }
        private static void PopulateFlightPriceDetails(BookingDetail bookingDetails, ref DataTable flightPriceDetails)
        {
            try
            {
                List<FareDetails> lstFareDetail = new List<FareDetails>();

                if (bookingDetails.Contract.AdultFare != null && bookingDetails.Contract.Adult > 0)
                {
                    lstFareDetail.Add(bookingDetails.Contract.AdultFare);
                }
                if (bookingDetails.Contract.SeniorFare != null && bookingDetails.Contract.Senior > 0)
                {
                    lstFareDetail.Add(bookingDetails.Contract.SeniorFare);
                }
                if (bookingDetails.Contract.ChildFare != null && bookingDetails.Contract.Child > 0)
                {
                    lstFareDetail.Add(bookingDetails.Contract.ChildFare);
                }
                if (bookingDetails.Contract.InfantOnSeatFare != null && bookingDetails.Contract.InfantOnSeat > 0)
                {
                    lstFareDetail.Add(bookingDetails.Contract.InfantOnSeatFare);
                }
                if (bookingDetails.Contract.InfantOnLapFare != null && bookingDetails.Contract.InfantOnLap > 0)
                {
                    lstFareDetail.Add(bookingDetails.Contract.InfantOnLapFare);
                }

                if (lstFareDetail != null && lstFareDetail.Count > 0)
                {
                    bool isBaggageInsurance = false;
                    decimal baggageAmount = 0.0M;
                    if (bookingDetails.BagInsuranc != null && bookingDetails.BagInsuranc.BagInsuranceType != BagInsuranceType.NONE)
                    {
                        isBaggageInsurance = true;
                        baggageAmount = bookingDetails.BagInsuranc.PPaxPrice;
                    }
                    bool isTravelInsurance = false;
                    decimal travelAmount = 0.0M;
                    if (bookingDetails.TravelerInsurance != null && bookingDetails.TravelerInsurance.IsTravelProtected)
                    {
                        isTravelInsurance = true;
                        travelAmount = bookingDetails.TravelerInsurance.PPaxPrice;
                    }

                    bool isExtendedCancellation = false;
                    decimal ExtendedCancellationAmount = 0.0M;
                    if (bookingDetails.ExtendedCancellation != null && bookingDetails.ExtendedCancellation.IsExtendedCancellation)
                    {
                        isExtendedCancellation = true;
                        ExtendedCancellationAmount = bookingDetails.ExtendedCancellation.PPaxPrice;
                    }

                    DataRow dataRow;
                    foreach (FareDetails item in lstFareDetail)
                    {
                        dataRow = flightPriceDetails.NewRow();
                        dataRow["FareBaseCode"] = item.FareBaseCode;
                        dataRow["PaxType"] = (int)item.PaxType;
                        dataRow["Currency"] = (int)item.CurrencyType;
                        dataRow["PaxCount"] = item.PaxCount;
                        dataRow["BaseFare"] = item.BaseFare;
                        dataRow["Tax"] = item.Tax;
                        dataRow["Markup"] = item.Markup;
                        dataRow["SupplierFee"] = item.SupplierFee;
                        dataRow["Discount"] = item.Discount;
                        item.InsuranceAmount = Convert.ToSingle(travelAmount);
                        item.IsSellInsurance = isTravelInsurance;
                        dataRow["IsSellInsurance"] = isTravelInsurance;
                        dataRow["InsuranceAmount"] = travelAmount;
                        dataRow["TotalAmount"] = item.TotalFareV2;
                        item.BaggageInsuranceAmount = Convert.ToSingle(baggageAmount);
                        item.IsSellBaggageInsurance = isBaggageInsurance;
                        dataRow["IsSellBaggageInsurance"] = isBaggageInsurance;
                        dataRow["BaggageInsuranceAmount"] = baggageAmount;
                        item.IsExtendedCancellation = isExtendedCancellation;
                        dataRow["IsExtendedCancellation"] = isExtendedCancellation;
                        dataRow["ExtendedCancellationAmount"] = ExtendedCancellationAmount;
                        dataRow["BookingFee"] = item.BookingFee;
                        flightPriceDetails.Rows.Add(dataRow);
                        flightPriceDetails.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateFlightPriceDetails|EXception:", ex.ToString());
            }
        }
        private static void PopulateBookingExtras(BookingDetail bookingDetails, ref DataTable bookingExtra)
        {
            try
            {
                DataRow dataRow;
                if (bookingDetails.Contract.Provider == ProviderType.MYSTIFLY)
                {
                    dataRow = bookingExtra.NewRow();
                    if (bookingDetails.Contract.MystiflyExt.TktTimeLimit != null)
                    {
                        dataRow["TktTimeLimit"] = Convert.ToDateTime(bookingDetails.Contract.MystiflyExt.TktTimeLimit);
                    }
                    else
                    {
                        dataRow["TktTimeLimit"] = DBNull.Value;
                    }
                    dataRow["IsLowcost"] = bookingDetails.Contract.ValidatingCarrier.IsLowcost;
                    bookingExtra.Rows.Add(dataRow);
                    bookingExtra.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateTransaction|EXception:", ex.ToString());
            }
        }
        #endregion

        public static void SaveBookingDetailsByJson()
        {
            List<BookingDetail> bookingDetails = JsonConvert.DeserializeObject<List<BookingDetail>>("[{\"Transaction\":{\"Id\":0,\"Guid\":null,\"PNR\":\"INPROGRESS\",\"ReferenceNumber\":null,\"GDS\":0,\"ProviderId\":0,\"PortalId\":0,\"BookingType\":0,\"BookingStatus\":0,\"BookingSubStatus\":0,\"BookingSourceType\":0,\"AgentId\":0,\"AgentLead\":0,\"UserId\":0,\"BookedOn\":\"2021-11-27T00:00:00.0081136Z\"},\"FlightSearch\":{\"SearchGuidId\":\"3c79e9c4d0684f6f83f7ea0e3b4f4206\",\"PortalId\":1000,\"AffiliateId\":1000,\"TripType\":2,\"Origin\":\"YOW\",\"Destination\":\"YYZ\",\"Departure\":\"2021-12-12T00:00:00\",\"Return\":\"2021-12-14T00:00:00\",\"Adult\":1,\"Senior\":0,\"Child\":0,\"InfantOnSeat\":0,\"InfantOnLap\":0,\"Cabin\":4,\"PreferredCarrier\":null,\"IsDirectFlight\":false,\"IP\":\"64.137.152.250\",\"IsMobileDevice\":false,\"UserId\":0,\"UserAgent\":null,\"OriginSearch\":\"YOW - Ottawa, Canada\",\"DestinationSearch\":\"YYZ - Toronto, Canada\",\"OriginAirportName\":\"Mcdonald Cartier Intl Airport\",\"OriginCountry\":null,\"DestAirportName\":\"Lester B Pearson Intl\",\"DestCountry\":null,\"IsMetaSearch\":false,\"Created\":\"0001-01-01T00:00:00\",\"UtmSource\":null,\"UtmMedium\":null,\"UtmCampaign\":null,\"UtmTerm\":null,\"UtmContent\":null,\"UtmKeyword\":null,\"ClickedId\":null,\"PageType\":0,\"PageId\":0,\"SearchDateTime\":\"2021-11-26T16:25:09.4014055Z\",\"FlexiblityQualifier\":null},\"Contract\":{\"SearchGuid\":\"3c79e9c4d0684f6f83f7ea0e3b4f4206\",\"ContractId\":189,\"Provider\":6,\"GDSType\":6,\"Origin\":\"YOW\",\"OriginCityName\":\"Ottawa\",\"Destination\":\"YYZ\",\"DestinationCityName\":\"Toronto\",\"OriginSearch\":\"YOW\",\"DestinationSearch\":\"YYZ\",\"DepartureDate\":\"2021-12-12T10:10:00\",\"ArrivalDate\":\"2021-12-14T14:00:00\",\"TripDetails\":{\"OutBoundSegment\":[{\"Id\":1,\"IsReturn\":false,\"IsDepartDateHighlight\":false,\"IsOriginHighlight\":false,\"IsDestinationHighlight\":false,\"Departure\":\"2021-12-12T00:00:00\",\"Arrival\":\"2021-12-12T00:00:00\",\"MarketingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"OperatingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"DepartureTime\":\"10:10:00\",\"ArrivalTime\":\"11:23:00\",\"StopOverTime\":null,\"OutTerminal\":null,\"InTerminal\":\"1\",\"EquipmentType\":\"\",\"FlightNumber\":\"447\",\"CnxType\":null,\"FareBasis\":\"PT4NZAEL\",\"Class\":\"P\",\"PrevClass\":null,\"Cabin\":\"Business\",\"CabinType\":4,\"Origin\":\"YOW\",\"OriginCity\":\"Ottawa\",\"Destination\":\"YYZ\",\"DestinationCity\":\"Toronto\",\"FlightDuration\":\"01:13:00\",\"CompanyFranchiseDetails\":null,\"AvailableSeats\":1,\"NoOfStops\":0,\"IsSoldOut\":false,\"AirlineLocator\":null,\"SegmentStatus\":null,\"SegmentTripProExt\":null,\"SegmentASSExtension\":{\"SegmentId\":\"6\",\"Number\":\"447\",\"ASSExtensionOperating\":{\"carrierCode\":\"AC\"},\"CarrierCode\":\"AC\",\"AirCraftCode\":\"320\",\"BaggageQuantity\":2}}],\"InBoundSegment\":[{\"Id\":1,\"IsReturn\":false,\"IsDepartDateHighlight\":false,\"IsOriginHighlight\":false,\"IsDestinationHighlight\":false,\"Departure\":\"2021-12-14T00:00:00\",\"Arrival\":\"2021-12-14T00:00:00\",\"MarketingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"OperatingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"DepartureTime\":\"14:00:00\",\"ArrivalTime\":\"15:01:00\",\"StopOverTime\":null,\"OutTerminal\":\"1\",\"InTerminal\":null,\"EquipmentType\":\"\",\"FlightNumber\":\"454\",\"CnxType\":null,\"FareBasis\":\"ZV7NZAEL\",\"Class\":\"Z\",\"PrevClass\":null,\"Cabin\":\"Business\",\"CabinType\":4,\"Origin\":\"YYZ\",\"OriginCity\":\"Toronto\",\"Destination\":\"YOW\",\"DestinationCity\":\"Ottawa\",\"FlightDuration\":\"01:01:00\",\"CompanyFranchiseDetails\":null,\"AvailableSeats\":1,\"NoOfStops\":0,\"IsSoldOut\":false,\"AirlineLocator\":null,\"SegmentStatus\":null,\"SegmentTripProExt\":null,\"SegmentASSExtension\":{\"SegmentId\":\"29\",\"Number\":\"454\",\"ASSExtensionOperating\":{\"carrierCode\":\"AC\"},\"CarrierCode\":\"AC\",\"AirCraftCode\":\"320\",\"BaggageQuantity\":2}}]},\"ValidatingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"FareType\":\"PUBLISHED\",\"TripType\":2,\"IsRefundable\":false,\"Adult\":1,\"Senior\":0,\"Child\":0,\"InfantOnSeat\":0,\"InfantOnLap\":0,\"FareBasisCode\":\"PT4NZAEL\",\"AdultFare\":{\"PaxCount\":1,\"PaxType\":1,\"GDSPaxType\":null,\"ActualBaseFare\":720.0,\"BaseFare\":720.0,\"Tax\":214.169983,\"TotalFare\":934.17,\"Markup\":29.89344,\"SupplierFee\":0.0,\"Discount\":0.0,\"IsSellInsurance\":false,\"InsuranceAmount\":0.0,\"IsSellBaggageInsurance\":false,\"BaggageInsuranceAmount\":0.0,\"FareBaseCode\":\"PT4NZAEL\",\"CurrencyType\":1,\"IsExtendedCancellation\":false,\"ExtendedCancellationAmount\":0.0,\"BookingFee\":0.0,\"TotalFareV2\":964.0634,\"TotalFarePPax\":964.0634,\"BaseFarePPax\":720.0,\"TotalBaseFare\":720.0,\"TaxPPax\":244.063416,\"TotalTax\":244.063416},\"ChildFare\":null,\"InfantOnSeatFare\":null,\"InfantOnLapFare\":null,\"SeniorFare\":null,\"TotalMarkup\":29.89344,\"TotalSupplierFee\":0.0,\"TotalBaseFare\":720.0,\"TotalTax\":214.169983,\"TotalGDSFareV2\":964.0634,\"EnginePriority\":0,\"Contractkey\":\"121010AC447141400AC454\",\"DatesKey\":null,\"PricingSource\":\"PUBLISHED\",\"MaxStopOutbound\":0,\"MaxStopInbound\":0,\"IsMultipleAirlineContract\":false,\"MinSeatAvailableForContract\":1,\"IsPhoneOnly\":false,\"ContractType\":1,\"TotalOutBoundFlightDuration\":\"01:13:00\",\"TotalInBoundFlightDuration\":\"01:01:00\",\"AffiliateId\":1000,\"AmadeusSessionToken\":null,\"BookingStatus\":0,\"TotalBookingFee\":0.0,\"BaggageQuantity\":2,\"TripProExt\":null,\"MystiflyExt\":null,\"AmaduesSelfServiceExtension\":{\"Source\":\"GDS\",\"Fees\":[{\"Amount\":\"0.00\",\"Type\":\"SUPPLIER\"},{\"Amount\":\"0.00\",\"Type\":\"TICKETING\"}],\"TravelerPricing\":[{\"travelerId\":\"1\",\"fareOption\":\"STANDARD\",\"travelerType\":\"ADULT\",\"price\":{\"currency\":\"USD\",\"total\":934.17,\"base\":720.0,\"fees\":null,\"grandTotal\":0.0},\"fareDetailsBySegment\":[{\"segmentId\":\"6\",\"cabin\":\"BUSINESS\",\"fareBasis\":\"PT4NZAEL\",\"class\":\"P\",\"includedCheckedBags\":{\"quantity\":2},\"brandedFare\":\"EXECLOW\"},{\"segmentId\":\"29\",\"cabin\":\"BUSINESS\",\"fareBasis\":\"ZV7NZAEL\",\"class\":\"Z\",\"includedCheckedBags\":{\"quantity\":2},\"brandedFare\":\"EXECLOW\"}]}]}},\"BillingDetails\":{\"CCHolderName\":null,\"CardNumber\":null,\"CVVNumber\":null,\"ExpiryYear\":0,\"ExpiryMonth\":0,\"CardType\":0,\"Email\":\"rhillier@waygarcapital.com\",\"EmailConfirm\":null,\"Country\":\"US\",\"State\":\"AK\",\"StateName\":null,\"ZipCode\":null,\"AddressLine1\":null,\"AddressLine2\":null,\"AddressLine3\":null,\"City\":null,\"BillingPhone\":\"4165720025\",\"ContactPhone\":null,\"IsPrimaryCard\":false,\"AreaCode\":null,\"CountryCode\":\"1\"},\"Travellers\":[{\"PaxOrderId\":1,\"PaxType\":1,\"Title\":0,\"FirstName\":\"Rickey\",\"MiddleName\":null,\"LastName\":\"Hillier\",\"Gender\":0,\"DOBDay\":null,\"DOBMonth\":0,\"DOBYear\":null,\"PassportNumber\":null,\"PassportIssuingCountry\":null,\"PassportExpiryDate\":null}],\"PriceIncrease\":0.0,\"BagInsuranc\":null,\"TravelerInsurance\":null,\"CouponDetails\":null,\"ExtendedCancellation\":null,\"Currency\":0,\"CurrencyConversion\":0.0,\"CurrencyCode\":null},{\"Transaction\":{\"Id\":0,\"Guid\":null,\"PNR\":\"INPROGRESS\",\"ReferenceNumber\":null,\"GDS\":0,\"ProviderId\":0,\"PortalId\":0,\"BookingType\":0,\"BookingStatus\":0,\"BookingSubStatus\":0,\"BookingSourceType\":0,\"AgentId\":0,\"AgentLead\":0,\"UserId\":0,\"BookedOn\":\"2021-11-27T00:00:00.0237005Z\"},\"FlightSearch\":{\"SearchGuidId\":\"410ff206e48b4b90a3b40eb64d181398\",\"PortalId\":1000,\"AffiliateId\":1000,\"TripType\":2,\"Origin\":\"YOW\",\"Destination\":\"YYZ\",\"Departure\":\"2021-12-12T00:00:00\",\"Return\":\"2021-12-14T00:00:00\",\"Adult\":1,\"Senior\":0,\"Child\":0,\"InfantOnSeat\":0,\"InfantOnLap\":0,\"Cabin\":1,\"PreferredCarrier\":null,\"IsDirectFlight\":false,\"IP\":\"64.137.152.250\",\"IsMobileDevice\":false,\"UserId\":0,\"UserAgent\":null,\"OriginSearch\":\"YOW - Ottawa, Canada\",\"DestinationSearch\":\"YYZ - Toronto, Canada\",\"OriginAirportName\":\"Mcdonald Cartier Intl Airport\",\"OriginCountry\":null,\"DestAirportName\":\"Lester B Pearson Intl\",\"DestCountry\":null,\"IsMetaSearch\":false,\"Created\":\"0001-01-01T00:00:00\",\"UtmSource\":null,\"UtmMedium\":null,\"UtmCampaign\":null,\"UtmTerm\":null,\"UtmContent\":null,\"UtmKeyword\":null,\"ClickedId\":null,\"PageType\":0,\"PageId\":0,\"SearchDateTime\":\"2021-11-26T16:35:13.6808907Z\",\"FlexiblityQualifier\":null},\"Contract\":{\"SearchGuid\":\"410ff206e48b4b90a3b40eb64d181398\",\"ContractId\":150,\"Provider\":6,\"GDSType\":6,\"Origin\":\"YOW\",\"OriginCityName\":\"Ottawa\",\"Destination\":\"YYZ\",\"DestinationCityName\":\"Toronto\",\"OriginSearch\":\"YOW\",\"DestinationSearch\":\"YYZ\",\"DepartureDate\":\"2021-12-12T10:10:00\",\"ArrivalDate\":\"2021-12-14T14:00:00\",\"TripDetails\":{\"OutBoundSegment\":[{\"Id\":1,\"IsReturn\":false,\"IsDepartDateHighlight\":false,\"IsOriginHighlight\":false,\"IsDestinationHighlight\":false,\"Departure\":\"2021-12-12T00:00:00\",\"Arrival\":\"2021-12-12T00:00:00\",\"MarketingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"OperatingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"DepartureTime\":\"10:10:00\",\"ArrivalTime\":\"11:23:00\",\"StopOverTime\":null,\"OutTerminal\":null,\"InTerminal\":\"1\",\"EquipmentType\":\"\",\"FlightNumber\":\"447\",\"CnxType\":null,\"FareBasis\":\"LW4LZATG\",\"Class\":\"L\",\"PrevClass\":null,\"Cabin\":\"Economy\",\"CabinType\":1,\"Origin\":\"YOW\",\"OriginCity\":\"Ottawa\",\"Destination\":\"YYZ\",\"DestinationCity\":\"Toronto\",\"FlightDuration\":\"01:13:00\",\"CompanyFranchiseDetails\":null,\"AvailableSeats\":6,\"NoOfStops\":0,\"IsSoldOut\":false,\"AirlineLocator\":null,\"SegmentStatus\":null,\"SegmentTripProExt\":null,\"SegmentASSExtension\":{\"SegmentId\":\"4\",\"Number\":\"447\",\"ASSExtensionOperating\":{\"carrierCode\":\"AC\"},\"CarrierCode\":\"AC\",\"AirCraftCode\":\"320\",\"BaggageQuantity\":0}}],\"InBoundSegment\":[{\"Id\":1,\"IsReturn\":false,\"IsDepartDateHighlight\":false,\"IsOriginHighlight\":false,\"IsDestinationHighlight\":false,\"Departure\":\"2021-12-14T00:00:00\",\"Arrival\":\"2021-12-14T00:00:00\",\"MarketingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"OperatingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"DepartureTime\":\"14:00:00\",\"ArrivalTime\":\"15:01:00\",\"StopOverTime\":null,\"OutTerminal\":\"1\",\"InTerminal\":null,\"EquipmentType\":\"\",\"FlightNumber\":\"454\",\"CnxType\":null,\"FareBasis\":\"LW4LZATG\",\"Class\":\"L\",\"PrevClass\":null,\"Cabin\":\"Economy\",\"CabinType\":1,\"Origin\":\"YYZ\",\"OriginCity\":\"Toronto\",\"Destination\":\"YOW\",\"DestinationCity\":\"Ottawa\",\"FlightDuration\":\"01:01:00\",\"CompanyFranchiseDetails\":null,\"AvailableSeats\":6,\"NoOfStops\":0,\"IsSoldOut\":false,\"AirlineLocator\":null,\"SegmentStatus\":null,\"SegmentTripProExt\":null,\"SegmentASSExtension\":{\"SegmentId\":\"31\",\"Number\":\"454\",\"ASSExtensionOperating\":{\"carrierCode\":\"AC\"},\"CarrierCode\":\"AC\",\"AirCraftCode\":\"320\",\"BaggageQuantity\":0}}]},\"ValidatingCarrier\":{\"Code\":\"AC\",\"Name\":\"Air Canada\",\"IsLowcost\":false},\"FareType\":\"PUBLISHED\",\"TripType\":2,\"IsRefundable\":false,\"Adult\":1,\"Senior\":0,\"Child\":0,\"InfantOnSeat\":0,\"InfantOnLap\":0,\"FareBasisCode\":\"LW4LZATG\",\"AdultFare\":{\"PaxCount\":1,\"PaxType\":1,\"GDSPaxType\":null,\"ActualBaseFare\":180.0,\"BaseFare\":180.0,\"Tax\":103.47,\"TotalFare\":283.47,\"Markup\":9.07104,\"SupplierFee\":0.0,\"Discount\":0.0,\"IsSellInsurance\":false,\"InsuranceAmount\":0.0,\"IsSellBaggageInsurance\":false,\"BaggageInsuranceAmount\":0.0,\"FareBaseCode\":\"LW4LZATG\",\"CurrencyType\":1,\"IsExtendedCancellation\":false,\"ExtendedCancellationAmount\":0.0,\"BookingFee\":0.0,\"TotalFareV2\":292.541046,\"TotalFarePPax\":292.541046,\"BaseFarePPax\":180.0,\"TotalBaseFare\":180.0,\"TaxPPax\":112.541039,\"TotalTax\":112.541039},\"ChildFare\":null,\"InfantOnSeatFare\":null,\"InfantOnLapFare\":null,\"SeniorFare\":null,\"TotalMarkup\":9.07104,\"TotalSupplierFee\":0.0,\"TotalBaseFare\":180.0,\"TotalTax\":103.47,\"TotalGDSFareV2\":292.541046,\"EnginePriority\":0,\"Contractkey\":\"121010AC447141400AC454\",\"DatesKey\":null,\"PricingSource\":\"PUBLISHED\",\"MaxStopOutbound\":0,\"MaxStopInbound\":0,\"IsMultipleAirlineContract\":false,\"MinSeatAvailableForContract\":6,\"IsPhoneOnly\":false,\"ContractType\":1,\"TotalOutBoundFlightDuration\":\"01:13:00\",\"TotalInBoundFlightDuration\":\"01:01:00\",\"AffiliateId\":1000,\"AmadeusSessionToken\":null,\"BookingStatus\":0,\"TotalBookingFee\":0.0,\"BaggageQuantity\":0,\"TripProExt\":null,\"MystiflyExt\":null,\"AmaduesSelfServiceExtension\":{\"Source\":\"GDS\",\"Fees\":[{\"Amount\":\"0.00\",\"Type\":\"SUPPLIER\"},{\"Amount\":\"0.00\",\"Type\":\"TICKETING\"}],\"TravelerPricing\":[{\"travelerId\":\"1\",\"fareOption\":\"STANDARD\",\"travelerType\":\"ADULT\",\"price\":{\"currency\":\"USD\",\"total\":283.47,\"base\":180.0,\"fees\":null,\"grandTotal\":0.0},\"fareDetailsBySegment\":[{\"segmentId\":\"4\",\"cabin\":\"ECONOMY\",\"fareBasis\":\"LW4LZATG\",\"class\":\"L\",\"includedCheckedBags\":{\"quantity\":0},\"brandedFare\":\"STANDARD\"},{\"segmentId\":\"31\",\"cabin\":\"ECONOMY\",\"fareBasis\":\"LW4LZATG\",\"class\":\"L\",\"includedCheckedBags\":{\"quantity\":0},\"brandedFare\":\"STANDARD\"}]}]}},\"BillingDetails\":{\"CCHolderName\":\"Aaron Ehgoetz\",\"CardNumber\":\"5193910011214240\",\"CVVNumber\":\"127\",\"ExpiryYear\":2024,\"ExpiryMonth\":12,\"CardType\":2,\"Email\":\"rhillier@waygarcapital.com\",\"EmailConfirm\":null,\"Country\":\"CA\",\"State\":\"ON\",\"StateName\":null,\"ZipCode\":\"M5L 2A1\",\"AddressLine1\":\"25 king St West, Suite 1700\",\"AddressLine2\":null,\"AddressLine3\":null,\"City\":\"Toronto\",\"BillingPhone\":\"4165720025\",\"ContactPhone\":null,\"IsPrimaryCard\":false,\"AreaCode\":null,\"CountryCode\":\"1\"},\"Travellers\":[{\"PaxOrderId\":1,\"PaxType\":1,\"Title\":0,\"FirstName\":\"Rickey\",\"MiddleName\":null,\"LastName\":\"Hillier\",\"Gender\":1,\"DOBDay\":null,\"DOBMonth\":0,\"DOBYear\":null,\"PassportNumber\":null,\"PassportIssuingCountry\":null,\"PassportExpiryDate\":null}],\"PriceIncrease\":0.0,\"BagInsuranc\":null,\"TravelerInsurance\":null,\"CouponDetails\":null,\"ExtendedCancellation\":null,\"Currency\":0,\"CurrencyConversion\":0.0,\"CurrencyCode\":null},{\"Transaction\":{\"Id\":0,\"Guid\":null,\"PNR\":\"INPROGRESS\",\"ReferenceNumber\":null,\"GDS\":0,\"ProviderId\":0,\"PortalId\":0,\"BookingType\":0,\"BookingStatus\":0,\"BookingSubStatus\":0,\"BookingSourceType\":0,\"AgentId\":0,\"AgentLead\":0,\"UserId\":0,\"BookedOn\":\"2021-11-27T00:00:00.0237005Z\"},\"FlightSearch\":{\"SearchGuidId\":\"aa28d3bb7de44a22be8fc4bf5abc82a5\",\"PortalId\":1000,\"AffiliateId\":1000,\"TripType\":2,\"Origin\":\"PHL\",\"Destination\":\"MCO\",\"Departure\":\"2022-01-18T00:00:00\",\"Return\":\"2022-01-24T00:00:00\",\"Adult\":1,\"Senior\":0,\"Child\":0,\"InfantOnSeat\":0,\"InfantOnLap\":0,\"Cabin\":3,\"PreferredCarrier\":null,\"IsDirectFlight\":false,\"IP\":\"159.250.65.99\",\"IsMobileDevice\":false,\"UserId\":0,\"UserAgent\":null,\"OriginSearch\":\"PHL - Philadelphia, United States\",\"DestinationSearch\":\"MCO - Orlando, United States\",\"OriginAirportName\":\"Philadelphia Intl Arpt\",\"OriginCountry\":null,\"DestAirportName\":\"Orlando Intl Arpt\",\"DestCountry\":null,\"IsMetaSearch\":false,\"Created\":\"0001-01-01T00:00:00\",\"UtmSource\":null,\"UtmMedium\":null,\"UtmCampaign\":null,\"UtmTerm\":null,\"UtmContent\":null,\"UtmKeyword\":null,\"ClickedId\":null,\"PageType\":0,\"PageId\":0,\"SearchDateTime\":\"2021-11-26T18:37:33.8592357Z\",\"FlexiblityQualifier\":null},\"Contract\":{\"SearchGuid\":\"aa28d3bb7de44a22be8fc4bf5abc82a5\",\"ContractId\":118,\"Provider\":6,\"GDSType\":6,\"Origin\":\"PHL\",\"OriginCityName\":\"Philadelphia\",\"Destination\":\"MCO\",\"DestinationCityName\":\"Orlando\",\"OriginSearch\":\"PHL\",\"DestinationSearch\":\"MCO\",\"DepartureDate\":\"2022-01-18T13:14:00\",\"ArrivalDate\":\"2022-01-24T10:45:00\",\"TripDetails\":{\"OutBoundSegment\":[{\"Id\":1,\"IsReturn\":false,\"IsDepartDateHighlight\":false,\"IsOriginHighlight\":false,\"IsDestinationHighlight\":false,\"Departure\":\"2022-01-18T00:00:00\",\"Arrival\":\"2022-01-18T00:00:00\",\"MarketingCarrier\":{\"Code\":\"F9\",\"Name\":\"Frontier Airlines\",\"IsLowcost\":false},\"OperatingCarrier\":{\"Code\":\"F9\",\"Name\":\"Frontier Airlines\",\"IsLowcost\":false},\"DepartureTime\":\"13:14:00\",\"ArrivalTime\":\"16:00:00\",\"StopOverTime\":null,\"OutTerminal\":\"E\",\"InTerminal\":null,\"EquipmentType\":\"\",\"FlightNumber\":\"1173\",\"CnxType\":null,\"FareBasis\":\"K21EXS2\",\"Class\":\"K\",\"PrevClass\":null,\"Cabin\":\"Economy\",\"CabinType\":1,\"Origin\":\"PHL\",\"OriginCity\":\"Philadelphia\",\"Destination\":\"MCO\",\"DestinationCity\":\"Orlando\",\"FlightDuration\":\"02:46:00\",\"CompanyFranchiseDetails\":null,\"AvailableSeats\":4,\"NoOfStops\":0,\"IsSoldOut\":false,\"AirlineLocator\":null,\"SegmentStatus\":null,\"SegmentTripProExt\":null,\"SegmentASSExtension\":{\"SegmentId\":\"63\",\"Number\":\"1173\",\"ASSExtensionOperating\":{\"carrierCode\":\"F9\"},\"CarrierCode\":\"F9\",\"AirCraftCode\":\"321\",\"BaggageQuantity\":0}}],\"InBoundSegment\":[{\"Id\":1,\"IsReturn\":false,\"IsDepartDateHighlight\":false,\"IsOriginHighlight\":false,\"IsDestinationHighlight\":false,\"Departure\":\"2022-01-24T00:00:00\",\"Arrival\":\"2022-01-24T00:00:00\",\"MarketingCarrier\":{\"Code\":\"F9\",\"Name\":\"Frontier Airlines\",\"IsLowcost\":false},\"OperatingCarrier\":{\"Code\":\"F9\",\"Name\":\"Frontier Airlines\",\"IsLowcost\":false},\"DepartureTime\":\"10:45:00\",\"ArrivalTime\":\"13:11:00\",\"StopOverTime\":null,\"OutTerminal\":null,\"InTerminal\":\"E\",\"EquipmentType\":\"\",\"FlightNumber\":\"1162\",\"CnxType\":null,\"FareBasis\":\"G21NXS2\",\"Class\":\"G\",\"PrevClass\":null,\"Cabin\":\"Economy\",\"CabinType\":1,\"Origin\":\"MCO\",\"OriginCity\":\"Orlando\",\"Destination\":\"PHL\",\"DestinationCity\":\"Philadelphia\",\"FlightDuration\":\"02:26:00\",\"CompanyFranchiseDetails\":null,\"AvailableSeats\":4,\"NoOfStops\":0,\"IsSoldOut\":false,\"AirlineLocator\":null,\"SegmentStatus\":null,\"SegmentTripProExt\":null,\"SegmentASSExtension\":{\"SegmentId\":\"106\",\"Number\":\"1162\",\"ASSExtensionOperating\":{\"carrierCode\":\"F9\"},\"CarrierCode\":\"F9\",\"AirCraftCode\":\"32N\",\"BaggageQuantity\":0}}]},\"ValidatingCarrier\":{\"Code\":\"F9\",\"Name\":\"Frontier Airlines\",\"IsLowcost\":false},\"FareType\":\"PUBLISHED\",\"TripType\":2,\"IsRefundable\":false,\"Adult\":1,\"Senior\":0,\"Child\":0,\"InfantOnSeat\":0,\"InfantOnLap\":0,\"FareBasisCode\":\"K21EXS2\",\"AdultFare\":{\"PaxCount\":1,\"PaxType\":1,\"GDSPaxType\":null,\"ActualBaseFare\":58.76,\"BaseFare\":58.76,\"Tax\":33.2100029,\"TotalFare\":91.97,\"Markup\":4.78243971,\"SupplierFee\":0.0,\"Discount\":0.0,\"IsSellInsurance\":false,\"InsuranceAmount\":0.0,\"IsSellBaggageInsurance\":false,\"BaggageInsuranceAmount\":0.0,\"FareBaseCode\":\"K21EXS2\",\"CurrencyType\":1,\"IsExtendedCancellation\":false,\"ExtendedCancellationAmount\":0.0,\"BookingFee\":0.0,\"TotalFareV2\":96.75244,\"TotalFarePPax\":96.75244,\"BaseFarePPax\":58.76,\"TotalBaseFare\":58.76,\"TaxPPax\":37.9924431,\"TotalTax\":37.9924431},\"ChildFare\":null,\"InfantOnSeatFare\":null,\"InfantOnLapFare\":null,\"SeniorFare\":null,\"TotalMarkup\":4.78243971,\"TotalSupplierFee\":0.0,\"TotalBaseFare\":58.76,\"TotalTax\":33.2100029,\"TotalGDSFareV2\":96.75244,\"EnginePriority\":0,\"Contractkey\":\"181314F91173241045F91162\",\"DatesKey\":null,\"PricingSource\":\"PUBLISHED\",\"MaxStopOutbound\":0,\"MaxStopInbound\":0,\"IsMultipleAirlineContract\":false,\"MinSeatAvailableForContract\":4,\"IsPhoneOnly\":false,\"ContractType\":1,\"TotalOutBoundFlightDuration\":\"02:46:00\",\"TotalInBoundFlightDuration\":\"02:26:00\",\"AffiliateId\":1000,\"AmadeusSessionToken\":null,\"BookingStatus\":0,\"TotalBookingFee\":0.0,\"BaggageQuantity\":0,\"TripProExt\":null,\"MystiflyExt\":null,\"AmaduesSelfServiceExtension\":{\"Source\":\"GDS\",\"Fees\":[{\"Amount\":\"0.00\",\"Type\":\"SUPPLIER\"},{\"Amount\":\"0.00\",\"Type\":\"TICKETING\"}],\"TravelerPricing\":[{\"travelerId\":\"1\",\"fareOption\":\"STANDARD\",\"travelerType\":\"ADULT\",\"price\":{\"currency\":\"USD\",\"total\":91.97,\"base\":58.76,\"fees\":null,\"grandTotal\":0.0},\"fareDetailsBySegment\":[{\"segmentId\":\"63\",\"cabin\":\"ECONOMY\",\"fareBasis\":\"K21EXS2\",\"class\":\"K\",\"includedCheckedBags\":null,\"brandedFare\":null},{\"segmentId\":\"106\",\"cabin\":\"ECONOMY\",\"fareBasis\":\"G21NXS2\",\"class\":\"G\",\"includedCheckedBags\":null,\"brandedFare\":null}]}]}},\"BillingDetails\":{\"CCHolderName\":null,\"CardNumber\":null,\"CVVNumber\":null,\"ExpiryYear\":0,\"ExpiryMonth\":0,\"CardType\":0,\"Email\":null,\"EmailConfirm\":null,\"Country\":\"US\",\"State\":\"DE\",\"StateName\":null,\"ZipCode\":\"19709\",\"AddressLine1\":\"729 marian dr\",\"AddressLine2\":null,\"AddressLine3\":null,\"City\":\"middletown\",\"BillingPhone\":null,\"ContactPhone\":null,\"IsPrimaryCard\":false,\"AreaCode\":null,\"CountryCode\":\"1\"},\"Travellers\":[{\"PaxOrderId\":1,\"PaxType\":1,\"Title\":0,\"FirstName\":null,\"MiddleName\":null,\"LastName\":null,\"Gender\":0,\"DOBDay\":null,\"DOBMonth\":0,\"DOBYear\":null,\"PassportNumber\":null,\"PassportIssuingCountry\":null,\"PassportExpiryDate\":null}],\"PriceIncrease\":0.0,\"BagInsuranc\":null,\"TravelerInsurance\":null,\"CouponDetails\":null,\"ExtendedCancellation\":null,\"Currency\":0,\"CurrencyConversion\":0.0,\"CurrencyCode\":null}]");

            foreach (var item in bookingDetails)
            {
                int tid = SaveBookingDetails(item);
            }
        }
    }
}
