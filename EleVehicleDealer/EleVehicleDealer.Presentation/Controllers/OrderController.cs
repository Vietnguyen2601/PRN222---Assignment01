using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,StationCarId,StaffId,PromotionId,Status,IsActive")] Order order)
        {
            try
            {
                _logger.LogInformation("Attempting to create order with CustomerId: {CustomerId}, StationCarId: {StationCarId}, StaffId: {StaffId}, PromotionId: {PromotionId}, Status: {Status}, IsActive: {IsActive}",
                    order.CustomerId, order.StationCarId, order.StaffId, order.PromotionId, order.Status, order.IsActive);

                //if (!ModelState.IsValid)
                //{
                //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                //    _logger.LogWarning("Invalid model state for order creation. Errors: {Errors}", string.Join("; ", errors));
                //    TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường trong form.";
                //    ViewBag.EditOrder = order; // Giữ lại dữ liệu form
                //    return View("OrderManager", await _orderService.GetAllOrdersAsync());
                //}

                order.OrderDate = DateTime.Now;
                order.CreatedAt = DateTime.Now;
                order.UpdatedAt = DateTime.Now;
                order.TotalPrice = 100; // Cần thay bằng logic tính toán thực tế

                await _orderService.CreateOrderAsync(order);
                _logger.LogInformation("Order created successfully with ID: {OrderId}", order.OrderId);
                TempData["Message"] = "Tạo đơn hàng thành công!";
                return RedirectToAction(nameof(OrderManager));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng: {Message}", ex.Message);
                TempData["Error"] = $"Không thể tạo đơn hàng: {ex.Message}";
                ViewBag.EditOrder = order; // Giữ lại dữ liệu form
                return View("OrderManager", await _orderService.GetAllOrdersAsync());
            }
        }

        public async Task<IActionResult> OrderManager()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View("OrderManager", orders); // Sử dụng view OrderManager.cshtml
        }

        // GET: Order/GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Starting GetAll action. Fetching orders from OrderService...");
                var orders = await _orderService.GetAllOrdersAsync();

                if (orders == null)
                {
                    _logger.LogError("OrderService returned null for GetAllOrdersAsync");
                    TempData["Error"] = "Failed to load orders due to service error.";
                    return View("OrderManager", new List<Order>());
                }

                _logger.LogInformation("Retrieved {Count} orders", orders.Count());

                if (!orders.Any())
                {
                    _logger.LogWarning("No orders found in the database");
                    TempData["Error"] = "No orders found in the database.";
                }

                return View("OrderManager", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll action while fetching orders");
                TempData["Error"] = "Failed to load orders. Please check the database connection or try again later.";
                return View("OrderManager", new List<Order>());
            }
        }
    }
}
