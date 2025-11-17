using System.Collections.Generic;
using System.Linq;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.Domain.DTOs.Vehicles;

namespace EleVehicleDealer.BLL.Mappers
{
    public static class VehicleMapper
    {
        public static VehicleCatalogDto? ToVehicleCatalogDto(this Vehicle? vehicle)
        {
            if (vehicle == null) return null;

            return new VehicleCatalogDto
            {
                VehicleId = vehicle.VehicleId,
                Model = vehicle.Model,
                Type = vehicle.Type,
                Color = vehicle.Color,
                Price = vehicle.Price,
                Manufacturer = vehicle.Manufacturer,
                IsActive = vehicle.IsActive
            };
        }

        public static VehicleDetailDto? ToVehicleDetailDto(this Vehicle? vehicle)
        {
            if (vehicle == null) return null;

            return new VehicleDetailDto
            {
                VehicleId = vehicle.VehicleId,
                Model = vehicle.Model,
                Type = vehicle.Type,
                Color = vehicle.Color,
                Price = vehicle.Price,
                Manufacturer = vehicle.Manufacturer,
                IsActive = vehicle.IsActive,
                BatteryCapacity = vehicle.BatteryCapacity,
                Range = vehicle.Range,
                Inventories = vehicle.StationCars.Select(sc => sc.ToStationInventoryDto()!).Where(dto => dto != null).ToList()
            };
        }

        public static StationInventoryDto? ToStationInventoryDto(this StationCar? stationCar)
        {
            if (stationCar == null) return null;

            return new StationInventoryDto
            {
                StationCarId = stationCar.StationCarId,
                StationId = stationCar.StationId,
                StationName = stationCar.Station?.StationName ?? string.Empty,
                Quantity = stationCar.Quantity,
                IsActive = stationCar.IsActive
            };
        }

        public static IEnumerable<VehicleCatalogDto> ToVehicleCatalogDtos(this IEnumerable<Vehicle>? vehicles) =>
            vehicles?.Select(v => v.ToVehicleCatalogDto()!).Where(dto => dto != null)! ?? Enumerable.Empty<VehicleCatalogDto>();
    }
}
