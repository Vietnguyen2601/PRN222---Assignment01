using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly IStationService _stationService;

        public VehicleController(IVehicleService vehicleService, IStationService stationService)
        {
            _vehicleService = vehicleService;
            _stationService = stationService;
        }

        public async Task<IActionResult> Index(int? editId)
        {
            var vehicles = await _vehicleService.GetAllVehicleAsync();
            if (editId.HasValue)
            {
                var editVehicle = await _vehicleService.GetByIdAsync(editId.Value);
                if (editVehicle != null)
                {
                    ViewBag.EditVehicle = editVehicle;
                }
            }
            return View(vehicles);
        }

        public async Task<IActionResult> Catalog(string searchTerm, string type, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<Vehicle> vehicles;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                vehicles = await _vehicleService.SearchVehiclesAsync(searchTerm);
            }
            else if (!string.IsNullOrEmpty(type))
            {
                vehicles = await _vehicleService.GetVehiclesByTypeAsync(type);
            }
            else if (minPrice.HasValue && maxPrice.HasValue)
            {
                vehicles = await _vehicleService.GetVehiclesByPriceRangeAsync(minPrice.Value, maxPrice.Value);
            }
            else
            {
                vehicles = await _vehicleService.GetAllVehicleAsync();
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Type = type;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View("Catalog", vehicles);
        }

        public async Task<IActionResult> Compare(int[] vehicleIds)
        {
            if (vehicleIds == null || vehicleIds.Length < 2 || vehicleIds.Length > 4)
            {
                TempData["Error"] = "Please select 2-4 vehicles to compare.";
                return RedirectToAction(nameof(Index));
            }

            var vehicles = new List<Vehicle>();
            foreach (var id in vehicleIds)
            {
                var vehicle = await _vehicleService.GetByIdAsync(id);
                if (vehicle != null)
                {
                    vehicles.Add(vehicle);
                }
            }

            if (vehicles.Count < 2)
            {
                TempData["Error"] = "Some vehicles not found or not active.";
                return RedirectToAction(nameof(Index));
            }

            return View(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EditVehicle = null;
                return View("Index", await _vehicleService.GetAllVehicleAsync());
            }
            // Loại bỏ logic Availability
            vehicle.IsActive = true;
            await _vehicleService.CreateAsync(vehicle);
            TempData["Message"] = "Vehicle created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                // Loại bỏ logic Availability
                await _vehicleService.UpdateAsync(vehicle);
                TempData["Message"] = "Vehicle updated successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vehicleService.DeleteAsync(id);
            if (success)
            {
                TempData["Message"] = "Vehicle soft deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete vehicle.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Home(int? stationId, string sortBy, string filterType, string filterModel, decimal? minPrice, decimal? maxPrice)
        {
            var stations = await _stationService.GetAllStationsAsync();
            var vehicles = stationId.HasValue && stationId.Value > 0
                ? await _vehicleService.GetVehiclesByStationAsync(stationId.Value)
                : await _vehicleService.GetAllVehicleAsync();

            // Apply filters
            if (!string.IsNullOrEmpty(filterType) && filterType != "All")
            {
                vehicles = vehicles.Where(v => v.Type.Contains(filterType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filterModel) && filterModel != "All")
            {
                vehicles = vehicles.Where(v => v.Model.Contains(filterModel, StringComparison.OrdinalIgnoreCase));
            }

            if (minPrice.HasValue)
            {
                vehicles = vehicles.Where(v => v.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                vehicles = vehicles.Where(v => v.Price <= maxPrice.Value);
            }

            // Apply sorting
            vehicles = sortBy switch
            {
                "alphabet_desc" => vehicles.OrderByDescending(v => v.Model),
                "price" => vehicles.OrderBy(v => v.Price),
                "price_desc" => vehicles.OrderByDescending(v => v.Price),
                "type" => vehicles.OrderBy(v => v.Type),
                "type_desc" => vehicles.OrderByDescending(v => v.Type),
                _ => vehicles.OrderBy(v => v.Model) // default alphabet
            };

            if (stations == null || !stations.Any())
            {
                TempData["Error"] = "No stations found.";
            }

            if (vehicles == null || !vehicles.Any())
            {
                TempData["Error"] = "No vehicles found for the selected criteria.";
            }

            // Prepare ViewBag data
            var allVehicles = await _vehicleService.GetAllVehicleAsync();
            ViewBag.Stations = stations;
            ViewBag.SelectedStationId = stationId;
            ViewBag.SortBy = sortBy;
            ViewBag.FilterType = filterType;
            ViewBag.FilterModel = filterModel;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.VehicleTypes = allVehicles.Select(v => v.Type).Distinct().OrderBy(t => t).ToList();
            ViewBag.VehicleModels = allVehicles.Select(v => v.Model).Distinct().OrderBy(m => m).ToList();

            return View(vehicles);
        }
    }
}

