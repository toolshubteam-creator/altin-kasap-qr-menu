using AltinKasap.Web.Models;
using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using AltinKasap.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/daily-specials")]
public class DailySpecialsController : Controller
{
    private readonly IDailySpecialRepository _repo;
    private readonly IProductRepository _productRepo;
    private readonly IMenuService _menuService;

    public DailySpecialsController(
        IDailySpecialRepository repo,
        IProductRepository productRepo,
        IMenuService menuService)
    {
        _repo = repo;
        _productRepo = productRepo;
        _menuService = menuService;
    }

    private async Task PopulateProductsAsync()
    {
        var products = (await _productRepo.GetAllWithCategoryAsync())
            .Where(p => p.IsActive)
            .ToList();

        ViewBag.GroupedProducts = products
            .GroupBy(p => p.Category!)
            .OrderBy(g => g.Key.SortOrder)
            .ToList();
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var list = await _repo.Query()
            .Include(d => d.Product)
            .OrderByDescending(d => d.SpecialDate)
            .ThenByDescending(d => d.CreatedAt)
            .ToListAsync();
        return View(list);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        await PopulateProductsAsync();
        return View(new DailySpecialFormViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DailySpecialFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateProductsAsync();
            return View(model);
        }

        var entity = new DailySpecial
        {
            ProductId = model.ProductId,
            SpecialDate = model.SpecialDate.Date,
            Note = model.Note,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Günün önerisi eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        await PopulateProductsAsync();
        return View(new DailySpecialFormViewModel
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            SpecialDate = entity.SpecialDate,
            Note = entity.Note
        });
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DailySpecialFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await PopulateProductsAsync();
            return View(model);
        }

        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        entity.ProductId = model.ProductId;
        entity.SpecialDate = model.SpecialDate.Date;
        entity.Note = model.Note;

        _repo.Update(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Günün önerisi güncellendi.";
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
        TempData["Success"] = "Günün önerisi silindi.";
        return RedirectToAction(nameof(Index));
    }
}
