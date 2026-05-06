using AltinKasap.Web.Helpers;
using AltinKasap.Web.Models;
using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using AltinKasap.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/categories")]
public class CategoriesController : Controller
{
    private readonly ICategoryRepository _repo;
    private readonly IProductRepository _productRepo;
    private readonly IMenuService _menuService;

    public CategoriesController(
        ICategoryRepository repo,
        IProductRepository productRepo,
        IMenuService menuService)
    {
        _repo = repo;
        _productRepo = productRepo;
        _menuService = menuService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var list = (await _repo.GetAllAsync()).OrderBy(c => c.SortOrder).ToList();
        return View(list);
    }

    [HttpGet("create")]
    public IActionResult Create() => View(new CategoryFormViewModel());

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var maxOrder = await _repo.GetMaxSortOrderAsync();
        var entity = new Category
        {
            Name = model.Name,
            Slug = SlugHelper.Generate(model.Name),
            Description = model.Description,
            IconClass = model.IconClass,
            IsActive = model.IsActive,
            SortOrder = maxOrder + 1,
            CreatedAt = DateTime.UtcNow
        };
        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Kategori eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return View(new CategoryFormViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IconClass = entity.IconClass,
            IsActive = entity.IsActive
        });
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        entity.Name = model.Name;
        entity.Slug = SlugHelper.Generate(model.Name);
        entity.Description = model.Description;
        entity.IconClass = model.IconClass;
        entity.IsActive = model.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        _repo.Update(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Kategori güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var hasProducts = (await _productRepo.GetByCategoryAsync(id, onlyActive: false)).Any();
        if (hasProducts)
        {
            TempData["Error"] = "Bu kategoride ürünler var, önce ürünleri silin veya başka kategoriye taşıyın.";
            return RedirectToAction(nameof(Index));
        }

        _repo.Remove(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Kategori silindi.";
        return RedirectToAction(nameof(Index));
    }
}
