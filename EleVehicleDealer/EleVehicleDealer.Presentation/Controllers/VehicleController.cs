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
        public async Task<IActionResult> Home(int? stationId)
        {
            var stations = await _stationService.GetAllStationsAsync();
            var vehicles = stationId.HasValue && stationId.Value > 0
                ? await _vehicleService.GetVehiclesByStationAsync(stationId.Value)
                : await _vehicleService.GetAllVehicleAsync();

            if (stations == null || !stations.Any())
            {
                TempData["Error"] = "No stations found.";
            }

            if (vehicles == null || !vehicles.Any())
            {
                TempData["Error"] = "No vehicles found.";
            }

            ViewBag.Stations = stations;
            ViewBag.SelectedStationId = stationId;

            return View(vehicles);
        }
    }
}

