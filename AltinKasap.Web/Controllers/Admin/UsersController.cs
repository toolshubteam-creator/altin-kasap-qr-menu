using AltinKasap.Web.Models;
using AltinKasap.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/users")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.OrderBy(u => u.UserName).ToList();
        var list = new List<UserListItemViewModel>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            list.Add(new UserListItemViewModel
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                LastLoginAt = u.LastLoginAt,
                Roles = roles.ToList(),
                LockoutEnd = u.LockoutEnd
            });
        }
        return View(list);
    }

    [HttpGet("create")]
    public IActionResult Create() => View(new UserCreateViewModel());

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var existing = await _userManager.FindByEmailAsync(model.Email);
        if (existing != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Bu e-posta zaten kayıtlı.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, TranslateIdentityError(err));
            return View(model);
        }

        const string adminRole = "Admin";
        if (!await _roleManager.RoleExistsAsync(adminRole))
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        await _userManager.AddToRoleAsync(user, adminRole);

        _logger.LogInformation("New admin created: {Email} by {Creator}", model.Email, User.Identity?.Name);
        TempData["Success"] = $"'{model.Email}' başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        return View(new UserEditViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName
        });
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserEditViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var editingSelf = string.Equals(user.UserName, User.Identity?.Name, StringComparison.OrdinalIgnoreCase);

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null && existing.Id != user.Id)
            {
                ModelState.AddModelError(nameof(model.Email), "Bu e-posta başka bir hesapta kullanılıyor.");
                return View(model);
            }
            user.UserName = model.Email;
            user.Email = model.Email;
            user.NormalizedEmail = _userManager.NormalizeEmail(model.Email);
            user.NormalizedUserName = _userManager.NormalizeName(model.Email);
        }

        user.FullName = model.FullName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, TranslateIdentityError(err));
            return View(model);
        }

        _logger.LogInformation("User edited: {Email} by {Editor}", user.Email, User.Identity?.Name);
        TempData["Success"] = "Kullanıcı bilgileri güncellendi.";

        if (editingSelf)
        {
            await _signInManager.RefreshSignInAsync(user);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("change-password/{id}")]
    public async Task<IActionResult> ChangePassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        return View(new ChangePasswordViewModel
        {
            UserId = user.Id,
            UserEmail = user.Email ?? string.Empty,
            IsSelf = string.Equals(user.UserName, User.Identity?.Name, StringComparison.OrdinalIgnoreCase)
        });
    }

    [HttpPost("change-password/{id}")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("LoginAttempt")]
    public async Task<IActionResult> ChangePassword(string id, ChangePasswordViewModel model)
    {
        if (id != model.UserId) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var isSelf = string.Equals(user.UserName, User.Identity?.Name, StringComparison.OrdinalIgnoreCase);
        model.IsSelf = isSelf;

        IdentityResult result;
        if (isSelf)
        {
            if (string.IsNullOrEmpty(model.CurrentPassword))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Mevcut şifre zorunludur.");
                return View(model);
            }
            result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        }
        else
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
        }

        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, TranslateIdentityError(err));
            return View(model);
        }

        _logger.LogInformation("Password changed for {Email} by {Changer}", user.Email, User.Identity?.Name);
        TempData["Success"] = isSelf
            ? "Şifreniz değiştirildi."
            : $"'{user.Email}' kullanıcısının şifresi sıfırlandı.";

        if (isSelf)
            await _signInManager.RefreshSignInAsync(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (string.Equals(user.UserName, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Kendi hesabınızı silemezsiniz.";
            return RedirectToAction(nameof(Index));
        }

        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
        if (adminUsers.Count <= 1)
        {
            TempData["Error"] = "Sistemde en az bir admin kullanıcı bulunmalıdır.";
            return RedirectToAction(nameof(Index));
        }

        var email = user.Email;
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            TempData["Error"] = "Kullanıcı silinirken hata oluştu: " +
                string.Join(", ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        _logger.LogWarning("User deleted: {Email} by {Deleter}", email, User.Identity?.Name);
        TempData["Success"] = $"'{email}' silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("unlock/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.ResetAccessFailedCountAsync(user);

        _logger.LogInformation("User unlocked: {Email} by {Unlocker}", user.Email, User.Identity?.Name);
        TempData["Success"] = $"'{user.Email}' kilidi açıldı.";
        return RedirectToAction(nameof(Index));
    }

    private static string TranslateIdentityError(IdentityError err) => err.Code switch
    {
        "PasswordTooShort" => "Şifre çok kısa.",
        "PasswordRequiresDigit" => "Şifre en az bir rakam içermelidir.",
        "PasswordRequiresLower" => "Şifre en az bir küçük harf içermelidir.",
        "PasswordRequiresUpper" => "Şifre en az bir büyük harf içermelidir.",
        "PasswordRequiresNonAlphanumeric" => "Şifre en az bir özel karakter içermelidir.",
        "PasswordRequiresUniqueChars" => "Şifre yeterli benzersiz karakter içermiyor.",
        "PasswordMismatch" => "Mevcut şifre hatalı.",
        "DuplicateUserName" => "Bu kullanıcı adı zaten alınmış.",
        "DuplicateEmail" => "Bu e-posta zaten kayıtlı.",
        "InvalidEmail" => "Geçersiz e-posta formatı.",
        "InvalidUserName" => "Geçersiz kullanıcı adı.",
        _ => err.Description
    };
}
