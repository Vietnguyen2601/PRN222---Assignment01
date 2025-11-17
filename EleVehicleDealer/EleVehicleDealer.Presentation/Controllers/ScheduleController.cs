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
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(IScheduleService scheduleService, ILogger<ScheduleController> logger)
        {
            _scheduleService = scheduleService;
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
                if (string.IsNullOrEmpty(status) || (status != "Complete" && status != "Cancel"))
                {
                    TempData["Error"] = "Trạng thái không hợp lệ.";
                    return RedirectToAction("Schedule");
                }

                await _scheduleService.UpdateScheduleStatusAsync(id, status);
                _logger.LogInformation("Updated status of schedule {ScheduleId} to {Status}", id, status);
                TempData["Message"] = $"Cập nhật trạng thái thành {status} thành công!";
                return RedirectToAction("Schedule");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái lịch trình {ScheduleId}: {Message}", id, ex.Message);
                TempData["Error"] = $"Không thể cập nhật trạng thái: {ex.Message}";
                return RedirectToAction("Schedule");
            }
        }

    }
}
