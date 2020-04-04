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

        public StockInfo()
        {

        }

        public StockInfo(string StockName, decimal ChangePercent, decimal ClosedPrice )
        {
            this.StockName = StockName;
            this.ChangePercent = ChangePercent;
            this.ClosedPrice = ClosedPrice;
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
    }
}
