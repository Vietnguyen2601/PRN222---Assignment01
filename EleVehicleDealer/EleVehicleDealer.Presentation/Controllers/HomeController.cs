using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.BLL.Services;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.EntityModels;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly EvdmsDatabaseContext _context;
        private readonly IVehicleService _vehicleService;

        public HomeController(EvdmsDatabaseContext context, IVehicleService vehicleService)
        {
            _context = context;
            _vehicleService = vehicleService;
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
    }
}
