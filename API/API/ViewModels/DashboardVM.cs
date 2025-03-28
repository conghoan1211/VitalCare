namespace API.ViewModels
{
    public class DashboardVM
    {
    }

    public class RevenueVM
    {
        public string Date { get; set; }
        public long TotalRevenue { get; set; }
    }

    public class UserStatisticsVM
    {
        public List<AgeGroupVM> AgeGroups { get; set; }
        public List<GenderStatsVM> GenderStats { get; set; }
        public List<LocationStatsVM> LocationStats { get; set; }
    }

    public class AgeGroupVM
    {
        public string AgeGroup { get; set; }
        public int UserCount { get; set; }
    }

    public class GenderStatsVM
    {
        public string Gender { get; set; }
        public int UserCount { get; set; }
    }

    public class LocationStatsVM
    {
        public string Location { get; set; }
        public int UserCount { get; set; }
    }

    public class CategoryStatisticsVM
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int TotalSold { get; set; }
    }

}
