using BackGroundReportJob.Models;

namespace BackGroundReportJob.Services.Interface
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync();
        Task<EmployeeEntity?> GetEmployeeByIdAsync(Guid id);
    }
}
