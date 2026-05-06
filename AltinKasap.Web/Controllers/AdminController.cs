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
    private readonly IReportService _reportService;

    public AdminController(
        ICategoryRepository categoryRepo,
        IProductRepository productRepo,
        IQrScanLogRepository scanRepo,
        IReportService reportService)
    {
        _categoryRepo = categoryRepo;
        _productRepo = productRepo;
        _scanRepo = scanRepo;
        _reportService = reportService;
    }

    [HttpGet("")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-6);

        var vm = new DashboardViewModel
        {
            TotalCategories = (await _categoryRepo.GetAllAsync()).Count(),
            TotalProducts = (await _productRepo.GetAllAsync()).Count(),
            TodaysScanCount = await _scanRepo.CountAsync(today, today.AddDays(1)),
            Last7DaysReport = await _reportService.GetWeeklyAsync(weekStart)
        };

        return View(vm);
    }
}
