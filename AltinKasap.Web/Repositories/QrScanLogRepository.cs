using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using AltinKasap.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class QrScanLogRepository : IQrScanLogRepository
{
    private readonly AppDbContext _db;

    public QrScanLogRepository(AppDbContext db) => _db = db;

    public IQueryable<QrScanLog> Query() => _db.QrScanLogs.AsQueryable();

    public Task<long> CountAsync(DateTime from, DateTime to) =>
        _db.QrScanLogs.LongCountAsync(l => l.ScannedAt >= from && l.ScannedAt < to);

    public async Task<List<DailyScanCount>> GetDailyCountsAsync(DateTime from, DateTime to)
    {
        var rows = await _db.QrScanLogs
            .Where(l => l.ScannedAt >= from && l.ScannedAt < to)
            .GroupBy(l => l.ScannedAt.Date)
            .Select(g => new DailyScanCount { Date = g.Key, Count = g.LongCount() })
            .OrderBy(x => x.Date)
            .ToListAsync();
        return rows;
    }

    public async Task LogScanAsync(int? qrCodeId, string ipAddress, string userAgent, string? referrer)
    {
        _db.QrScanLogs.Add(new QrScanLog
        {
            QrCodeId = qrCodeId,
            ScannedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Referrer = referrer
        });
        await _db.SaveChangesAsync();
    }
}
