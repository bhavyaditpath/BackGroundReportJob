using BackGroundReportJob.Models;

namespace BackGroundReportJob.Infrastructure.Repositories.Interface
{
    public interface IReportConfigurationRepository
    {
        Task<IEnumerable<ReportConfigurationEntity>> GetActiveReportsAsync(); Task<IEnumerable<ReportConfigurationEntity>> GetAllAsync();
        Task<ReportConfigurationEntity?> GetByIdAsync(Guid id);
        Task UpdateAsync(ReportConfigurationEntity entity);
    }
}
