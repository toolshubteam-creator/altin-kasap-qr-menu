using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.Models;

public class QrCode
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ForegroundColor { get; set; } = "#000000";

    [MaxLength(20)]
    public string BackgroundColor { get; set; } = "#FFFFFF";

    [MaxLength(500)]
    public string? LogoPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<QrScanLog> ScanLogs { get; set; } = new List<QrScanLog>();
}
