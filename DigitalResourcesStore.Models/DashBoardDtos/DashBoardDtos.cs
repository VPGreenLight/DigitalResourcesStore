using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.DashBoardDtos
{
    public class DashBoardDtos
    {
        public int ProductCount { get; set; }
        public int UserCount { get; set; }
        public int FeedbackCount { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<string> AllMonths { get; set; }
        public List<decimal> MonthlyRevenues { get; set; }
        public List<BestSellingProductDtos> BestSellingProducts { get; set; }
        public int QuantitySold { get; set; }
        public decimal PercentGrowth { get; set; }

        public DashBoardDtos()
        {
            AllMonths = new List<string>
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
            };
            MonthlyRevenues = new List<decimal>(new decimal[12]);
        }
    }
}
