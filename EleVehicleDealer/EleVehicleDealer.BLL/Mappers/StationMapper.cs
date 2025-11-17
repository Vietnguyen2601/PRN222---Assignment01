using System.Collections.Generic;
using System.Linq;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.Domain.DTOs.Stations;

namespace EleVehicleDealer.BLL.Mappers
{
    public static class StationMapper
    {
        public static StationSummaryDto? ToStationSummaryDto(this Station? station)
        {
            if (station == null) return null;

            return new StationSummaryDto
            {
                StationId = station.StationId,
                StationName = station.StationName,
                Location = station.Location,
                ContactNumber = station.ContactNumber,
                IsActive = station.IsActive
            };
        }

        public static IEnumerable<StationSummaryDto> ToStationSummaryDtos(this IEnumerable<Station>? stations) =>
            stations?.Select(s => s.ToStationSummaryDto()!).Where(dto => dto != null)! ?? Enumerable.Empty<StationSummaryDto>();
    }
}
