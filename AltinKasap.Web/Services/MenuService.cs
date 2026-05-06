using AltinKasap.Web.Models;
using AltinKasap.Web.Repositories;
using AltinKasap.Web.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace AltinKasap.Web.Services;

public class MenuService : IMenuService
{
    private const string CacheKey = "public_menu";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly IRestaurantRepository _restaurantRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IProductRepository _productRepo;
    private readonly IAnnouncementRepository _announcementRepo;
    private readonly IDailySpecialRepository _dailySpecialRepo;
    private readonly IMemoryCache _cache;

    public MenuService(
        IRestaurantRepository restaurantRepo,
        ICategoryRepository categoryRepo,
        IProductRepository productRepo,
        IAnnouncementRepository announcementRepo,
        IDailySpecialRepository dailySpecialRepo,
        IMemoryCache cache)
    {
        _restaurantRepo = restaurantRepo;
        _categoryRepo = categoryRepo;
        _productRepo = productRepo;
        _announcementRepo = announcementRepo;
        _dailySpecialRepo = dailySpecialRepo;
        _cache = cache;
    }

    public async Task<MenuViewModel> GetPublicMenuAsync()
    {
        if (_cache.TryGetValue(CacheKey, out MenuViewModel? cached) && cached != null)
            return cached;

        var restaurant = await _restaurantRepo.GetSingleAsync()
            ?? new Restaurant { Name = "Menü", Slug = "menu" };

        var categories = (await _categoryRepo.GetActiveOrderedAsync()).ToList();
        var categoryWithProducts = new List<CategoryWithProducts>();
        foreach (var cat in categories)
        {
            var products = (await _productRepo.GetByCategoryAsync(cat.Id, onlyActive: true)).ToList();
            categoryWithProducts.Add(new CategoryWithProducts { Category = cat, Products = products });
        }

        var now = DateTime.UtcNow;
        var announcements = (await _announcementRepo.GetActiveAsync(now)).ToList();
        var todaysSpecial = await _dailySpecialRepo.GetForDateAsync(now);

        var vm = new MenuViewModel
        {
            Restaurant = restaurant,
            Categories = categoryWithProducts,
            ActiveAnnouncements = announcements,
            TodaysSpecial = todaysSpecial
        };

        _cache.Set(CacheKey, vm, CacheTtl);
        return vm;
    }

    public void InvalidateCache() => _cache.Remove(CacheKey);
}
