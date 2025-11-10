using BackGroundReportJob.Models;

namespace BackGroundReportJob.Infrastructure.Repositories.Interface
{
    public interface IReportService
    {
        Task<IEnumerable<ReportConfigurationEntity>> GetAllReportConfigurationsAsync();
        Task<bool> UpdateReportStatusAsync(Guid id, bool isEnabled);
        Task GenerateReportsAsync();
    }
}
