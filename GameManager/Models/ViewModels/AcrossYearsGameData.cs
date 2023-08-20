using System.Collections.Generic;

namespace GameManager.Models.ViewModels
{
    public class AcrossYearsGameData
    {
        public int[] Years { get; set; } = null;
        public int[] YearlyTotalGames { get; set; } = null;
        public int?[] YearlyTotalSpent { get; set; } = null;
        public int?[] YearlyAverageSpent { get; set; } = null;
        public int[] YearlyTotalBorrowed { get; set; } = null;
        public int[] YearlyTotalReplayed { get; set; } = null;
        public int[] YearlyTotalPhysical { get; set; } = null;
        public int[] YearlyTotalDigital { get; set; } = null;
        public List<List<string>> YearlySystemNames { get; set; } = null;
        public List<List<int>> YearlySystemCounts { get; set; } = null;


    }
}