using AltinKasap.Web.Repositories;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AltinKasap.Web.Services;

public class QrService : IQrService
{
    private readonly IQrScanLogRepository _scanLogRepo;

    public QrService(IQrScanLogRepository scanLogRepo) => _scanLogRepo = scanLogRepo;

    public byte[] GeneratePng(string content, string fgHex = "#000000", string bgHex = "#FFFFFF", int pixelsPerModule = 20)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
        var pngQr = new PngByteQRCode(data);
        return pngQr.GetGraphic(pixelsPerModule, HexToRgb(fgHex), HexToRgb(bgHex));
    }

    public byte[] GeneratePngWithLogo(string content, string? logoAbsolutePath, string fgHex = "#000000", string bgHex = "#FFFFFF", int pixelsPerModule = 20)
    {
        var qrBytes = GeneratePng(content, fgHex, bgHex, pixelsPerModule);

        if (string.IsNullOrEmpty(logoAbsolutePath) || !File.Exists(logoAbsolutePath))
            return qrBytes;

        using var qrImg = Image.Load<Rgba32>(qrBytes);
        using var logoImg = Image.Load<Rgba32>(logoAbsolutePath);

        var logoSize = qrImg.Width / 5;
        logoImg.Mutate(x => x.Resize(logoSize, logoSize));

        var padding = logoSize / 10;
        var frameSize = logoSize + (padding * 2);
        using var frame = new Image<Rgba32>(frameSize, frameSize);
        frame.Mutate(x => x.BackgroundColor(Color.White));

        var frameX = (qrImg.Width - frameSize) / 2;
        var frameY = (qrImg.Height - frameSize) / 2;
        qrImg.Mutate(x => x.DrawImage(frame, new Point(frameX, frameY), 1f));

        var logoX = (qrImg.Width - logoSize) / 2;
        var logoY = (qrImg.Height - logoSize) / 2;
        qrImg.Mutate(x => x.DrawImage(logoImg, new Point(logoX, logoY), 1f));

        using var ms = new MemoryStream();
        qrImg.SaveAsPng(ms);
        return ms.ToArray();
    }

    public string GenerateSvg(string content, string fgHex = "#000000", string bgHex = "#FFFFFF")
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
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
