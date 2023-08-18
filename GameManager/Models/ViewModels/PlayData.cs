namespace GameManager.Models.ViewModels
{
    public class PlayData
    {
        public int TotalGames { get; set; }
        public int TotalBorrowed { get; set; }
        public int TotalReplayed { get; set; }
        public int TotalPhysical { get; set; }
        public int TotalDigital { get; set; }
        public int[] MonthlyTotalPlayed { get; set; }
        public int[] MonthlyTotalBorrowed { get; set; }
        public int[] MonthlyTotalReplayed { get; set; }
        public int[] MonthlyTotalPhysical { get; set; }
        public int[] MonthlyTotalDigital { get; set; }
        public string[] SystemNames { get; set; }
        public int[] SystemCounts { get; set; }

        public string[] Months { get; set; } = new string[] { "January", "Feburary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    }
}