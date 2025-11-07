using BackGroundReportJob.Models;

namespace BackGroundReportJob.Infrastructure.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync();
        Task<EmployeeEntity?> GetEmployeeByIdAsync(Guid id);
    }
}
