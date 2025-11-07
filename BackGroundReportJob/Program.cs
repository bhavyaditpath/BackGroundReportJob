using BackGroundReportJob.Infrastructure.Context;
using BackGroundReportJob.Infrastructure.Repositories;
using BackGroundReportJob.Infrastructure.Repositories.Interface;
using BackGroundReportJob.Services;
using BackGroundReportJob.Services.Interface;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

// Optional: Application Insights
// builder.Services.AddApplicationInsightsTelemetryWorkerService()
//                 .ConfigureFunctionsApplicationInsights();

builder.Services.AddLogging();

// Build & Run Function App
builder.ConfigureFunctionsWebApplication();
builder.Build().Run();
