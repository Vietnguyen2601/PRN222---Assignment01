using EleVehicleDealer.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public AdminController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Dashboard(int? year)
        {
            // Kiểm tra quyền Admin
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                TempData["Error"] = "You don't have permission to access this page.";
                return RedirectToAction("Index", "Home");
            }

            var selectedYear = year ?? DateTime.Now.Year;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.Years = Enumerable.Range(DateTime.Now.Year - 5, 6).Reverse().ToList();

            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            
            // Lấy dữ liệu cho năm được chọn
            dashboardData.RevenueByQuarter = await _dashboardService.GetRevenueByQuarterAsync(selectedYear);
            dashboardData.RevenueByMonth = await _dashboardService.GetRevenueByMonthAsync(selectedYear);

            return View(dashboardData);
        }

        [HttpPost]
        public async Task<IActionResult> GetCustomDateRangeRevenue([FromBody] CustomDateRequest request)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var data = await _dashboardService.GetRevenueByCustomDateRangeAsync(request.StartDate, request.EndDate);
                
                // Debug log
                Console.WriteLine($"Custom Date Range: {request.StartDate} to {request.EndDate}");
                Console.WriteLine($"Total Revenue: {data.TotalRevenue}, Total Orders: {data.TotalOrders}");
                Console.WriteLine($"Daily Revenue Count: {data.DailyRevenue.Count}");
                
                return new JsonResult(new { success = true, data }, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCustomDateRangeRevenue: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetStationComparison([FromBody] StationDateRequest request)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var data = await _dashboardService.GetRevenueByStationAsync(request.StartDate, request.EndDate);
                
                Console.WriteLine($"Station Comparison: {data.Count} stations");
                
                return new JsonResult(new { success = true, data }, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetStationComparison: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class CustomDateRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class StationDateRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
