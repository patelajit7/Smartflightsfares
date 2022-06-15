using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel.API
{
    public class AirMatrixMain
    {
        public AirMatrixMain()
        {
            this.AirlineMatrixList = new List<AirlineMatrixColumn>();
        }

        public List<AirlineMatrixColumn> AirlineMatrixList { get; set; }
        public TripType TripType { get; set; }
        public StopsType StopsType { get; set; }
        //public bool IsNonStopExist()
        //{
        //    var nonStop = this.AirlineMatrixList.Where<AirlineMatrixColumn>(o => o.IsNonStop == true).FirstOrDefault();
        //    if (nonStop != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        public bool IsNonStopPlusOne()
        {
            bool res = false;
            AirlineMatrixColumn airlineMatrixColumn= AirlineMatrixList.Where<AirlineMatrixColumn>(o => o.IsNonStop == true).FirstOrDefault<AirlineMatrixColumn>();
            if (airlineMatrixColumn != null)
            {
                res = true;
            }
            return res;
        }
        public bool IsMultiStop()
        {
            bool res = false;
            AirlineMatrixColumn airlineMatrixColumn = AirlineMatrixList.Where<AirlineMatrixColumn>(o => o.IsMultiStop == true).FirstOrDefault<AirlineMatrixColumn>();
            if (airlineMatrixColumn != null)
            {
                res = true;
            }
            return res;
        }
    }
    public class AirlineMatrixColumn
    {
        public Airline AirlineInfo { get; set; }
        public bool IsNonStop { get; set; }
        public ContractType NonContractType { get; set; }
        public float NonStopFare { get; set; } /*RoundTrip  0-0 or 0-1, One-way: 0*/
        public bool IsMultiStop { get; set; }
        public ContractType MultiContractType { get; set; }
        public float MultiStopFare { get; set; }
        public string AirlineImageUrl { get; set; }
        public float AirlineMinPrice { get; set; }
        public bool IsMultipleAirline { get; set; }

        //public float GetMinimumAmount()
        //{
        //    float minamount = this.NonStopFare;
        //    if (this.IsNonStop && minamount >= this.NonStopFare)
        //    {
        //        minamount = this.NonStopFare;
        //    }
        //    if (this.IsMultiStop && minamount >= this.MultiStopFare)
        //    {
        //        minamount = this.MultiStopFare;
        //    }
        //    return minamount;
        //}
    }
}
