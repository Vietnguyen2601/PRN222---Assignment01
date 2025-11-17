namespace EleVehicleDealer.Domain.DTOs.Stations
{
    public class StationSummaryDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
