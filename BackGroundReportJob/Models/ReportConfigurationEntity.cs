namespace BackGroundReportJob.Models
{
    public class ReportConfigurationEntity : BaseEntity
    {
        public string ReportName { get; set; }
        public ReportFrequency Frequency { get; set; }
        public bool IsEnabled { get; set; }
    }
}
