using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, bool onlyActive = true)
    {
        var q = Set.Where(p => p.CategoryId == categoryId);
        if (onlyActive) q = q.Where(p => p.IsActive);
        return await q.OrderBy(p => p.SortOrder).ThenBy(p => p.Name).ToListAsync();
    }

    public Task<Product?> GetWithDetailsAsync(int id) =>
        Set.Include(p => p.Category)
           .Include(p => p.TagMappings).ThenInclude(m => m.ProductTag)
           .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync() =>
        await Set.Include(p => p.Category)
                 .OrderBy(p => p.Category!.SortOrder)
                 .ThenBy(p => p.SortOrder)
                 .ToListAsync();

    public async Task<int> GetMaxSortOrderAsync(int categoryId) =>
        await Set.AnyAsync(p => p.CategoryId == categoryId)
            ? await Set.Where(p => p.CategoryId == categoryId).MaxAsync(p => p.SortOrder)
            : 0;
}
