using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class DailySpecialFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ürün seçin.")]
    [Display(Name = "Ürün")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Tarih gerekli.")]
    [DataType(DataType.Date)]
    [Display(Name = "Öneri Tarihi")]
    public DateTime SpecialDate { get; set; } = DateTime.Today;

    [MaxLength(500)]
    [Display(Name = "Not")]
    public string? Note { get; set; }
}
