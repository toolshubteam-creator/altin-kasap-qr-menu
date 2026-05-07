using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class QrCodeFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad gerekli.")]
    [MaxLength(200)]
    [Display(Name = "QR Adı")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex renk girin (#RRGGBB)")]
    [Display(Name = "Ön Plan Rengi")]
    public string ForegroundColor { get; set; } = "#000000";

    [Required]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex renk girin (#RRGGBB)")]
    [Display(Name = "Arka Plan Rengi")]
    public string BackgroundColor { get; set; } = "#FFFFFF";

    [Display(Name = "Logo (opsiyonel)")]
    public IFormFile? LogoFile { get; set; }

    public string? ExistingLogoPath { get; set; }

    [Display(Name = "Logoyu Kaldır")]
    public bool RemoveLogo { get; set; }
}
