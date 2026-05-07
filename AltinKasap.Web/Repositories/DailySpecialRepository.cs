using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class DailySpecialRepository : GenericRepository<DailySpecial>, IDailySpecialRepository
{
    public DailySpecialRepository(AppDbContext db) : base(db) { }

    public Task<DailySpecial?> GetForDateAsync(DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);
        return Set.Include(d => d.Product)
                  .Where(d => d.SpecialDate >= dayStart && d.SpecialDate < dayEnd)
                  .OrderByDescending(d => d.CreatedAt)
                  .FirstOrDefaultAsync();
    }
}
