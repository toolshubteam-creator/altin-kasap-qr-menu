using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class QrCodeRepository : GenericRepository<QrCode>, IQrCodeRepository
{
    public QrCodeRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<QrCode>> GetAllWithScanCountAsync() =>
        await Set.Include(q => q.ScanLogs)
                 .OrderByDescending(q => q.CreatedAt)
                 .ToListAsync();
}
