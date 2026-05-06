using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.Models;

public class ProductTag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? IconClass { get; set; }

    [MaxLength(20)]
    public string? ColorHex { get; set; }

    public virtual ICollection<ProductTagMapping> ProductMappings { get; set; } = new List<ProductTagMapping>();
}
