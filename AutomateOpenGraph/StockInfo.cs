using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateOpenGraph
{
    public class StockInfo : IEquatable<StockInfo>, IComparable<StockInfo>
    {
        public string StockName { get; set; }
        public decimal ChangePercent { get; set; }
        public decimal ClosedPrice { get; set; }
        public DateTime IPODate { get; set; }
        public double IPOSizeML { get; set; }
        public string SETMAI { get; set; }
        public double IPOPrice { get; set; }
        public DateTime SpDate { get; set; }


        public StockInfo()
        {

        }

        public StockInfo(string StockName, decimal ChangePercent, decimal ClosedPrice )
        {
            this.StockName = StockName;
            this.ChangePercent = ChangePercent;
            this.ClosedPrice = ClosedPrice;
        }

        public StockInfo(string StockName, DateTime IPODate,double IPOSizeML, string SETMAI, double IPOPrice, DateTime SpDate)
        {
            this.StockName = StockName;
            this.IPODate = IPODate;
            this.IPOSizeML = IPOSizeML;
            this.SETMAI = SETMAI;
            this.IPOPrice = IPOPrice;
            this.SpDate = SpDate;

        }

        //IEquatable for  contain
        public bool Equals(StockInfo other)
        {
            if (this.StockName == other.StockName)
                return true;
            else
                return false;
        }

        //IComparable for binarysearch
        public int CompareTo(StockInfo other)
        {
            return this.StockName.CompareTo(other.StockName);
        }

        public double IPOLast
        {
            get
            {
                return Math.Ceiling( DateTime.Now.Subtract(this.IPODate).TotalDays);
            }
        } 

        public double DayToSP
        {
            get
            {
                return Math.Ceiling(this.SpDate.Subtract(DateTime.Now).TotalDays);
            }
        }
    }
}
