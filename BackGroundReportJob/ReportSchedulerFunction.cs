using BackGroundReportJob.Helpers;
using BackGroundReportJob.Infrastructure.Repositories.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BackGroundReportJob
{
    public class ReportSchedulerFunction
    {
        private readonly ILogger<ReportSchedulerFunction> _logger;
        private readonly IReportService _reportService;
        private readonly IReportConfigurationRepository _reportConfigRepo;

        public ReportSchedulerFunction(
            ILogger<ReportSchedulerFunction> logger,
            IReportService reportService,
            IReportConfigurationRepository reportConfigRepo)
        {
            _logger = logger;
            _reportService = reportService;
            _reportConfigRepo = reportConfigRepo;
        }

        [Function("ReportSchedulerFunction")]
        public async Task RunAsync([TimerTrigger("0 */1 * * * *")] TimerInfo timerInfo)
        {
            var activeReports = await _reportConfigRepo.GetActiveReportsAsync();

            foreach (var report in activeReports)
            {
                if (ReportScheduleHelper.ShouldGenerateReport(report))
                {
                    await _reportService.GenerateReportsAsync();
                }

            }
            _logger.LogInformation("Next scheduled run: {next}", timerInfo?.ScheduleStatus?.Next);
        }
    }
}
