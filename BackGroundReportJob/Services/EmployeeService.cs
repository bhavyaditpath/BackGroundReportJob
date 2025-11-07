using BackGroundReportJob.Infrastructure.Repositories.Interface;
using BackGroundReportJob.Models;
using BackGroundReportJob.Services.Interface;

namespace BackGroundReportJob.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return employees;
        }

        public async Task<EmployeeEntity?> GetEmployeeByIdAsync(Guid id)
        {
            return await _employeeRepository.GetEmployeeByIdAsync(id);
        }
    }
}
