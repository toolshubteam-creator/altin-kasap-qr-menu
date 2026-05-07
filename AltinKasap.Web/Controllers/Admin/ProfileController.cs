using AltinKasap.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AltinKasap.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("admin/profile")]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();
        return RedirectToAction("Edit", "Users", new { id = user.Id });
    }

    [HttpGet("change-password")]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();
        return RedirectToAction("ChangePassword", "Users", new { id = user.Id });
    }
}
