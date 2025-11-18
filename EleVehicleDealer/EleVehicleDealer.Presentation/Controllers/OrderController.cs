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
        private readonly ICartService _cartService;
        private readonly IAccountService _accountService;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger, IStationCarService stationCardService, IVehicleService vehicleService, ICartService cartService, IAccountService accountService)
        {
            _orderService = orderService;
            _logger = logger;
            _stationCarService = stationCardService;
            _vehicleService = vehicleService;
            _cartService = cartService;
            _accountService = accountService;
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

        public async Task<IActionResult> Checkout()
        {
            // Kiểm tra đăng nhập
            var username = HttpContext.Session.GetString("Username");
            var accountId = HttpContext.Session.GetInt32("AccountId");
            
            if (string.IsNullOrEmpty(username) || !accountId.HasValue)
            {
                TempData["Error"] = "Please login to checkout.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy cart
            var cart = await _cartService.GetCartAsync();
            if (cart.Items.Count == 0)
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Lấy thông tin customer từ AccountService
            var customer = await _accountService.GetByIdAsync(accountId.Value);
            
            // Pre-fill checkout form với thông tin customer có sẵn
            var checkoutDto = new CheckoutDto
            {
                CustomerName = customer?.Username ?? username,
                Email = customer?.Email ?? "",
                Phone = customer?.ContactNumber ?? "",  // Sử dụng ContactNumber thay vì Phone
                Address = "",  // Để trống vì AccountDto không có Address
                City = ""
            };

            ViewBag.Cart = cart;
            
            return View(checkoutDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckout(CheckoutDto checkout)
        {
            // Kiểm tra đăng nhập
            var username = HttpContext.Session.GetString("Username");
            var accountId = HttpContext.Session.GetInt32("AccountId");
            
            if (string.IsNullOrEmpty(username) || !accountId.HasValue)
            {
                TempData["Error"] = "Please login to checkout.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var cart = await _cartService.GetCartAsync();
                ViewBag.Cart = cart;
                return View("Checkout", checkout);
            }

            // Lấy cart
            var cartData = await _cartService.GetCartAsync();
            if (cartData.Items.Count == 0)
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Tạo order items từ cart
            var orderItems = new List<OrderCreateItemDto>();
            foreach (var cartItem in cartData.Items)
            {
                // Giả sử bạn cần tìm StationCarId dựa trên VehicleId
                // Bạn có thể cần thêm logic để lấy StationCarId
                // Tạm thời dùng 1 như default hoặc bỏ qua nếu không bắt buộc
                orderItems.Add(new OrderCreateItemDto
                {
                    StationCarId = 1, // TODO: Lấy StationCarId thực tế
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price
                });
            }

            // Tạo order
            var orderCreateDto = new OrderCreateDto
            {
                CustomerId = accountId.Value,
                StaffId = 1, // TODO: Lấy StaffId nếu cần
                Status = "Pending",
                Items = orderItems
            };

            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(orderCreateDto);
                
                // Clear cart sau khi đặt hàng thành công
                await _cartService.ClearCartAsync();

                // Lưu thông tin order vào TempData để hiển thị ở trang Success
                // Chuyển decimal sang string để TempData có thể serialize
                TempData["OrderId"] = createdOrder.OrderId;
                TempData["CustomerName"] = checkout.CustomerName;
                TempData["TotalAmount"] = cartData.TotalAmount.ToString("F2"); // Chuyển decimal sang string
                TempData["OrderDate"] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create order: {ex.Message}";
                var cart = await _cartService.GetCartAsync();
                ViewBag.Cart = cart;
                return View("Checkout", checkout);
            }
        }

        public IActionResult Success()
        {
            // Lấy thông tin từ TempData
            ViewBag.OrderId = TempData["OrderId"];
            ViewBag.CustomerName = TempData["CustomerName"];
            
            // Parse string về decimal
            var totalAmountStr = TempData["TotalAmount"] as string;
            ViewBag.TotalAmount = decimal.TryParse(totalAmountStr, out var amount) ? amount : 0m;
            
            ViewBag.OrderDate = TempData["OrderDate"];

            return View();
        }
    }
}
