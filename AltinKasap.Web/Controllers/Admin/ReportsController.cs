using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/reports")]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;
    private readonly IQrCodeRepository _qrRepo;

    public ReportsController(IReportService reportService, IQrCodeRepository qrRepo)
    {
        _reportService = reportService;
        _qrRepo = qrRepo;
    }

    [HttpGet("")]
    public IActionResult Index() => RedirectToAction(nameof(Daily));

    [HttpGet("daily")]
    public async Task<IActionResult> Daily(DateTime? date = null)
    {
        var d = (date ?? DateTime.Today).Date;
        var report = await _reportService.GetDailyAsync(d);
        ViewBag.SelectedDate = d;
        return View(report);
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> Weekly(DateTime? weekStart = null)
    {
        var d = (weekStart ?? StartOfIsoWeek(DateTime.Today)).Date;
        var report = await _reportService.GetWeeklyAsync(d);
        ViewBag.WeekStart = d;
        return View(report);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> Monthly(int? year = null, int? month = null)
    {
        var y = year ?? DateTime.Today.Year;
        var m = month ?? DateTime.Today.Month;
        var report = await _reportService.GetMonthlyAsync(y, m);
        ViewBag.Year = y;
        ViewBag.Month = m;
        return View(report);
    }

    [HttpGet("yearly")]
    public async Task<IActionResult> Yearly(int? year = null)
    {
        var y = year ?? DateTime.Today.Year;
        var report = await _reportService.GetYearlyAsync(y);
        ViewBag.Year = y;
        return View(report);
    }

    [HttpGet("qr-codes")]
    public async Task<IActionResult> QrCodes()
    {
        var qrCodes = await _qrRepo.GetAllWithScanCountAsync();
        return View(qrCodes
            .OrderByDescending(q => q.ScanLogs?.Count ?? 0)
            .ToList());
    }

    private static DateTime StartOfIsoWeek(DateTime d)
    {
        var diff = (7 + (d.DayOfWeek - DayOfWeek.Monday)) % 7;
        return d.AddDays(-diff).Date;
    }
}
