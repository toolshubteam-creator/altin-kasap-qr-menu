using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class PriceHistoryRepository : GenericRepository<PriceHistory>, IPriceHistoryRepository
{
    public PriceHistoryRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<PriceHistory>> GetForProductAsync(int productId) =>
        await Set.Where(h => h.ProductId == productId)
                 .OrderByDescending(h => h.ChangedAt)
                 .ToListAsync();
}
