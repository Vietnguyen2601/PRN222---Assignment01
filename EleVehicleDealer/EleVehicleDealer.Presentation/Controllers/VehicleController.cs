using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
using Microsoft.AspNetCore.Mvc;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> Index(int? editId)
        {
            var vehicles = await _vehicleService.GetAllAsync();
            if (editId.HasValue)
            {
                var editVehicle = await _vehicleService.GetByIdAsync(editId.Value);
                ViewBag.EditVehicle = editVehicle;
            }
            return View(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EvdmsVehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                // Trả về view với lỗi để biết lý do
                ViewBag.EditVehicle = null;
                return View("Index", await _vehicleService.GetAllAsync());
            }
            await _vehicleService.CreateAsync(vehicle);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EvdmsVehicle vehicle)
        {
            if (ModelState.IsValid)
                await _vehicleService.UpdateAsync(vehicle);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine("Delete action called with id: " + id);
            await _vehicleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

