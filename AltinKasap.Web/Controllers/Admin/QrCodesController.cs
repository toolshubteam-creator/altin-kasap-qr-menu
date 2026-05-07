using AltinKasap.Web.Helpers;
using AltinKasap.Web.Models;
using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using AltinKasap.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/qrcodes")]
public class QrCodesController : Controller
{
    private readonly IQrCodeRepository _repo;
    private readonly IQrService _qrService;
    private readonly IImageService _imageService;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;

    public QrCodesController(
        IQrCodeRepository repo,
        IQrService qrService,
        IImageService imageService,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment env)
    {
        _repo = repo;
        _qrService = qrService;
        _imageService = imageService;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _env = env;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var qrCodes = await _repo.GetAllWithScanCountAsync();
        return View(qrCodes);
    }

    [HttpGet("create")]
    public IActionResult Create() => View(new QrCodeFormViewModel
    {
        ForegroundColor = "#000000",
        BackgroundColor = "#FFFFFF"
    });

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Create(QrCodeFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        string? logoPath = null;
        if (model.LogoFile != null && model.LogoFile.Length > 0)
        {
            try
            {
                logoPath = await _imageService.SaveResizedAsync(model.LogoFile, "qr", maxWidth: 400, maxHeight: 400);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.LogoFile), ex.Message);
                return View(model);
            }
        }

        var entity = new QrCode
        {
            Name = model.Name,
            Slug = SlugHelper.Generate(model.Name),
            ForegroundColor = model.ForegroundColor,
            BackgroundColor = model.BackgroundColor,
            LogoPath = logoPath,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        TempData["Success"] = "QR kod oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return View(new QrCodeFormViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            ForegroundColor = entity.ForegroundColor,
            BackgroundColor = entity.BackgroundColor,
            ExistingLogoPath = entity.LogoPath
        });
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Edit(int id, QrCodeFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        if (model.LogoFile != null && model.LogoFile.Length > 0)
        {
            try
            {
                var newPath = await _imageService.SaveResizedAsync(model.LogoFile, "qr", maxWidth: 400, maxHeight: 400);
                _imageService.DeleteIfExists(entity.LogoPath);
                entity.LogoPath = newPath;
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.LogoFile), ex.Message);
                return View(model);
            }
        }
        else if (model.RemoveLogo)
        {
            _imageService.DeleteIfExists(entity.LogoPath);
            entity.LogoPath = null;
        }

        entity.Name = model.Name;
        entity.Slug = SlugHelper.Generate(model.Name);
        entity.ForegroundColor = model.ForegroundColor;
        entity.BackgroundColor = model.BackgroundColor;
        entity.UpdatedAt = DateTime.UtcNow;

        _repo.Update(entity);
        await _repo.SaveChangesAsync();
        TempData["Success"] = "QR kod güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _imageService.DeleteIfExists(entity.LogoPath);
        _repo.Remove(entity);
        await _repo.SaveChangesAsync();
        TempData["Success"] = "QR kod silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("download/{id:int}/png")]
    public async Task<IActionResult> DownloadPng(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var url = BuildMenuUrl(entity.Id);
        var bytes = _qrService.GeneratePngWithLogo(url, ResolveLogoAbsolutePath(entity.LogoPath),
            entity.ForegroundColor, entity.BackgroundColor, pixelsPerModule: 20);
        var slug = string.IsNullOrWhiteSpace(entity.Slug) ? entity.Id.ToString() : entity.Slug;
        return File(bytes, "image/png", $"qr-{slug}.png");
    }

    [HttpGet("download/{id:int}/svg")]
    public async Task<IActionResult> DownloadSvg(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var url = BuildMenuUrl(entity.Id);
        var svg = _qrService.GenerateSvg(url, entity.ForegroundColor, entity.BackgroundColor);
        var bytes = System.Text.Encoding.UTF8.GetBytes(svg);
        var slug = string.IsNullOrWhiteSpace(entity.Slug) ? entity.Id.ToString() : entity.Slug;
        return File(bytes, "image/svg+xml", $"qr-{slug}.svg");
    }

    [HttpGet("preview/{id:int}")]
    public async Task<IActionResult> Preview(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var url = BuildMenuUrl(entity.Id);
        var bytes = _qrService.GeneratePngWithLogo(url, ResolveLogoAbsolutePath(entity.LogoPath),
            entity.ForegroundColor, entity.BackgroundColor, pixelsPerModule: 12);
        return File(bytes, "image/png");
    }

    [HttpGet("print/{id:int}")]
    public async Task<IActionResult> Print(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        ViewBag.MenuUrl = BuildMenuUrl(entity.Id);
        ViewBag.PreviewUrl = Url.Action(nameof(Preview), new { id = entity.Id });
        return View(entity);
    }

    private string? ResolveLogoAbsolutePath(string? logoRelativePath)
    {
        if (string.IsNullOrWhiteSpace(logoRelativePath)) return null;
        var trimmed = logoRelativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(_env.WebRootPath, trimmed);
    }

    private string BuildMenuUrl(int qrId)
    {
        var configured = _config["AppSettings:MenuBaseUrl"];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured.TrimEnd('/') + $"/menu?qr={qrId}";
        }

        var req = _httpContextAccessor.HttpContext?.Request;
        var scheme = req?.Scheme ?? "http";
        var host = req?.Host.Value ?? "localhost:5000";
        return $"{scheme}://{host}/menu?qr={qrId}";
    }
}
