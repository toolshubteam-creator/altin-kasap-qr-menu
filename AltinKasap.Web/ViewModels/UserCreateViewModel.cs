using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class UserCreateViewModel
{
    [Required(ErrorMessage = "E-posta gerekli.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
    [MaxLength(200)]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "Ad Soyad")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Şifre gerekli.")]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalı.")]
    [MaxLength(100)]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı gerekli.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Şifre (Tekrar)")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
