using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public interface IDashboardService
    {
        public Task<(string, long)> GetTotalRevenue();
        public Task<(string, List<RevenueVM>?)> GetRevenueByDate(string period);
        public Task<(string, UserStatisticsVM?)> GetUserStatistics();
        public Task<(string, List<CategoryStatisticsVM>?)> GetCategoryStatistics();
    }
    public class DashboardService : IDashboardService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;

        public DashboardService(IMapper mapper, Exe201Context context)
        {
            _context = context;
            _mapper = mapper;
        }

        #region revenue dashboard
        public async Task<(string, long)> GetTotalRevenue()
        {
            var totalRevenue = await _context.Orders
              .Where(o => o.Status == (int)OrderStatus.Completed).SumAsync(o => o.Total);

            return ("", totalRevenue);
        }

        public async Task<(string, List<RevenueVM>?)> GetRevenueByDate(string period)
        {
            var query = _context.Orders.Where(o => o.Status == (int)OrderStatus.Completed); // Chỉ tính đơn hàng đã giao

            if (period == "day")
            {
                var result = await query
                    .GroupBy(o => o.OrderDate.Value.Date)
                    .Select(g => new RevenueVM { Date = g.Key.ToString(), TotalRevenue = g.Sum(o => o.Total) })
                    .OrderBy(g => g.Date)
                    .ToListAsync();
                return ("", result);
            }
            else if (period == "month")
            {
                var result = await query
                   .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                   .Select(g => new RevenueVM
                   {
                       Date = g.Key.Month.ToString() + "/" + g.Key.Year.ToString(), // Ghép chuỗi ngay trong SQL
                       TotalRevenue = g.Sum(o => o.Total)
                   })
                   .OrderBy(g => g.Date)
                   .ToListAsync();
                return ("", result);
            }
            else if (period == "year")
            {
                var result = await query
                    .GroupBy(o => o.OrderDate.Value.Year)
                    .Select(g => new RevenueVM { Date = g.Key.ToString(), TotalRevenue = g.Sum(o => o.Total) })
                    .OrderBy(g => g.Date)
                    .ToListAsync();
                return ("", result);
            }
            return ("Invalid period", null);
        }
        #endregion

        public async Task<(string, UserStatisticsVM?)> GetUserStatistics()
        {
            try
            {
                var query = from o in _context.Orders
                            join u in _context.Users on o.UserId equals u.UserId
                            where o.Status == (int)OrderStatus.Completed // Chỉ lấy đơn đã hoàn thành
                            select new
                            {
                                u.Sex,
                                Age = DateTime.Now.Year - u.Dob.Value.Year, // Tính tuổi
                                u.ProvinceName
                            };

                var ageStats = await query
                    .GroupBy(u => new
                    {
                        AgeGroup = u.Age <= 20 ? "0-20" :
                                   u.Age <= 30 ? "21-30" :
                                   u.Age <= 45 ? "31-45" : "46+"
                    })
                    .Select(g => new AgeGroupVM
                    {
                        AgeGroup = g.Key.AgeGroup,
                        UserCount = g.Count()
                    }).ToListAsync();

                var genderStats = await query
                    .GroupBy(u => u.Sex)
                    .Select(g => new GenderStatsVM
                    {
                        Gender = g.Key == (int)Gender.Male ? "Nam" : g.Key == (int)Gender.Female ? "Nữ" : "Khác",
                        UserCount = g.Count()
                    }).ToListAsync();

                var locationStats = await query
                    .GroupBy(u => u.ProvinceName)
                    .Select(g => new LocationStatsVM
                    {
                        Location = g.Key,
                        UserCount = g.Count()
                    }).ToListAsync();

                return ("", new UserStatisticsVM {
                    AgeGroups = ageStats, GenderStats = genderStats, LocationStats = locationStats
                });
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }
        public async Task<(string, List<CategoryStatisticsVM>?)> GetCategoryStatistics()
        {
            try
            {
                var categorySales = await _context.Categories
                   .Where(c => c.IsActive == true && c.TypeObject == (int)TypeCateria.Product)
                   .Select(c => new CategoryStatisticsVM
                   {
                       CategoryID = c.Id,
                       CategoryName = c.Name,
                       TotalSold = _context.Products
                           .Where(p => p.CategoryId == c.Id && p.IsDeleted == false)
                           .Sum(p => (int?)p.Sold) ?? 0
                   }).ToListAsync();

                return ("", categorySales);
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }
        }
    }
}
