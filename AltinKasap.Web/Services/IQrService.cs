namespace AltinKasap.Web.Services;

public interface IQrService
{
    byte[] GeneratePng(string content, string fgHex = "#000000", string bgHex = "#FFFFFF", int pixelsPerModule = 20);
    string GenerateSvg(string content, string fgHex = "#000000", string bgHex = "#FFFFFF");
    Task LogScanAsync(int? qrCodeId, HttpContext context);
}
