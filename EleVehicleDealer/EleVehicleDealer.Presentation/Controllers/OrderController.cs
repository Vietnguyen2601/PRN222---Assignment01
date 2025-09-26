using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EditOrder = null; // Sử dụng ViewBag để truyền dữ liệu vào view
                //return View("Index", await _orderService.GetAllOrdersAsync());
            }

            order.IsActive = true; // Đặt trạng thái mặc định
            await _orderService.CreateOrderAsync(order); // Gọi service để tạo đơn hàng
            TempData["Message"] = "Order created successfully!"; // Thông báo thành công
            return RedirectToAction(nameof(Index)); // Chuyển hướng về danh sách
        }

        //Temp to push



        //public async Task<IActionResult> Index()
        //{
        //    //var orders = await _orderService.GetAllOrdersAsync();
        //    //return View(orders); // Trả về view Index với danh sách đơn hàng
        //}
    }
}
