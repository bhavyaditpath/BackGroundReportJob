using BackGroundReportJob.Helpers;
using BackGroundReportJob.Infrastructure.Context;
using BackGroundReportJob.Infrastructure.Repositories;
using BackGroundReportJob.Infrastructure.Repositories.Interface;
using BackGroundReportJob.Services;
using BackGroundReportJob.Services.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

// Load configuration from appsettings.json or local.settings.json
var configuration = builder.Configuration;

// Register DbContext with SQL Server connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration["SqlConnection"]));

// Register Repositories & Services
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IReportConfigurationRepository, ReportConfigurationRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Optional: Application Insights
// builder.Services.AddApplicationInsightsTelemetryWorkerService()
//                 .ConfigureFunctionsApplicationInsights();

// Configure JSON serialization (optional but good for Angular)
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.WriteIndented = true;
});

// Add Logging
builder.Services.AddLogging();

// Add CORS Middleware (for Angular UI)
//builder.UseMiddleware<CorsMiddleware>();

// Build & Run Function App
builder.ConfigureFunctionsWebApplication();
builder.Build().Run();

public class CorsMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        // Run the function first
        await next(context);

        // Then get the request and response (if it’s an HTTP-triggered function)
        var req = await context.GetHttpRequestDataAsync();
        if (req == null)
            return; // not an HTTP function (e.g., timer)

        var res = context.GetHttpResponseData();
        if (res != null)
        {
            // Add CORS headers only if not already present
            if (!res.Headers.Contains("Access-Control-Allow-Origin"))
                res.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
            if (!res.Headers.Contains("Access-Control-Allow-Methods"))
                res.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            if (!res.Headers.Contains("Access-Control-Allow-Headers"))
                res.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        }
    }
}