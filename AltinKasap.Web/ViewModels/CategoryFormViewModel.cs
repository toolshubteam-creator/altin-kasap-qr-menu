using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class CategoryFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad gerekli.")]
    [MaxLength(200)]
    [Display(Name = "Ad")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [MaxLength(100)]
    [Display(Name = "Font Awesome İkon")]
    public string? IconClass { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
}
