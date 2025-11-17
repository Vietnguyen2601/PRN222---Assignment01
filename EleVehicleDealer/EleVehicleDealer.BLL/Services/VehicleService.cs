using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.Domain.DTOs.Vehicles;
using EleVehicleDealer.BLL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.BLL.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly EvdmsDatabaseContext _context;

        public VehicleService(IVehicleRepository vehicleRepository, EvdmsDatabaseContext context)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<VehicleDetailDto?> CreateAsync(VehicleCreateDto vehicle)
        {
            var entity = MapToVehicle(vehicle);
            var created = await _vehicleRepository.CreateVehicleAsync(entity);
            return created.ToVehicleDetailDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _vehicleRepository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<VehicleCatalogDto>> GetAllVehicleAsync()
        {
            var vehicles = await _vehicleRepository.GetAllVehicleAsync();
            return vehicles.ToVehicleCatalogDtos();
        }

        public async Task<VehicleDetailDto?> GetByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.StationCars)
                    .ThenInclude(sc => sc.Station)
                .FirstOrDefaultAsync(v => v.VehicleId == id);
            return vehicle.ToVehicleDetailDto();
        }

        public async Task<IEnumerable<VehicleCatalogDto>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var vehicles = await _vehicleRepository.GetVehiclesByPriceRangeAsync(minPrice, maxPrice);
            return vehicles.ToVehicleCatalogDtos();
        }

        public async Task<IEnumerable<VehicleCatalogDto>> GetVehiclesByTypeAsync(string type)
        {
            var vehicles = await _vehicleRepository.GetVehiclesByTypeAsync(type);
            return vehicles.ToVehicleCatalogDtos();
        }

        public async Task<IEnumerable<VehicleCatalogDto>> SearchVehiclesAsync(string searchTerm)
        {
            var vehicles = await _vehicleRepository.SearchVehiclesAsync(searchTerm);
            return vehicles.ToVehicleCatalogDtos();
        }

        public async Task<VehicleDetailDto?> UpdateAsync(VehicleUpdateDto vehicle)
        {
            if (vehicle == null)
                throw new System.ArgumentNullException(nameof(vehicle));

            var existingVehicle = await _context.Vehicles.FindAsync(vehicle.VehicleId);
            if (existingVehicle == null)
            {
                return null;
            }

            existingVehicle.Model = vehicle.Model;
            existingVehicle.Type = vehicle.Type;
            existingVehicle.Color = vehicle.Color;
            existingVehicle.Price = vehicle.Price;
            existingVehicle.Manufacturer = vehicle.Manufacturer;
            existingVehicle.BatteryCapacity = vehicle.BatteryCapacity;
            existingVehicle.Range = vehicle.Range;
            existingVehicle.IsActive = vehicle.IsActive;
            existingVehicle.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var reload = await _context.Vehicles
                .Include(v => v.StationCars)
                    .ThenInclude(sc => sc.Station)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicle.VehicleId);

            return reload.ToVehicleDetailDto();
        }

        public async Task<IEnumerable<VehicleCatalogDto>> GetVehiclesByStationAsync(int stationId)
        {
            var vehicles = await _vehicleRepository.GetVehiclesByStationAsync(stationId);
            return vehicles.ToVehicleCatalogDtos();
        }

        public async Task<IEnumerable<decimal>> GetVehiclePriceByModelAsync(string model)
        {
            return await _vehicleRepository.GetVehiclePriceByModelAsync(model);
        }

        private static Vehicle MapToVehicle(VehicleCreateDto dto)
        {
            return new Vehicle
            {
                Model = dto.Model,
                Type = dto.Type,
                Color = dto.Color,
                Price = dto.Price,
                Manufacturer = dto.Manufacturer,
                BatteryCapacity = dto.BatteryCapacity,
                Range = dto.Range,
                CreatedAt = System.DateTime.Now,
                IsActive = true
            };
        }
    }
}
