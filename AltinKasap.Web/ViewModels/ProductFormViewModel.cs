using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori seçin.")]
    [Display(Name = "Kategori")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Ad gerekli.")]
    [MaxLength(200)]
    [Display(Name = "Ad")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [MaxLength(100)]
    [Display(Name = "Birim")]
    public string? Unit { get; set; }

    [Required(ErrorMessage = "Fiyat gerekli.")]
    [Range(0, 100000, ErrorMessage = "Fiyat 0 ile 100.000 arasında olmalı.")]
    [Display(Name = "Fiyat (₺)")]
    public decimal Price { get; set; }

    [Display(Name = "Görsel")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImagePath { get; set; }

    [Display(Name = "Mevcut görseli kaldır")]
    public bool RemoveImage { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Tükendi")]
    public bool IsSoldOut { get; set; }

    public List<int> SelectedTagIds { get; set; } = new();
}
