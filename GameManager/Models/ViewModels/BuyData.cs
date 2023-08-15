using Microsoft.Ajax.Utilities;
using System.Collections.Generic;

namespace GameManager.Models.ViewModels
{
    public class BuyData
    {
        public int TotalSpent { get; set; }
        public int TotalGames { get; set; }
        public int AverageSpend { get; set; }
        public int TotalPhysical { get; set; }
        public int TotalDigital { get; set; }
        public int[] MonthlyTotalSpent { get; set; }
        public int[] MonthlyTotalGames { get; set; }
        public int[] MonthlyTotalPhysical { get; set; }
        public int[] MonthlyTotalDigital { get; set; }
        public int[] TotalByPriceRange { get; set; }
        public string[] SystemNames { get; set; }
        public int[] SystemCounts { get; set; }

        public string[] Months { get; set; } = new string[] { "January", "Feburary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public string[] PriceLabels = new string[] { "<$20", "$20-$29", "$30-$39", "$40-$49", "$50-$59", "$60-$69", "$70-$79", "$80-$89", "$90-$99", ">$100" };
    }
}