using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly IStationService _stationService;
        private readonly EvdmsDatabaseContext _context;

        public VehicleController(IVehicleService vehicleService, IStationService stationService, EvdmsDatabaseContext context)
        {
            _vehicleService = vehicleService;
            _stationService = stationService;
            _context = context;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            Console.WriteLine($"ToggleActive called with id: {id}, isActive: {isActive}");
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
            {
                TempData["Error"] = $"Không tìm thấy xe với ID {id}.";
                Console.WriteLine($"Error: Vehicle with ID {id} not found.");
                return RedirectToAction(nameof(Index));
            }

            bool wasActive = vehicle.IsActive;
            vehicle.IsActive = isActive;
            var updatedVehicle = await _vehicleService.UpdateAsync(vehicle);
            if (updatedVehicle == null)
            {
                TempData["Error"] = $"Lỗi khi cập nhật trạng thái xe ID {id}.";
                Console.WriteLine($"Update failed for VehicleId={id}");
                return RedirectToAction(nameof(Index));
            }

            string message = wasActive
                ? "Đã vô hiệu hóa xe thành công."
                : "Đã kích hoạt xe thành công.";
            TempData["Message"] = message;

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Update(Vehicle vehicle)
        {
            Console.WriteLine($"Received vehicle: Id={vehicle.VehicleId}, IsActive={vehicle.IsActive}, Model={vehicle.Model}, Type={vehicle.Type}, Color={vehicle.Color}, Price={vehicle.Price}");
            if (vehicle.VehicleId <= 0)
            {
                TempData["Error"] = "Invalid VehicleId. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine($"ModelState is invalid. Errors: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
            }

            if (ModelState.IsValid)
            {
                var existingVehicle = await _vehicleService.GetByIdAsync(vehicle.VehicleId);
                Console.WriteLine($"Existing vehicle: Id={existingVehicle?.VehicleId}, IsActive={existingVehicle?.IsActive}");
                if (existingVehicle == null)
                {
                    TempData["Error"] = $"Không tìm thấy xe với ID {vehicle.VehicleId}.";
                    return RedirectToAction(nameof(Index));
                }

                existingVehicle.Model = vehicle.Model;
                existingVehicle.Type = vehicle.Type;
                existingVehicle.Color = vehicle.Color;
                existingVehicle.Price = vehicle.Price;
                existingVehicle.IsActive = vehicle.IsActive;

                var updatedVehicle = await _vehicleService.UpdateAsync(existingVehicle);
                Console.WriteLine($"Update result: {updatedVehicle != null}");
                if (updatedVehicle == null)
                {
                    TempData["Error"] = $"Lỗi khi cập nhật xe ID {vehicle.VehicleId}.";
                }
                else
                {
                    TempData["Message"] = "Đã cập nhật xe thành công.";
                }
                return RedirectToAction(nameof(Index));
            }

            var vehicles = await _vehicleService.GetAllVehicleAsync();
            ViewBag.EditVehicle = vehicle;
            return View("Index", vehicles);
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

