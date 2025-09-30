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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("VehicleId,Model,Type,Color,Price")] Vehicle vehicle)
        {
            if (vehicle == null)
            {
                TempData["Error"] = "Invalid vehicle data. No data received.";
                Console.WriteLine("Error: Vehicle is null");
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine($"Received Vehicle: Id={vehicle.VehicleId}, Model={vehicle.Model}, Type={vehicle.Type}, Color={vehicle.Color}, Price={vehicle.Price}");

            if (vehicle.VehicleId <= 0)
            {
                TempData["Error"] = "Invalid vehicle data. VehicleId is required.";
                Console.WriteLine("Error: VehicleId is invalid or zero");
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var updatedVehicle = await _vehicleService.UpdateAsync(vehicle);
                if (updatedVehicle != null)
                {
                    TempData["Message"] = "Vehicle updated successfully!";
                    Console.WriteLine("Update successful");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Vehicle not found or not active.";
                    Console.WriteLine("Error: Vehicle not found or not active");
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                TempData["Error"] = "Invalid input data. Please check the form.";
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
    }
}

