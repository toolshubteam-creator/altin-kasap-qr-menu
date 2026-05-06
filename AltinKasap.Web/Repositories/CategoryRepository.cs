using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Category>> GetActiveOrderedAsync() =>
        await Set.Where(c => c.IsActive)
                 .OrderBy(c => c.SortOrder)
                 .ThenBy(c => c.Name)
                 .ToListAsync();

    public Task<Category?> GetBySlugAsync(string slug) =>
        Set.FirstOrDefaultAsync(c => c.Slug == slug);

    public async Task<int> GetMaxSortOrderAsync() =>
        await Set.AnyAsync() ? await Set.MaxAsync(c => c.SortOrder) : 0;
}
