using AltinKasap.Web.Repositories;
using AltinKasap.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Services;

public class ReportService : IReportService
{
    private readonly IQrScanLogRepository _scanLogRepo;
    private static readonly string[] TurkishMonths = new[]
    {
        "Oca", "Şub", "Mar", "Nis", "May", "Haz",
        "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"
    };

    public ReportService(IQrScanLogRepository scanLogRepo) => _scanLogRepo = scanLogRepo;

    public async Task<ReportViewModel> GetDailyAsync(DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        var hourly = await _scanLogRepo.Query()
            .Where(l => l.ScannedAt >= dayStart && l.ScannedAt < dayEnd)
            .GroupBy(l => l.ScannedAt.Hour)
            .Select(g => new { Hour = g.Key, Count = g.LongCount() })
            .ToListAsync();

        var lookup = hourly.ToDictionary(x => x.Hour, x => x.Count);
        var labels = Enumerable.Range(0, 24).Select(h => h.ToString("00") + ":00").ToArray();
        var data = Enumerable.Range(0, 24).Select(h => lookup.TryGetValue(h, out var c) ? c : 0L).ToArray();

        return new ReportViewModel
        {
            Labels = labels,
            Data = data,
            Total = data.Sum(),
            Title = $"Günlük Rapor — {dayStart:dd.MM.yyyy}"
        };
    }

    public async Task<ReportViewModel> GetWeeklyAsync(DateTime weekStart)
    {
        var start = weekStart.Date;
        var end = start.AddDays(7);
        var counts = await _scanLogRepo.GetDailyCountsAsync(start, end);
        var lookup = counts.ToDictionary(x => x.Date.Date, x => x.Count);

        var labels = Enumerable.Range(0, 7).Select(i => start.AddDays(i).ToString("dd.MM")).ToArray();
        var data = Enumerable.Range(0, 7).Select(i => lookup.TryGetValue(start.AddDays(i), out var c) ? c : 0L).ToArray();

        return new ReportViewModel
        {
            Labels = labels,
            Data = data,
            Total = data.Sum(),
            Title = $"Haftalık Rapor — {start:dd.MM.yyyy} - {end.AddDays(-1):dd.MM.yyyy}"
        };
    }

    public async Task<ReportViewModel> GetMonthlyAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var counts = await _scanLogRepo.GetDailyCountsAsync(start, end);
        var lookup = counts.ToDictionary(x => x.Date.Date, x => x.Count);

        var labels = Enumerable.Range(1, daysInMonth).Select(d => d.ToString("00")).ToArray();
        var data = Enumerable.Range(1, daysInMonth)
            .Select(d => lookup.TryGetValue(new DateTime(year, month, d), out var c) ? c : 0L)
            .ToArray();

        return new ReportViewModel
        {
            Labels = labels,
            Data = data,
            Total = data.Sum(),
            Title = $"Aylık Rapor — {start:MMMM yyyy}"
        };
    }

    public async Task<ReportViewModel> GetYearlyAsync(int year)
    {
        var start = new DateTime(year, 1, 1);
        var end = start.AddYears(1);

        var monthly = await _scanLogRepo.Query()
            .Where(l => l.ScannedAt >= start && l.ScannedAt < end)
            .GroupBy(l => l.ScannedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.LongCount() })
            .ToListAsync();

        var lookup = monthly.ToDictionary(x => x.Month, x => x.Count);
        var labels = TurkishMonths;
        var data = Enumerable.Range(1, 12).Select(m => lookup.TryGetValue(m, out var c) ? c : 0L).ToArray();

        return new ReportViewModel
        {
            Labels = labels,
            Data = data,
            Total = data.Sum(),
            Title = $"Yıllık Rapor — {year}"
        };
    }
}
