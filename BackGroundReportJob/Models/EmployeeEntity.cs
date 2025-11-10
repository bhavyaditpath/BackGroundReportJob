using BackGroundReportJob.Enums;

namespace BackGroundReportJob.Models
{
    public class EmployeeEntity:BaseEntity
    {
        public string RollNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public EmployeeRole Role { get; set; }
        public string? Email { get; set; }
    }
}
