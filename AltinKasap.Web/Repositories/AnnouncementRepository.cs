using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class AnnouncementRepository : GenericRepository<Announcement>, IAnnouncementRepository
{
    public AnnouncementRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Announcement>> GetActiveAsync(DateTime now) =>
        await Set.Where(a => a.IsActive && a.StartDate <= now && a.EndDate >= now)
                 .OrderByDescending(a => a.CreatedAt)
                 .ToListAsync();
}
