using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.Domain.EntityModels;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly IAccountService _accountService;
        private readonly IOrderService _orderService;

        public HomeController(IVehicleService vehicleService, IAccountService accountService, IOrderService orderService)
        {
            _vehicleService = vehicleService;
            _accountService = accountService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Home"); // Redirect đến action Home
        }
        public async Task<IActionResult> Home()
        {
            var vehicles = await _vehicleService.GetAllVehicleAsync();

            var vehicleTypes = vehicles.Select(v => v.Type).Distinct().ToList();
            var vehicleModels = vehicles.Select(v => v.Model).Distinct().ToList();

            var sortBy = Request.Query["sortBy"].ToString();
            var filterType = Request.Query["filterType"].ToString();
            var filterModel = Request.Query["filterModel"].ToString();
            var minPrice = decimal.TryParse(Request.Query["minPrice"], out var min) ? min : (decimal?)null;
            var maxPrice = decimal.TryParse(Request.Query["maxPrice"], out var max) ? max : (decimal?)null;

            IQueryable<Vehicle> query = vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(filterType) && filterType != "All")
            {
                query = query.Where(v => v.Type == filterType);
            }

            if (!string.IsNullOrEmpty(filterModel) && filterModel != "All")
            {
                query = query.Where(v => v.Model == filterModel);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(v => v.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(v => v.Price <= maxPrice.Value);
            }

            switch (sortBy)
            {
                case "alphabet_desc":
                    query = query.OrderByDescending(v => v.Model);
                    break;
                case "price":
                    query = query.OrderBy(v => v.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(v => v.Price);
                    break;
                case "type":
                    query = query.OrderBy(v => v.Type);
                    break;
                case "type_desc":
                    query = query.OrderByDescending(v => v.Type);
                    break;
                case "alphabet":
                default:
                    query = query.OrderBy(v => v.Model);
                    break;
            }

            vehicles = query.ToList();

            // Truyền dữ liệu qua ViewBag
            ViewBag.VehicleTypes = vehicleTypes;
            ViewBag.VehicleModels = vehicleModels;
            ViewBag.SortBy = sortBy;
            ViewBag.FilterType = filterType;
            ViewBag.FilterModel = filterModel;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Lấy error từ TempData nếu có
            ViewBag.Error = TempData["Error"] as string;

            return View(vehicles);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> Manage()
        {
            if (HttpContext.Session.GetString("Username") == null ||
                !new[] { "Admin", "Staff" }.Contains(HttpContext.Session.GetString("Role")))
            {
                return RedirectToAction("Login", "Account");
            }

            var vehicles = (await _vehicleService.GetAllVehicleAsync()).ToList();
            var accounts = await _accountService.GetAllAsync();
            var orders = (await _orderService.GetAllOrdersAsync()).ToList();

            ViewBag.Vehicles = vehicles;
            ViewBag.Accounts = accounts;
            ViewBag.Orders = orders;

            return View(); // Sử dụng Views/Home/Manage.cshtml để render _ManagementPage
        }
    }
}
