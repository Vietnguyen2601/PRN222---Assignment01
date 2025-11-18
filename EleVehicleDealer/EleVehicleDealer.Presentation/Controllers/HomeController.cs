using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.Domain.DTOs.Vehicles;
using Microsoft.AspNetCore.Mvc;
using EleVehicleDealer.Presentation.Helpers;
using System.Text.Json;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly IAccountService _accountService;
        private readonly IOrderService _orderService;
        private readonly IStationService _stationService;
        private readonly IScheduleService _scheduleService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IVehicleService vehicleService, IAccountService accountService, IOrderService orderService, IStationService stationService, IScheduleService scheduleService, IWebHostEnvironment webHostEnvironment)
        {
            _vehicleService = vehicleService;
            _accountService = accountService;
            _orderService = orderService;
            _stationService = stationService;
            _scheduleService = scheduleService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Home"); // Redirect đến action Home
        }

        public async Task<IActionResult> Home()
        {
            var vehicles = (await _vehicleService.GetAllVehicleAsync()).ToList();

            // Lấy hoặc tạo mapping VehicleId -> ImageName
            var imageMapping = HttpContext.Session.GetString("VehicleImageMapping");
            Dictionary<int, string> mapping;
            
            if (string.IsNullOrEmpty(imageMapping))
            {
                mapping = new Dictionary<int, string>();
                var availableImages = ImageHelper.GetAllImages(_webHostEnvironment);
                var random = new Random();
                
                foreach (var vehicle in vehicles)
                {
                    if (availableImages.Count > 0)
                    {
                        mapping[vehicle.VehicleId] = availableImages[random.Next(availableImages.Count)];
                    }
                    else
                    {
                        mapping[vehicle.VehicleId] = "default.jpg";
                    }
                }
                
                HttpContext.Session.SetString("VehicleImageMapping", JsonSerializer.Serialize(mapping));
            }
            else
            {
                mapping = JsonSerializer.Deserialize<Dictionary<int, string>>(imageMapping) ?? new Dictionary<int, string>();
            }

            ViewBag.ImageMapping = mapping;

            var vehicleTypes = vehicles.Select(v => v.Type).Distinct().ToList();
            var vehicleModels = vehicles.Select(v => v.Model).Distinct().ToList();

            var sortBy = Request.Query["sortBy"].ToString();
            var filterType = Request.Query["filterType"].ToString();
            var filterModel = Request.Query["filterModel"].ToString();
            var minPrice = decimal.TryParse(Request.Query["minPrice"], out var min) ? min : (decimal?)null;
            var maxPrice = decimal.TryParse(Request.Query["maxPrice"], out var max) ? max : (decimal?)null;

            IQueryable<VehicleCatalogDto> query = vehicles.AsQueryable();

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

            ViewBag.VehicleTypes = vehicleTypes;
            ViewBag.VehicleModels = vehicleModels;
            ViewBag.SortBy = sortBy;
            ViewBag.FilterType = filterType;
            ViewBag.FilterModel = filterModel;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
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
            var accounts = (await _accountService.GetAllAsync()).ToList();
            var orders = (await _orderService.GetAllOrdersAsync()).ToList();
            var schedules = (await _scheduleService.GetAllSchedulesAsync()).ToList();

            ViewBag.Vehicles = vehicles;
            ViewBag.Accounts = accounts;
            ViewBag.Orders = orders;
            ViewBag.Schedules = schedules;

            return View(); // Sử dụng Views/Home/Manage.cshtml để render _ManagementPage
        }

        [HttpGet]
        public async Task<IActionResult> GetStations()
        {
            try
            {
                var stations = await _stationService.GetAllStationsAsync();
                var activeStations = stations.Where(s => s.IsActive).ToList();
                return Json(activeStations);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStationsByVehicleModel(string model)
        {
            try
            {
                if (string.IsNullOrEmpty(model))
                {
                    return Json(new { error = "Model is required" });
                }
                var stations = await _stationService.GetStationsByVehicleModelAsync(model);
                return Json(stations);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
