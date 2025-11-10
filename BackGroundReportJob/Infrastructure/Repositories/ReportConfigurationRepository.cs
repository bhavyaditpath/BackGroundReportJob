using BackGroundReportJob.Infrastructure.Context;
using BackGroundReportJob.Infrastructure.Repositories.Interface;
using BackGroundReportJob.Models;
using Microsoft.EntityFrameworkCore;

namespace BackGroundReportJob.Infrastructure.Repositories
{
    public class ReportConfigurationRepository : IReportConfigurationRepository
    {
        private readonly ApplicationDbContext _context;
        
        public ReportConfigurationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReportConfigurationEntity>> GetActiveReportsAsync()
        {
            var now = DateTime.UtcNow;
            
            return await _context.ReportConfigurations
                .Where(r => r.IsEnabled 
                    && !r.IsDeleted
                    && ((r.Frequency == ReportFrequency.Daily)
                        || (r.Frequency == ReportFrequency.Weekly && now.DayOfWeek == DayOfWeek.Monday && now.Hour == 0 && now.Minute == 0)
                        || (r.Frequency == ReportFrequency.Monthly && now.Day == 1 && now.Hour == 0 && now.Minute == 0)
                        || (r.Frequency == ReportFrequency.Yearly && now.Month == 1 && now.Day == 1 && now.Hour == 0 && now.Minute == 0)))
                .ToListAsync();
        }

        public async Task<IEnumerable<ReportConfigurationEntity>> GetAllAsync()
        {
            return await _context.ReportConfigurations.AsNoTracking().ToListAsync();
        }

        public async Task<ReportConfigurationEntity?> GetByIdAsync(Guid id)
        {
            return await _context.ReportConfigurations.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task UpdateAsync(ReportConfigurationEntity entity)
        {
            _context.ReportConfigurations.Update(entity);
            await _context.SaveChangesAsync();
        }

    }
}
