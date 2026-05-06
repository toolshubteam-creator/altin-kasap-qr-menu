using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.Models;

public class QrScanLog
{
    public long Id { get; set; }

    public int? QrCodeId { get; set; }

    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    [MaxLength(500)]
    public string UserAgent { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Referrer { get; set; }

    public virtual QrCode? QrCode { get; set; }
}
