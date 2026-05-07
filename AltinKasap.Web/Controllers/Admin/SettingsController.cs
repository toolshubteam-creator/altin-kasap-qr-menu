using AltinKasap.Web.Helpers;
using AltinKasap.Web.Models;
using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using AltinKasap.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/settings")]
public class SettingsController : Controller
{
    private readonly IRestaurantRepository _repo;
    private readonly IImageService _imageService;
    private readonly IMenuService _menuService;

    public SettingsController(
        IRestaurantRepository repo,
        IImageService imageService,
        IMenuService menuService)
    {
        _repo = repo;
        _imageService = imageService;
        _menuService = menuService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var entity = await _repo.GetSingleAsync();
        if (entity == null)
        {
            entity = new Restaurant
            {
                Name = "Altın Kasap",
                Slug = "altin-kasap",
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        return View(new RestaurantFormViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Address = entity.Address,
            Phone = entity.Phone,
            InstagramUrl = entity.InstagramUrl,
            FacebookUrl = entity.FacebookUrl,
            WebsiteUrl = entity.WebsiteUrl,
            PrimaryColor = entity.PrimaryColor ?? "#C8A032",
            ExistingLogoPath = entity.LogoPath,
            ExistingCoverPath = entity.CoverImagePath
        });
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(12 * 1024 * 1024)]
    public async Task<IActionResult> Index(RestaurantFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var entity = await _repo.GetByIdAsync(model.Id);
        if (entity == null) return NotFound();

        if (model.LogoFile != null && model.LogoFile.Length > 0)
        {
            try
            {
                var newPath = await _imageService.SaveResizedAsync(model.LogoFile, "restaurant", maxWidth: 400, maxHeight: 400);
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

        if (model.CoverFile != null && model.CoverFile.Length > 0)
        {
            try
            {
                var newPath = await _imageService.SaveResizedAsync(model.CoverFile, "restaurant", maxWidth: 1600, maxHeight: 600);
                _imageService.DeleteIfExists(entity.CoverImagePath);
                entity.CoverImagePath = newPath;
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.CoverFile), ex.Message);
                return View(model);
            }
        }
        else if (model.RemoveCover)
        {
            _imageService.DeleteIfExists(entity.CoverImagePath);
            entity.CoverImagePath = null;
        }

        entity.Name = model.Name;
        entity.Slug = SlugHelper.Generate(model.Name);
        entity.Address = model.Address;
        entity.Phone = model.Phone;
        entity.InstagramUrl = model.InstagramUrl;
        entity.FacebookUrl = model.FacebookUrl;
        entity.WebsiteUrl = model.WebsiteUrl;
        entity.PrimaryColor = model.PrimaryColor;
        entity.UpdatedAt = DateTime.UtcNow;

        _repo.Update(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Restoran ayarları güncellendi.";
        return RedirectToAction(nameof(Index));
    }
}
