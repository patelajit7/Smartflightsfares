using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public enum TripType : byte
    {
        NONE = 0,
        [Description("One way")]
        ONEWAY = 1,
        [Description("Round trip")]
        ROUNDTRIP = 2,
        [Description("Multi job")]
        MULTICITY = 3
    }
    public enum ProviderType : int
    {
        NONE = 0,
        AMADEUS = 1,
        TRIPPRO = 2,
        MYSTIFLY = 3,
        SABRE = 4,
        SABREOT = 5,
        AMADEUSSELFSERVICE = 6

    }
    public enum GDSType : int
    {
        NONE = 0,
        AMADEUS = 1,
        TRIPPRO = 2,
        MYSTIFLY = 3,
        SABRE = 4
    }
    public enum TravellerPaxType : int
    {
        [Description("Pax Type")]
        None = 0,
        [Description("Adult")]
        ADT = 1,
        [Description("Senior")]
        SEN = 2,
        [Description("Child")]
        CHD = 3,
        [Description("Infant on Seat")]
        INS = 4,
        [Description("Infant on lap")]
        INL = 5
    }

    public enum GDSPaxType : byte
    {
        ADT = 0,
        INF = 1,
        SEN = 2,
        CHD = 3,
        INL = 4,
        YTH = 5,
        CNN = 6,
        INS = 7,
        CH = 8,
        JCB = 9,
        JNN = 10,
        JNS = 11,
        JNF = 12,
        SRC = 13,
    }

    public enum CurrencyType : int
    {
        None = 0,
        USD = 1
    }
    public enum ContractType : int
    {
        None = 0,
        Actual = 1,
        NearBy = 2,
        Flexi = 3,
        NearByFlexi = 4,
        PhoneOnly = 5
    }
    public enum FareType
    {
        PUBLISHED,
        PRIVATE
    }
    public enum CabinType : int
    {
        None = 0,
        [Description("Economy")]
        Economy = 1,
        [Description("Economy Coach")]
        EconomyCoach = 2,
        [Description("Premium Economy")]
        PremiumEconomy = 3,
        [Description("Business")]
        Business = 4,
        [Description("Premium Business")]
        PremiumBusiness = 5,
        [Description("First")]
        First = 6,
        [Description("Basic Economy")]
        BasicEconomy = 7,
        [Description("Premium First")]
        PremiumFirst = 8
    }
    public enum WebTransactionStatus : int
    {
        None = 0,
        [Description("OK-Success")]
        Success = 200,
        [Description("Bad Request")]
        BadRequest = 400,
        [Description("Un Authorized")]
        Unauthorized = 401,
        [Description("Not Be Authenticate")]
        NotBeAuthenticate = 402,
        [Description("Contracts not found")]
        ContractNotFound = 404,
        [Description("Internal Server Error")]
        InternalServerError = 500,
        [Description("Engine Not Found")]
        EngineNotFound = 403,

        [Description("Sold out")]
        Soldout = 501



    }
    public enum GenericAirlineCode
    {
        [Description("Multiple Airlines")]
        MUL = 0
    }
    public enum BookingStatus : int
    {
        None = 0,
        InProgress = 1,
        BookWithHigherPriceChanged = 2,
        BackToListing = 4,
        PendingConfirmation = 5,
        SoldOutOrUnavailable = 7,
        BookWithLowerPrice = 9,
        Error = 505,
        BookingConfirmed = 200,
    }

    public enum PaymentMethod : int
    {
        [Description("Payment Card")]
        None = 0,
        [Description("Visa")]
        Visa = 1,
        [Description("Master Card")]
        MasterCard = 2,
        [Description("American Express")]
        AmericanExpress = 3,
        [Description("Diners Club")]
        DinersClub = 4,
        [Description("Discover")]
        Discover = 5,
        [Description("Electron")]
        Electron = 6,
        [Description("Maestro")]
        Maestro = 7,
        [Description("BC Card")]
        BCCard = 8,
        [Description("JCB")]
        JCB = 9,
        [Description("CC")]
        CC = 10

    }
    public enum OriginType : byte
    {
        None = 0,
        Airport = 1,
        City = 2
    }


    public enum StopType : byte
    {
        [Description("Non Stop")]
        NonStop = 0,
        [Description("1 Stop")]
        OneStop = 1,
        Multi_Stop1 = 1 << 2,
        Multi_Stop2 = 1 << 3,
        Multi_Stop3 = 1 << 4,
        Multi_Stop4 = 1 << 5,
        Multi_Stop5 = 1 << 6,
        [Description("Multi Stop")]
        Multi_Stops = Multi_Stop1 | Multi_Stop2 | Multi_Stop3 | Multi_Stop4 | Multi_Stop5
    }
    public enum TravellerTitleType : int
    {
        [Description("Title Type")]
        None = 0,
        [Description("Mr.")]
        MR = 1,
        [Description("Mrs.")]
        MRS = 2,
        [Description("Ms.")]
        MS = 3,

    }
    public enum GenderType : int
    {
        [Description("Select")]
        None = 0,
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female = 2
    }
    public enum BookingTimeZone : int
    {
        [Description("Coordinated Universal Time")]
        CoordinatedUniversalTime = 0,
        [Description("Pacific Standard Time")]
        PacificStandardTime = 1,
        [Description("Philippine Time Zone")]
        PhilippineTimeZone = 2,
        [Description("Eastern Standard Time")]
        EasternStandardTime = 3,
        [Description("India Time Zone")]
        IndiaTimeZone = 4
    }
    public enum EmailType : int
    {
        None = 0,
        [Description("Booking Receipt")]
        BookingReceipt = 1,
        [Description("Etickets")]
        ETickets = 2,
        [Description("DocuSign")]
        DocuSign = 3,
        [Description("Payment Receipt")]
        PaymentReceipt = 4,
        [Description("Payment Confirmation")]
        PaymentConfirmation = 5,
        [Description("Itinery Details")]
        ItineryDetails = 6,
        [Description("Self Booking")]
        SelefBooking = 7
    }

    public enum PageBannerType
    {
        None = 0,
        Airline = 1,
        Destination = 2,
        DestinationRoutes = 3,
        DestinationRoutesTo = 4,
        Blogs = 5,
        Flights = 6,
        ThemeHoliday = 7,
        Offers = 8

    }
    public enum SubscriptionType : int
    {
        All = 0,
        FareAlert = 1
    }
    public enum Status : int
    {
        None = 0,
        Pending = 1,
        InProgress = 2,
        NotInterested = 3,
        Converted = 4,
        OtherReason = 5
    }
    public enum DocuSignsStatus : int
    {
        Pending = 0,
        Viewed = 1,
        Signed = 2,
        Finish = 3
    }
    public enum BagInsuranceType : int
    {
        NONE = 0,
        GOLD = 1,
        PLATINUM = 2,
        DIAMOND = 3
    }
    public enum DealThemeType : int
    {
        None = 0,
        [Description("Theme Page")]
        ThemePage = 1,

        [Description("Holiday Page")]
        HolidayPage = 2,

        [Description("Seasonal Page")]
        SeasonalPage = 3,

        [Description("Travel Type Page")]
        TravelTypePage = 4
    }
    public enum PayStatus : int
    {
        None = 0,
        Pending = 1,
        Success = 2,
        Error = 3
    }
    public enum StopsType : int
    {
        NonStop = 0,
        NonNOneStop = 1,
        MultiStop = 2
    }
    public enum Tabs : int
    {
        AllFlights = 0,
        ShortestDirectFlights = 1,
        NearbyFlights = 2,
        FlexFlights = 3,
        MultiAirines = 4,
        PhoneOnly = 5
    }
    public enum PriceChangeAction : int
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }

    public enum PagePopupType : int
    {
        None = 0,
        FocusCall = 1,
        FocusCallnBooking = 2,
        FocusCallCallnBooking = 3
    }
    public enum SocialMediaLogin : int
    {
        None = 0,
        Facebook = 1,
        Google = 2
    }
    public enum BookingType : int
    {
        None = 0,
        Flight = 1,
        Hotel = 2,
        Car = 3
    }
    public enum BookingSourceType : int
    {
        None = 0,
        OnlineBooking = 1,
        OfflineBooking = 2
    }
    public enum BookingSubStatus : int
    {
        [Description("TICKET AND MCO ISSUED")]
        TicketAndMCOIssued = 5,
        [Description("AL BOOKING MCO ISSUED")]
        ALBookingMCOIssued = 10,
        [Description("MCO CHARGED FOR CHANGES & CANCELLATIONS")]
        MCOChargedForChangesNCancellations = 15,
        [Description("MCO CHARGED FOR CHANGES")]
        MCOChargedForChanges = 20,
        [Description("TICKET ISSUED ON SPIRIT")]
        TicketIssuedOnSpirit = 25,
    }

    public enum AffiliateType : int
    {
        Efarehub = 1000,
        Wizfairtours = 2000,
        Gotripadvisors = 3000,
        Kayak = 4000
    }
    public enum DiscountType
    {
        Amount = 1,
        Percentage = 2
    }
}
