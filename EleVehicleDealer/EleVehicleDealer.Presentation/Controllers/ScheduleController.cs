using System;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.Domain.DTOs.Schedules;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly IStationCarService _stationCarService;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(IScheduleService scheduleService, IStationCarService stationCarService, ILogger<ScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _stationCarService = stationCarService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Schedule()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return View(schedules);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScheduleCreateDto schedule)
        {
            try
            {
                _logger.LogInformation("Creating schedule with CustomerId: {CustomerId}, StationCarId: {StationCarId}, Status: {Status}, ScheduleTime: {ScheduleTime}, IsActive: {IsActive}",
                    schedule.CustomerId, schedule.StationCarId, schedule.Status, schedule.ScheduleTime, true);

                await _scheduleService.CreateScheduleAsync(schedule);
                _logger.LogInformation("Schedule created successfully for CustomerId: {CustomerId}", schedule.CustomerId);
                TempData["Message"] = "Tạo lịch trình thành công!";
                var updatedSchedules = await _scheduleService.GetAllSchedulesAsync();
                return View("Schedule", updatedSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lịch trình: {Message}", ex.Message);
                TempData["Error"] = $"Không thể tạo lịch trình: {ex.Message}";
                var schedules = await _scheduleService.GetAllSchedulesAsync();
                return View("Schedule", schedules);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                // Validate status - accept Confirmed, Completed, and Cancel
                var validStatuses = new[] { "Confirmed", "Completed", "Complete", "Cancel" };
                if (string.IsNullOrEmpty(status) || !validStatuses.Contains(status))
                {
                    // Return JSON for AJAX calls
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                        Request.ContentType?.Contains("application/json") == true ||
                        Request.Headers["Accept"].ToString().Contains("*/*"))
                    {
                        return Json(new { success = false, message = "Trạng thái không hợp lệ." });
                    }
                    
                    TempData["Error"] = "Trạng thái không hợp lệ.";
                    return RedirectToAction("Schedule");
                }

                await _scheduleService.UpdateScheduleStatusAsync(id, status);
                _logger.LogInformation("Updated status of schedule {ScheduleId} to {Status}", id, status);
                
                // Return JSON for AJAX calls
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                    Request.ContentType?.Contains("application/json") == true ||
                    Request.Headers["Accept"].ToString().Contains("*/*"))
                {
                    return Json(new { success = true, message = $"Cập nhật trạng thái thành {status} thành công!" });
                }
                
                TempData["Message"] = $"Cập nhật trạng thái thành {status} thành công!";
                return RedirectToAction("Schedule");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái lịch trình {ScheduleId}: {Message}", id, ex.Message);
                
                // Return JSON for AJAX calls
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                    Request.ContentType?.Contains("application/json") == true ||
                    Request.Headers["Accept"].ToString().Contains("*/*"))
                {
                    return Json(new { success = false, message = $"Không thể cập nhật trạng thái: {ex.Message}" });
                }
                
                TempData["Error"] = $"Không thể cập nhật trạng thái: {ex.Message}";
                return RedirectToAction("Schedule");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BookTestDrive([FromBody] TestDriveRequest request)
        {
            try
            {
                // Get CustomerId from session
                var customerId = HttpContext.Session.GetInt32("AccountId");
                if (!customerId.HasValue)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để đặt lịch." });
                }

                _logger.LogInformation("BookTestDrive - VehicleModel: {Model}, StationName: {Station}, Time: {Time}",
                    request.VehicleModel, request.StationName, request.ScheduleTime);

                // Get StationCarId based on StationName and VehicleModel
                var stationCarId = await _stationCarService.GetStationCarIdByStationNameAndModelAsync(
                    request.StationName, request.VehicleModel);

                if (!stationCarId.HasValue)
                {
                    _logger.LogWarning("StationCarId not found for Station: {Station}, Model: {Model}",
                        request.StationName, request.VehicleModel);
                    return Json(new { success = false, message = "Không tìm thấy xe tại trạm này." });
                }

                // Create schedule
                var scheduleDto = new ScheduleCreateDto
                {
                    CustomerId = customerId.Value,
                    StationCarId = stationCarId.Value,
                    ScheduleTime = request.ScheduleTime,
                    Status = "Pending"
                };

                var created = await _scheduleService.CreateScheduleAsync(scheduleDto);
                
                _logger.LogInformation("Test drive scheduled successfully for Customer {CustomerId}", customerId.Value);
                
                return Json(new { success = true, message = "Đặt lịch lái thử thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking test drive: {Message}", ex.Message);
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại sau." });
            }
        }

        public class TestDriveRequest
        {
            public string VehicleModel { get; set; } = string.Empty;
            public string StationName { get; set; } = string.Empty;
            public DateTime ScheduleTime { get; set; }
        }
    }
}
