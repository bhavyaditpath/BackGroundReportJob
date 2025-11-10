using BackGroundReportJob.Helpers;
using BackGroundReportJob.Infrastructure.Repositories.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

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

        [Function("UpdateReportStatus")]
        public async Task<HttpResponseData> UpdateReportStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "reports/{id:guid}/status")] HttpRequestData req,
            Guid id)
        {
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();
            var payload = JsonSerializer.Deserialize<ReportStatusUpdateRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            var result = await _reportService.UpdateReportStatusAsync(id, payload.IsEnabled);

            var response = req.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.NotFound);
            await response.WriteStringAsync(result ? "Report status updated successfully." : "Report not found.");
            return response;
        }

        [Function("GetAllReports")]
        public async Task<HttpResponseData> GetAllReports(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reports")] HttpRequestData req)
        {
            var reports = await _reportService.GetAllReportConfigurationsAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(reports);

            return response;
        }

    }

    public class ReportStatusUpdateRequest
    {
        public bool IsEnabled { get; set; }
    }
}
