using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class UserEditViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta gerekli.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
    [MaxLength(200)]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "Ad Soyad")]
    public string? FullName { get; set; }
}
