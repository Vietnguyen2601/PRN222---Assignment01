using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.Domain.DTOs.Orders;
using EleVehicleDealer.Presentation.Modal;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly IStationCarService _stationCarService;
        private readonly IVehicleService _vehicleService;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger, IStationCarService stationCardService, IVehicleService vehicleService)
        {
            _orderService = orderService;
            _logger = logger;
            _stationCarService = stationCardService;
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> OrderManager(int? editId)
        {
            try
            {
                _logger.LogInformation("Fetching orders for OrderManager with editId: {EditId}", editId);
                var orders = await _orderService.GetAllOrdersAsync();
                if (orders == null || !orders.Any())
                {
                    _logger.LogWarning("No orders found in the database");
                    TempData["Error"] = "No orders found in the database.";
                }
                ViewBag.EditId = editId;
                if (editId.HasValue)
                {
                    ViewBag.EditOrderDetail = await _orderService.GetOrderByIdAsync(editId.Value);
                }
                return View("OrderManager", orders ?? Enumerable.Empty<OrderSummaryDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for OrderManager");
                TempData["Error"] = "Failed to load orders. Please try again later.";
                return View("OrderManager", new List<OrderSummaryDto>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("Invalid model state for order creation. Errors: {Errors}", string.Join("; ", errors));
                    TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường trong form.";
                    ViewBag.EditOrder = viewModel;
                    return View("OrderManager", await _orderService.GetAllOrdersAsync());
                }

                var prices = await _vehicleService.GetVehiclePriceByModelAsync(viewModel.Model);
                var totalPrice = prices.Any() ? prices.First() : throw new InvalidOperationException($"No active vehicle price found for model: {viewModel.Model}");

                var stationCarId = await _stationCarService.GetStationCarIdByStationNameAndModelAsync(viewModel.StationName, viewModel.Model);
                if (stationCarId == null)
                {
                    _logger.LogWarning("No active StationCar found for StationName: {StationName}, Model: {Model}", viewModel.StationName, viewModel.Model);
                    TempData["Error"] = "Không tìm thấy StationCar phù hợp.";
                    ViewBag.EditOrder = viewModel;
                    return View("OrderManager", await _orderService.GetAllOrdersAsync());
                }

                var createDto = new OrderCreateDto
                {
                    CustomerId = viewModel.CustomerId,
                    StaffId = viewModel.StaffId,
                    Status = "Pending",
                    Items = new List<OrderCreateItemDto>
                    {
                        new OrderCreateItemDto
                        {
                            StationCarId = stationCarId.Value,
                            Quantity = Math.Max(1, viewModel.Quantity),
                            Price = totalPrice
                        }
                    }
                };

                var createdOrder = await _orderService.CreateOrderAsync(createDto);
                _logger.LogInformation("Order created successfully with ID: {OrderId}", createdOrder?.OrderId);
                TempData["Message"] = "Tạo đơn hàng thành công!";
                return RedirectToAction(nameof(OrderManager));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng: {Message}", ex.Message);
                TempData["Error"] = $"Không thể tạo đơn hàng: {ex.Message}";
                ViewBag.EditOrder = viewModel;
                return View("OrderManager", await _orderService.GetAllOrdersAsync());
            }
        }

        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder([FromForm] int orderId, [FromForm] string status, [FromForm] bool isActive)
        {
            if (orderId <= 0 || string.IsNullOrWhiteSpace(status))
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction("OrderManager", new { editId = orderId });
            }
            try
            {
                var dto = new OrderUpdateDto
                {
                    OrderId = orderId,
                    Status = status,
                    IsActive = isActive
                };
                await _orderService.UpdateOrderAsync(dto);
                _logger.LogInformation("Order updated successfully. OrderId: {OrderId}", orderId);
                TempData["Message"] = "Cập nhật đơn hàng thành công!";
                return RedirectToAction("OrderManager");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order: {Message}", ex.Message);
                TempData["Error"] = $"Lỗi khi cập nhật đơn hàng: {ex.Message}";
                return RedirectToAction("OrderManager", new { editId = orderId });
            }
        }

        [HttpPost("SoftDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var existing = await _orderService.GetOrderByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Order not found for soft delete. OrderId: {OrderId}", id);
                    TempData["Error"] = "Đơn hàng không tồn tại.";
                    return RedirectToAction("OrderManager");
                }
                var dto = new OrderUpdateDto
                {
                    OrderId = id,
                    Status = existing.Status,
                    IsActive = false
                };
                await _orderService.UpdateOrderAsync(dto);
                _logger.LogInformation("Order soft deleted successfully. OrderId: {OrderId}", id);
                TempData["Message"] = "Xóa mềm đơn hàng thành công!";
                return RedirectToAction("OrderManager");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting order: {Message}", ex.Message);
                TempData["Error"] = $"Lỗi khi xóa mềm đơn hàng: {ex.Message}";
                return RedirectToAction("OrderManager");
            }
        }
    }
}
