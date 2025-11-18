using EleVehicleDealer.Domain.DTOs.Dashboard;
using System;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
        Task<CustomDateRangeDto> GetRevenueByCustomDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<RevenueByPeriodDto>> GetRevenueByQuarterAsync(int year);
        Task<List<RevenueByPeriodDto>> GetRevenueByMonthAsync(int year);
        Task<List<StationRevenueDto>> GetRevenueByStationAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
