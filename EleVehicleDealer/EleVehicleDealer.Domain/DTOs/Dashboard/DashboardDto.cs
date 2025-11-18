namespace EleVehicleDealer.Domain.DTOs.Dashboard
{
    public class DashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalVehicles { get; set; }
        public List<RevenueByPeriodDto> RevenueByQuarter { get; set; } = new List<RevenueByPeriodDto>();
        public List<RevenueByPeriodDto> RevenueByMonth { get; set; } = new List<RevenueByPeriodDto>();
        public List<StationRevenueDto> RevenueByStation { get; set; } = new List<StationRevenueDto>();
    }

    public class RevenueByPeriodDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class StationRevenueDto
    {
        public string StationName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public int VehiclesSold { get; set; }
    }

    public class CustomDateRangeDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public List<RevenueByPeriodDto> DailyRevenue { get; set; } = new List<RevenueByPeriodDto>();
    }
}
