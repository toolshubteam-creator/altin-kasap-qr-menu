using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltinKasap.Web.Models;

public class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "TEXT")]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Unit { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsSoldOut { get; set; } = false;
    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<ProductTagMapping> TagMappings { get; set; } = new List<ProductTagMapping>();
    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
}
