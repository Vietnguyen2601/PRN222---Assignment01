using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<IEnumerable<EvdmsVehicle>> GetAllAsync()
            => await _vehicleRepository.GetAllAsync();

        public async Task<EvdmsVehicle> GetByIdAsync(int id)
            => await _vehicleRepository.GetByIdAsync(id);

        public async Task<EvdmsVehicle> CreateAsync(EvdmsVehicle vehicle)
            => await _vehicleRepository.CreateVehicleAsync(vehicle);

        public async Task<EvdmsVehicle> UpdateAsync(EvdmsVehicle vehicle)
            => await _vehicleRepository.UpdateAsync(vehicle);

        public async Task<bool> DeleteAsync(int id)
        {
            await _vehicleRepository.DeleteByIdAsync(id);
            return true;
        }
    }
}
