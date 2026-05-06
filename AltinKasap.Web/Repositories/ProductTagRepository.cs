using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class ProductTagRepository : GenericRepository<ProductTag>, IProductTagRepository
{
    public ProductTagRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<ProductTag>> GetAllOrderedAsync() =>
        await Set.OrderBy(t => t.Name).ToListAsync();
}
