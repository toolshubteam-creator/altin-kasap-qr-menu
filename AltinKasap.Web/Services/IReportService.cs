using AltinKasap.Web.ViewModels;

namespace AltinKasap.Web.Services;

public interface IReportService
{
    Task<ReportViewModel> GetDailyAsync(DateTime date);
    Task<ReportViewModel> GetWeeklyAsync(DateTime weekStart);
    Task<ReportViewModel> GetMonthlyAsync(int year, int month);
    Task<ReportViewModel> GetYearlyAsync(int year);
}
