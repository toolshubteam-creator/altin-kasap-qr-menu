using AltinKasap.Web.Models;
using AltinKasap.Web.ViewModels;

namespace AltinKasap.Web.Repositories;

public interface IQrScanLogRepository
{
    IQueryable<QrScanLog> Query();
    Task<long> CountAsync(DateTime from, DateTime to);
    Task<List<DailyScanCount>> GetDailyCountsAsync(DateTime from, DateTime to);
    Task LogScanAsync(int? qrCodeId, string ipAddress, string userAgent, string? referrer);
}
