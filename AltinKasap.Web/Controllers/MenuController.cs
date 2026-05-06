using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly ILogger<MenuController> _logger;

    public MenuController(IMenuService menuService, ILogger<MenuController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        if (ip.Length > 45) ip = ip[..45];

        var ua = Request.Headers["User-Agent"].ToString();
        if (ua.Length > 500) ua = ua[..500];

        var referrer = Request.Headers["Referer"].ToString();
        string? referrerOrNull = string.IsNullOrEmpty(referrer) ? null : (referrer.Length > 500 ? referrer[..500] : referrer);

        var scopeFactory = HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();
        var loggerForBg = _logger;
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var scanRepo = scope.ServiceProvider.GetRequiredService<IQrScanLogRepository>();
                await scanRepo.LogScanAsync(null, ip, ua, referrerOrNull);
            }
            catch (Exception ex)
            {
                loggerForBg.LogWarning(ex, "Scan log failed (non-critical)");
            }
        });

        var model = await _menuService.GetPublicMenuAsync();
        return View(model);
    }
}
