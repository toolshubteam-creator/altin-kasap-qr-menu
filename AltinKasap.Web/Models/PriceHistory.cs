using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltinKasap.Web.Models;

public class PriceHistory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal OldPrice { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal NewPrice { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? ChangedBy { get; set; }

    public virtual Product? Product { get; set; }
}
