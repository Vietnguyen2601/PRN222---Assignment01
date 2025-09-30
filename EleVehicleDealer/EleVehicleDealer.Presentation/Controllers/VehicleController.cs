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
                TempData["Error"] = "Dữ liệu xe không hợp lệ. Không nhận được dữ liệu.";
                Console.WriteLine("Lỗi: Vehicle là null");
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine($"Nhận được Vehicle: Id={vehicle.VehicleId}, Model={vehicle.Model}, Type={vehicle.Type}, Color={vehicle.Color}, Price={vehicle.Price}");

            if (vehicle.VehicleId <= 0)
            {
                TempData["Error"] = "Dữ liệu xe không hợp lệ. VehicleId là bắt buộc.";
                Console.WriteLine("Lỗi: VehicleId không hợp lệ hoặc bằng 0");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState không hợp lệ. Lỗi: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                TempData["Error"] = "Dữ liệu đầu vào không hợp lệ. Vui lòng kiểm tra biểu mẫu.";

                // Lấy danh sách xe để truyền vào view Index
                var vehicles = await _context.Vehicles.ToListAsync();
                ViewBag.EditVehicle = vehicle; // Đặt lại dữ liệu xe để hiển thị form chỉnh sửa
                return View("Index", vehicles);
            }

            var updatedVehicle = await _vehicleService.UpdateAsync(vehicle);
            if (updatedVehicle != null)
            {
                TempData["Message"] = "Cập nhật xe thành công!";
                Console.WriteLine("Cập nhật thành công");
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Không tìm thấy xe hoặc xe không hoạt động.";
            Console.WriteLine("Lỗi: Không tìm thấy xe hoặc xe không hoạt động");

            // Lấy danh sách xe để truyền vào view Index
            var vehiclesError = await _context.Vehicles.ToListAsync();
            ViewBag.EditVehicle = vehicle; // Đặt lại dữ liệu xe để hiển thị form chỉnh sửa
            return View("Index", vehiclesError);
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

