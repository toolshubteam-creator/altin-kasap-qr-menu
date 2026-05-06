using AltinKasap.Web.Repositories;
using QRCoder;

namespace AltinKasap.Web.Services;

public class QrService : IQrService
{
    private readonly IQrScanLogRepository _scanLogRepo;

    public QrService(IQrScanLogRepository scanLogRepo) => _scanLogRepo = scanLogRepo;

    public byte[] GeneratePng(string content, string fgHex = "#000000", string bgHex = "#FFFFFF", int pixelsPerModule = 20)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var pngQr = new PngByteQRCode(data);
        return pngQr.GetGraphic(pixelsPerModule, HexToRgb(fgHex), HexToRgb(bgHex));
    }

    public string GenerateSvg(string content, string fgHex = "#000000", string bgHex = "#FFFFFF")
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var svgQr = new SvgQRCode(data);
        return svgQr.GetGraphic(20, fgHex, bgHex);
    }

    public async Task LogScanAsync(int? qrCodeId, HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        if (ip.Length > 45) ip = ip[..45];

        var ua = context.Request.Headers.UserAgent.ToString();
        if (ua.Length > 500) ua = ua[..500];

        var referrer = context.Request.Headers.Referer.ToString();
        if (string.IsNullOrEmpty(referrer)) referrer = null;
        else if (referrer.Length > 500) referrer = referrer[..500];

        await _scanLogRepo.LogScanAsync(qrCodeId, ip, ua, referrer);
    }

    private static byte[] HexToRgb(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length != 6)
            throw new ArgumentException($"Geçersiz hex renk: {hex}", nameof(hex));
        return new[]
        {
            Convert.ToByte(hex.Substring(0, 2), 16),
            Convert.ToByte(hex.Substring(2, 2), 16),
            Convert.ToByte(hex.Substring(4, 2), 16)
        };
    }
}
