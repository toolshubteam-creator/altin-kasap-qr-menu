namespace AltinKasap.Web.ViewModels;

public class DashboardViewModel
{
    public int TotalCategories { get; set; }
    public int TotalProducts { get; set; }
    public long TodaysScanCount { get; set; }
    public ReportViewModel Last7DaysReport { get; set; } = new();
}
