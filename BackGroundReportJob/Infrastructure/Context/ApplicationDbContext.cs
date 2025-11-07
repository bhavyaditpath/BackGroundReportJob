using Microsoft.EntityFrameworkCore;
using BackGroundReportJob.Models;

namespace BackGroundReportJob.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<ReportConfigurationEntity> ReportConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary keys
            modelBuilder.Entity<EmployeeEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<ReportConfigurationEntity>().HasKey(e => e.Id);

            // Configure soft delete query filters
            modelBuilder.Entity<EmployeeEntity>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ReportConfigurationEntity>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}