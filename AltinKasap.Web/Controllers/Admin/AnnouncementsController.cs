using AltinKasap.Web.Models;
using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using AltinKasap.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/announcements")]
public class AnnouncementsController : Controller
{
    private readonly IAnnouncementRepository _repo;
    private readonly IMenuService _menuService;

    public AnnouncementsController(IAnnouncementRepository repo, IMenuService menuService)
    {
        _repo = repo;
        _menuService = menuService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var list = (await _repo.GetAllAsync())
            .OrderByDescending(a => a.StartDate)
            .ToList();
        return View(list);
    }

    [HttpGet("create")]
    public IActionResult Create() => View(new AnnouncementFormViewModel());

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AnnouncementFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var entity = new Announcement
        {
            Title = model.Title,
            Message = model.Message ?? string.Empty,
            LinkUrl = model.LinkUrl,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            BackgroundColor = model.BackgroundColor,
            TextColor = model.TextColor,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Duyuru oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return View(new AnnouncementFormViewModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Message = entity.Message,
            LinkUrl = entity.LinkUrl,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            BackgroundColor = entity.BackgroundColor,
            TextColor = entity.TextColor,
            IsActive = entity.IsActive
        });
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AnnouncementFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        entity.Title = model.Title;
        entity.Message = model.Message ?? string.Empty;
        entity.LinkUrl = model.LinkUrl;
        entity.StartDate = model.StartDate;
        entity.EndDate = model.EndDate;
        entity.BackgroundColor = model.BackgroundColor;
        entity.TextColor = model.TextColor;
        entity.IsActive = model.IsActive;

        _repo.Update(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Duyuru güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _repo.Remove(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Duyuru silindi.";
        return RedirectToAction(nameof(Index));
    }
}
