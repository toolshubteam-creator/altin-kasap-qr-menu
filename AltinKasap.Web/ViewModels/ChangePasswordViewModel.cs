using System.ComponentModel.DataAnnotations;

namespace AltinKasap.Web.ViewModels;

public class ChangePasswordViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public bool IsSelf { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Mevcut Şifre")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Yeni şifre gerekli.")]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalı.")]
    [MaxLength(100)]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı gerekli.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Yeni Şifre (Tekrar)")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
