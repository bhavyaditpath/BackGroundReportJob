using BackGroundReportJob.Infrastructure.Context;
using BackGroundReportJob.Infrastructure.Repositories.Interface;
using BackGroundReportJob.Models;
using Microsoft.EntityFrameworkCore;

namespace BackGroundReportJob.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<EmployeeEntity?> GetEmployeeByIdAsync(Guid id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}
