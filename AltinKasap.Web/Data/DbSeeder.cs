using AltinKasap.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AltinKasap.Web.Data;

public static class DbSeeder
{
    public const string AdminRoleName = "Admin";
    public const string DefaultAdminEmail = "admin@altinkasap.local";

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(AdminRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
        }

        var adminEmail = configuration["DefaultAdmin:Email"];
        if (string.IsNullOrWhiteSpace(adminEmail)) adminEmail = DefaultAdminEmail;

        var adminPassword = configuration["DefaultAdmin:Password"];
        if (string.IsNullOrWhiteSpace(adminPassword)) adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Sistem Yöneticisi"
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Admin oluşturulamadı: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRoleName))
        {
            await userManager.AddToRoleAsync(admin, AdminRoleName);
        }
    }
}
