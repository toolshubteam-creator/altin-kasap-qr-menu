using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using AltinKasap.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/products")]
public class ProductsController : Controller
{
    private readonly IProductRepository _repo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IImageService _imageService;
    private readonly IMenuService _menuService;
    private readonly IPriceHistoryRepository _priceHistoryRepo;
    private readonly IProductTagRepository _tagRepo;
    private readonly AppDbContext _db;

    public ProductsController(
        IProductRepository repo,
        ICategoryRepository categoryRepo,
        IImageService imageService,
        IMenuService menuService,
        IPriceHistoryRepository priceHistoryRepo,
        IProductTagRepository tagRepo,
        AppDbContext db)
    {
        _repo = repo;
        _categoryRepo = categoryRepo;
        _imageService = imageService;
        _menuService = menuService;
        _priceHistoryRepo = priceHistoryRepo;
        _tagRepo = tagRepo;
        _db = db;
    }

    private async Task PopulateCategoriesAsync()
    {
        ViewBag.Categories = (await _categoryRepo.GetAllAsync())
            .OrderBy(c => c.SortOrder)
            .ToList();
    }

    private async Task PopulateTagsAsync()
    {
        ViewBag.AllTags = (await _tagRepo.GetAllOrderedAsync()).ToList();
    }

    private async Task SyncTagMappingsAsync(int productId, List<int> tagIds)
    {
        var current = await _db.ProductTagMappings
            .Where(m => m.ProductId == productId)
            .ToListAsync();

        var newSet = (tagIds ?? new List<int>()).Distinct().ToHashSet();
        var currentSet = current.Select(m => m.ProductTagId).ToHashSet();

        foreach (var m in current.Where(m => !newSet.Contains(m.ProductTagId)))
            _db.ProductTagMappings.Remove(m);

        foreach (var newId in newSet.Where(id => !currentSet.Contains(id)))
            _db.ProductTagMappings.Add(new ProductTagMapping
            {
                ProductId = productId,
                ProductTagId = newId
            });
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int? categoryId = null)
    {
        var products = (await _repo.GetAllWithCategoryAsync()).ToList();
        if (categoryId.HasValue)
            products = products.Where(p => p.CategoryId == categoryId.Value).ToList();

        await PopulateCategoriesAsync();
        ViewBag.SelectedCategoryId = categoryId;
        return View(products);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync();
        return View(new ProductFormViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync();
            await PopulateTagsAsync();
            return View(model);
        }

        string? imagePath = null;
        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            try
            {
                imagePath = await _imageService.SaveResizedAsync(model.ImageFile, "products");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
                await PopulateCategoriesAsync();
                await PopulateTagsAsync();
                return View(model);
            }
        }

        var maxOrder = await _repo.GetMaxSortOrderAsync(model.CategoryId);
        var entity = new Product
        {
            CategoryId = model.CategoryId,
            Name = model.Name,
            Description = model.Description,
            Unit = model.Unit,
            Price = model.Price,
            ImagePath = imagePath,
            IsActive = model.IsActive,
            IsSoldOut = model.IsSoldOut,
            SortOrder = maxOrder + 1,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Ürün eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _repo.GetWithDetailsAsync(id);
        if (entity == null) return NotFound();

        await PopulateCategoriesAsync();
        await PopulateTagsAsync();
        return View(new ProductFormViewModel
        {
            Id = entity.Id,
            CategoryId = entity.CategoryId,
            Name = entity.Name,
            Description = entity.Description,
            Unit = entity.Unit,
            Price = entity.Price,
            ExistingImagePath = entity.ImagePath,
            IsActive = entity.IsActive,
            IsSoldOut = entity.IsSoldOut,
            SelectedTagIds = entity.TagMappings.Select(m => m.ProductTagId).ToList()
        });
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync();
            await PopulateTagsAsync();
            return View(model);
        }

        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            try
            {
                var newPath = await _imageService.SaveResizedAsync(model.ImageFile, "products");
                _imageService.DeleteIfExists(entity.ImagePath);
                entity.ImagePath = newPath;
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
                await PopulateCategoriesAsync();
                await PopulateTagsAsync();
                return View(model);
            }
        }
        else if (model.RemoveImage)
        {
            _imageService.DeleteIfExists(entity.ImagePath);
            entity.ImagePath = null;
        }

        if (entity.Price != model.Price)
        {
            await _priceHistoryRepo.AddAsync(new PriceHistory
            {
                ProductId = entity.Id,
                OldPrice = entity.Price,
                NewPrice = model.Price,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = User.Identity?.Name
            });
            await _priceHistoryRepo.SaveChangesAsync();
        }

        entity.CategoryId = model.CategoryId;
        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Unit = model.Unit;
        entity.Price = model.Price;
        entity.IsActive = model.IsActive;
        entity.IsSoldOut = model.IsSoldOut;
        entity.UpdatedAt = DateTime.UtcNow;

        _repo.Update(entity);
        await SyncTagMappingsAsync(entity.Id, model.SelectedTagIds);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Ürün güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _imageService.DeleteIfExists(entity.ImagePath);
        _repo.Remove(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = "Ürün silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("toggle-soldout/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleSoldOut(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        entity.IsSoldOut = !entity.IsSoldOut;
        entity.UpdatedAt = DateTime.UtcNow;
        _repo.Update(entity);
        await _repo.SaveChangesAsync();
        _menuService.InvalidateCache();
        TempData["Success"] = entity.IsSoldOut ? "Ürün 'Tükendi' işaretlendi." : "Ürün stoğa alındı.";
        return RedirectToAction(nameof(Index));
    }
}
