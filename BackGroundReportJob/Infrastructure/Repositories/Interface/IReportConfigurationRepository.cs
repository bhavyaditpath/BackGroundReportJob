using BackGroundReportJob.Models;

namespace BackGroundReportJob.Infrastructure.Repositories.Interface
{
    public interface IReportConfigurationRepository
    {
        Task<IEnumerable<ReportConfigurationEntity>> GetActiveReportsAsync();
    }
}
