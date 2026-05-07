using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class AnnouncementFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık gerekli.")]
    [MaxLength(200)]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Mesaj")]
    public string Message { get; set; } = string.Empty;

    [Url(ErrorMessage = "Geçerli bir URL girin.")]
    [MaxLength(500)]
    [Display(Name = "Bağlantı URL (opsiyonel)")]
    public string? LinkUrl { get; set; }

    [Required(ErrorMessage = "Başlangıç tarihi gerekli.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Başlangıç")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Bitiş tarihi gerekli.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Bitiş")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);

    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex renk girin.")]
    [Display(Name = "Arka Plan Rengi")]
    public string BackgroundColor { get; set; } = "#C8A032";

    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex renk girin.")]
    [Display(Name = "Yazı Rengi")]
    public string TextColor { get; set; } = "#FFFFFF";

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
            yield return new ValidationResult(
                "Bitiş tarihi başlangıç tarihinden sonra olmalı.",
                new[] { nameof(EndDate) });
    }
}
