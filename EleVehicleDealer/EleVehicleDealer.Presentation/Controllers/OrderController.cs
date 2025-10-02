using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.Presentation.Modal;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly IStationCardService _stationCarService;
        private readonly IVehicleService _vehicleService;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger, IStationCardService stationCardService, IVehicleService vehicleService)
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
                return View("OrderManager", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for OrderManager");
                TempData["Error"] = "Failed to load orders. Please try again later.";
                return View("OrderManager", new List<Order>());
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

                var order = new Order
                {
                    CustomerId = viewModel.CustomerId,
                    StaffId = viewModel.StaffId,
                    PromotionId = viewModel.PromotionId,
                    Status = "Pending",
                    IsActive = true,
                    OrderDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    TotalPrice = totalPrice
                };

                var stationCarId = await _stationCarService.GetStationCarIdByStationNameAndModelAsync(viewModel.StationName, viewModel.Model);
                if (stationCarId == null)
                {
                    _logger.LogWarning("No active StationCar found for StationName: {StationName}, Model: {Model}", viewModel.StationName, viewModel.Model);
                    TempData["Error"] = "Không tìm thấy StationCar phù hợp.";
                    ViewBag.EditOrder = viewModel;
                    return View("OrderManager", await _orderService.GetAllOrdersAsync());
                }
                order.StationCarId = stationCarId.Value;

                await _orderService.CreateOrderAsync(order);
                _logger.LogInformation("Order created successfully with ID: {OrderId}", order.OrderId);
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
        public async Task<IActionResult> UpdateOrder([FromForm] Order model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for order update. OrderId: {OrderId}", model.OrderId);
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction("OrderManager", new { editId = model.OrderId });
            }
            try
            {
                var existingOrder = await _orderService.GetOrderByIdAsync(model.OrderId);
                if (existingOrder == null)
                {
                    _logger.LogWarning("Order not found for update. OrderId: {OrderId}", model.OrderId);
                    TempData["Error"] = "Đơn hàng không tồn tại.";
                    return RedirectToAction("OrderManager");
                }
                existingOrder.CustomerId = model.CustomerId;
                existingOrder.StationCarId = model.StationCarId;
                existingOrder.OrderDate = model.OrderDate;
                existingOrder.TotalPrice = model.TotalPrice;
                existingOrder.Status = model.Status;
                existingOrder.UpdatedAt = DateTime.Now;
                await _orderService.UpdateOrderAsync(existingOrder);
                _logger.LogInformation("Order updated successfully. OrderId: {OrderId}", model.OrderId);
                TempData["Message"] = "Cập nhật đơn hàng thành công!";
                return RedirectToAction("OrderManager");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order: {Message}", ex.Message);
                TempData["Error"] = $"Lỗi khi cập nhật đơn hàng: {ex.Message}";
                return RedirectToAction("OrderManager", new { editId = model.OrderId });
            }
        }

        [HttpPost("SoftDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Order not found for soft delete. OrderId: {OrderId}", id);
                    TempData["Error"] = "Đơn hàng không tồn tại.";
                    return RedirectToAction("OrderManager");
                }
                order.IsActive = false;
                order.UpdatedAt = DateTime.Now;
                await _orderService.UpdateOrderAsync(order);
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
