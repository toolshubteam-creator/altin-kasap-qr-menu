using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using AltinKasap.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly IProductRepository _productRepo;
    private readonly IQrScanLogRepository _scanRepo;
    private readonly IQrCodeRepository _qrCodeRepo;
    private readonly IReportService _reportService;

    public AdminController(
        ICategoryRepository categoryRepo,
        IProductRepository productRepo,
        IQrScanLogRepository scanRepo,
        IQrCodeRepository qrCodeRepo,
        IReportService reportService)
    {
        _categoryRepo = categoryRepo;
        _productRepo = productRepo;
        _scanRepo = scanRepo;
        _qrCodeRepo = qrCodeRepo;
        _reportService = reportService;
    }

    [HttpGet("")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-6);

        var qrCodes = await _qrCodeRepo.GetAllWithScanCountAsync();

        var vm = new DashboardViewModel
        {
            TotalCategories = (await _categoryRepo.GetAllAsync()).Count(),
            TotalProducts = (await _productRepo.GetAllAsync()).Count(),
            TodaysScanCount = await _scanRepo.CountAsync(today, today.AddDays(1)),
            Last7DaysReport = await _reportService.GetWeeklyAsync(weekStart),
            TopQrCodes = qrCodes
                .Select(q => (Name: q.Name, ScanCount: (long)q.ScanLogs.Count))
                .OrderByDescending(x => x.ScanCount)
                .Take(5)
                .ToList()
        };

        return View(vm);
    }
}
