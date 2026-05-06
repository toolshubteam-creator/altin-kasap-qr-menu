using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltinKasap.Web.Models;

public class Announcement
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Column(TypeName = "TEXT")]
    public string Message { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(20)]
    public string BackgroundColor { get; set; } = "#FFFFFF";

    [MaxLength(20)]
    public string TextColor { get; set; } = "#000000";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
