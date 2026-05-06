using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.Models;

public class DailySpecial
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public DateTime SpecialDate { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Product? Product { get; set; }
}
