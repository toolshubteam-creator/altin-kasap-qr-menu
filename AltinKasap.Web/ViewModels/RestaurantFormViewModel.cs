using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class RestaurantFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad gerekli.")]
    [MaxLength(200)]
    [Display(Name = "Restoran Adı")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Adres")]
    public string? Address { get; set; }

    [MaxLength(50)]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [Url(ErrorMessage = "Geçerli bir URL girin.")]
    [MaxLength(500)]
    [Display(Name = "Instagram URL")]
    public string? InstagramUrl { get; set; }

    [Url(ErrorMessage = "Geçerli bir URL girin.")]
    [MaxLength(500)]
    [Display(Name = "Facebook URL")]
    public string? FacebookUrl { get; set; }

    [Url(ErrorMessage = "Geçerli bir URL girin.")]
    [MaxLength(500)]
    [Display(Name = "Web Sitesi URL")]
    public string? WebsiteUrl { get; set; }

    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex renk girin.")]
    [Display(Name = "Birincil Renk")]
    public string PrimaryColor { get; set; } = "#C8A032";

    [Display(Name = "Logo")]
    public IFormFile? LogoFile { get; set; }
    public string? ExistingLogoPath { get; set; }
    [Display(Name = "Mevcut logoyu kaldır")]
    public bool RemoveLogo { get; set; }

    [Display(Name = "Kapak Görseli")]
    public IFormFile? CoverFile { get; set; }
    public string? ExistingCoverPath { get; set; }
    [Display(Name = "Mevcut kapak görselini kaldır")]
    public bool RemoveCover { get; set; }
}
