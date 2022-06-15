using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Common;
using Configration;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.Deals;

namespace Business
{
    public class DealBusness
    {
        private static readonly MemoryCache _cache = MemoryCache.Default;
        private static string _key = "HomeDeal1000";
        public static HomeDeals GetHomeDeals(string _ip)
        {
            HomeDeals homeDeals = null;
            //if (!_cache.Contains(_key))
            //{
            IATAGeoLocation res = GeoLocation.GetAirportCode(_ip);
            string fromCode = res.IATACode;
            HomeDeal item = Utility.PortalSettings.HomePageDeals.Domestic;
            DealRequest domestic = new DealRequest()
            {
                Airline = item.Airline,
                From = !string.IsNullOrEmpty(fromCode) ? fromCode : item.From,
                To = item.To,
                TripType = item.TripType,
                Guid = item.Guid
            };
            item = Utility.PortalSettings.HomePageDeals.International;
            DealRequest international = new DealRequest()
            {
                Airline = item.Airline,
                From = !string.IsNullOrEmpty(fromCode) ? fromCode : item.From,
                To = item.To,
                TripType = item.TripType,
                Guid = item.Guid
            };
            DealRQ dealRQ = new DealRQ() { Domestic = domestic, International = international, Latitude = res.Latitude, LongiTude = res.Longitude };
            homeDeals = RESTClient.GetDeals(dealRQ);

            //if ((homeDeals !=null && homeDeals.Domestic != null && homeDeals.Domestic.TransactionStatus != null && homeDeals.Domestic.TransactionStatus.IsSuccess)
            //     && (homeDeals != null && homeDeals.International != null && homeDeals.International.TransactionStatus != null && homeDeals.International.TransactionStatus.IsSuccess)
            //    )
            //{
            //    var policy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 5, 0) };
            //    _cache.Add(_key, homeDeals, policy);
            //}
            //}
            //else
            //{
            //    homeDeals = _cache[_key] as HomeDeals;
            //}
            return homeDeals;
        }

    }
    public class DealRQ
    {
        public DealRequest Domestic { get; set; }
        public DealRequest International { get; set; }
        public string Latitude { get; set; }
        public string LongiTude { get; set; }
    }
}
