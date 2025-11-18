using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IScheduleService _scheduleService;
        private readonly IAccountService _accountService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            IOrderService orderService,
            IScheduleService scheduleService,
            IAccountService accountService,
            ILogger<CustomerController> logger)
        {
            _orderService = orderService;
            _scheduleService = scheduleService;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Check if user is logged in
                var customerId = HttpContext.Session.GetInt32("AccountId");
                if (!customerId.HasValue)
                {
                    TempData["Error"] = "Please login first.";
                    return RedirectToAction("Login", "Account");
                }

                var username = HttpContext.Session.GetString("Username");
                var email = HttpContext.Session.GetString("Email") ?? "";

                Console.WriteLine($"=== DASHBOARD DEBUG ===");
                Console.WriteLine($"CustomerId: {customerId.Value}");
                Console.WriteLine($"Username: {username}");
                Console.WriteLine($"Email: {email}");

                // Get all orders for customer
                IEnumerable<EleVehicleDealer.Domain.DTOs.Orders.OrderSummaryDto> allOrders;
                try
                {
                    allOrders = await _orderService.GetAllOrdersAsync();
                    Console.WriteLine($"Total orders in system: {allOrders.Count()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR getting all orders: {ex.Message}");
                    Console.WriteLine($"Stack: {ex.StackTrace}");
                    throw;
                }
                
                var customerOrders = allOrders.Where(o => o.CustomerName == username).ToList();
                Console.WriteLine($"Orders for customer '{username}': {customerOrders.Count}");
                
                foreach (var o in customerOrders)
                {
                    Console.WriteLine($"  - Order #{o.OrderId}, Status: {o.Status}, Date: {o.OrderDate}");
                }

                // Get all schedules for customer
                var allSchedules = await _scheduleService.GetAllSchedulesAsync();
                var customerSchedules = allSchedules.Where(s => s.CustomerId == customerId.Value).ToList();
                Console.WriteLine($"Schedules for customer: {customerSchedules.Count}");

                // Get order details for each order
                var purchasedVehicles = new List<CustomerOrderDto>();
                foreach (var order in customerOrders)
                {
                    Console.WriteLine($"Processing order {order.OrderId}...");
                    try
                    {
                        var orderDetail = await _orderService.GetOrderByIdAsync(order.OrderId);
                        
                        if (orderDetail == null)
                        {
                            Console.WriteLine($"  WARNING: Order {order.OrderId} returned NULL");
                            continue;
                        }
                        
                        if (orderDetail.Items == null)
                        {
                            Console.WriteLine($"  WARNING: Order {order.OrderId} has NULL Items");
                            continue;
                        }
                        
                        Console.WriteLine($"  Order {order.OrderId} has {orderDetail.Items.Count} items");
                        
                        foreach (var item in orderDetail.Items)
                        {
                            Console.WriteLine($"    - Item: {item.VehicleModel}, Qty: {item.Quantity}, Price: {item.Price}");
                            purchasedVehicles.Add(new CustomerOrderDto
                            {
                                OrderId = order.OrderId,
                                OrderDate = order.OrderDate,
                                VehicleModel = item.VehicleModel,
                                VehicleType = "",
                                Color = "",
                                Price = item.Price,
                                Quantity = item.Quantity,
                                Status = order.Status,
                                StationName = item.StationName
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ERROR processing order {order.OrderId}: {ex.Message}");
                        Console.WriteLine($"  Stack: {ex.StackTrace}");
                    }
                }
                
                Console.WriteLine($"Total purchased vehicles: {purchasedVehicles.Count}");
                Console.WriteLine($"=== END DEBUG ===");

                // Map to dashboard DTO
                var dashboardData = new CustomerDashboardDto
                {
                    CustomerName = username ?? "Customer",
                    Email = email,
                    PurchasedVehicles = purchasedVehicles,
                    TestDriveSchedules = customerSchedules.Select(s => new CustomerScheduleDto
                    {
                        ScheduleId = s.ScheduleId,
                        VehicleModel = s.VehicleModel,
                        VehicleType = "",
                        StationName = s.StationName,
                        StationLocation = "",
                        ScheduleTime = s.ScheduleTime,
                        Status = s.Status
                    }).ToList()
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR in Dashboard: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                _logger.LogError(ex, "Error loading customer dashboard");
                TempData["Error"] = $"Không thể tải thông tin dashboard: {ex.Message}";
                return RedirectToAction("Home", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelSchedule(int scheduleId)
        {
            try
            {
                var customerId = HttpContext.Session.GetInt32("AccountId");
                if (!customerId.HasValue)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                await _scheduleService.UpdateScheduleStatusAsync(scheduleId, "Cancel");
                return Json(new { success = true, message = "Đã hủy lịch lái thử." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling schedule {ScheduleId}", scheduleId);
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }
    }
}
