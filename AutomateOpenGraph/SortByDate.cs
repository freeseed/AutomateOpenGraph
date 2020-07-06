using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateOpenGraph
{
    public class SortByDate : IComparer<StockInfo>
    {


        int IComparer<StockInfo>.Compare(StockInfo x, StockInfo y)
        {
            return (int)y.IPODate.Subtract(x.IPODate).TotalDays;
        }
    }
}
