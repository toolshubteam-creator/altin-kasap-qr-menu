using AltinKasap.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Data;

public static class DbSeeder
{
    public const string AdminRoleName = "Admin";
    public const string DefaultAdminEmail = "admin@altinkasap.local";

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        await SeedIdentityAsync(services, configuration);

        var db = services.GetRequiredService<AppDbContext>();
        await SeedRestaurantAsync(db);
        await SeedProductTagsAsync(db);
        await SeedCategoriesAsync(db);
        await SeedProductsAsync(db);
    }

    private static async Task SeedIdentityAsync(IServiceProvider services, IConfiguration configuration)
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

    private static async Task SeedRestaurantAsync(AppDbContext db)
    {
        if (await db.Restaurants.AnyAsync()) return;

        db.Restaurants.Add(new Restaurant
        {
            Name = "Altın Kasap",
            Slug = "altin-kasap",
            Address = "Sakarya, Türkiye",
            Phone = "+90 264 000 00 00",
            PrimaryColor = "#C8A032",
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    private static async Task SeedProductTagsAsync(AppDbContext db)
    {
        if (await db.ProductTags.AnyAsync()) return;

        var tags = new[]
        {
            new ProductTag { Name = "Glutensiz",     IconClass = "fa-wheat-awn-circle-exclamation", ColorHex = "#8B4513" },
            new ProductTag { Name = "Vejetaryen",    IconClass = "fa-leaf",                         ColorHex = "#228B22" },
            new ProductTag { Name = "Vegan",         IconClass = "fa-seedling",                     ColorHex = "#006400" },
            new ProductTag { Name = "Acılı",         IconClass = "fa-pepper-hot",                   ColorHex = "#DC143C" },
            new ProductTag { Name = "Süt İçeren",    IconClass = "fa-cow",                          ColorHex = "#4682B4" },
            new ProductTag { Name = "Şefin Önerisi", IconClass = "fa-star",                         ColorHex = "#FFD700" },
            new ProductTag { Name = "Yeni",          IconClass = "fa-bolt",                         ColorHex = "#FF8C00" }
        };
        db.ProductTags.AddRange(tags);
        await db.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(AppDbContext db)
    {
        if (await db.Categories.AnyAsync()) return;

        var now = DateTime.UtcNow;
        var categories = new[]
        {
            new Category { Name = "Tavuk Izgara",      Slug = "tavuk-izgara",      IconClass = "fa-drumstick-bite",  SortOrder = 1, IsActive = true, CreatedAt = now },
            new Category { Name = "Ekmek Arası",       Slug = "ekmek-arasi",       IconClass = "fa-bread-slice",     SortOrder = 2, IsActive = true, CreatedAt = now },
            new Category { Name = "Et Izgara",         Slug = "et-izgara",         IconClass = "fa-fire-flame-curved", SortOrder = 3, IsActive = true, CreatedAt = now },
            new Category { Name = "Kiloluk Lezzetler", Slug = "kiloluk-lezzetler", IconClass = "fa-weight-hanging",  SortOrder = 4, IsActive = true, CreatedAt = now },
            new Category { Name = "İçecekler",         Slug = "icecekler",         IconClass = "fa-mug-saucer",      SortOrder = 5, IsActive = true, CreatedAt = now },
            new Category { Name = "Ekstralar",         Slug = "ekstralar",         IconClass = "fa-plus",            SortOrder = 6, IsActive = true, CreatedAt = now }
        };
        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(AppDbContext db)
    {
        if (await db.Products.AnyAsync()) return;

        var categoryIdBySlug = await db.Categories.ToDictionaryAsync(c => c.Slug, c => c.Id);

        var seed = new (string CategorySlug, string Name, decimal Price, string Unit)[]
        {
            ("tavuk-izgara",      "Tavuk Pirzola",        190.00m, "Porsiyon"),
            ("tavuk-izgara",      "Tavuk İncik",          190.00m, "Porsiyon"),
            ("tavuk-izgara",      "Tavuk Kanat",          195.00m, "Porsiyon"),
            ("tavuk-izgara",      "Tavuk Kelebek",        190.00m, "Porsiyon"),
            ("tavuk-izgara",      "Tavuk Şiş",            195.00m, "Porsiyon"),
            ("tavuk-izgara",      "Tavuk Şinitsel",       195.00m, "Porsiyon"),

            ("ekmek-arasi",       "Köfte",                130.00m, "Adet"),
            ("ekmek-arasi",       "Sucuk",                130.00m, "Adet"),
            ("ekmek-arasi",       "Tavuk",                120.00m, "Adet"),

            ("et-izgara",         "Kasap Köfte",          230.00m, "Porsiyon"),
            ("et-izgara",         "Kasap Sucuk",          240.00m, "Porsiyon"),
            ("et-izgara",         "Adana-Urfa",           260.00m, "Porsiyon"),
            ("et-izgara",         "Antrikot",             300.00m, "Porsiyon"),
            ("et-izgara",         "Sırt Biftek",          300.00m, "Porsiyon"),
            ("et-izgara",         "Bonfile",              350.00m, "Porsiyon"),
            ("et-izgara",         "Kuzu Şiş",             330.00m, "Porsiyon"),
            ("et-izgara",         "Kuzu Pirzola",         350.00m, "Porsiyon"),
            ("et-izgara",         "Kuzu Lokum",           350.00m, "Porsiyon"),
            ("et-izgara",         "Kuzu Külbastı",        330.00m, "Porsiyon"),

            ("kiloluk-lezzetler", "Kasap Köfte (Kg)",     900.00m, "Kg"),
            ("kiloluk-lezzetler", "Seçmeli Tavuk (Kg)",   300.00m, "Kg"),

            ("icecekler",         "Litrelik",              75.00m, "1 Litre"),
            ("icecekler",         "Kutu",                  40.00m, "Kutu"),
            ("icecekler",         "Ayran",                 25.00m, "Bardak"),
            ("icecekler",         "Şalgam",                30.00m, "Bardak"),
            ("icecekler",         "Su",                    10.00m, "Şişe"),

            ("ekstralar",         "Salata",                 0.00m, "Porsiyon"),
            ("ekstralar",         "Mezeler",                0.00m, "Porsiyon"),
            ("ekstralar",         "Tatlılar",              60.00m, "Porsiyon")
        };

        var now = DateTime.UtcNow;
        var perCategoryOrder = new Dictionary<string, int>();
        var products = new List<Product>();
        foreach (var p in seed)
        {
            perCategoryOrder.TryGetValue(p.CategorySlug, out var current);
            current += 1;
            perCategoryOrder[p.CategorySlug] = current;

            products.Add(new Product
            {
                CategoryId = categoryIdBySlug[p.CategorySlug],
                Name = p.Name,
                Price = p.Price,
                Unit = p.Unit,
                IsActive = true,
                IsSoldOut = false,
                SortOrder = current,
                CreatedAt = now
            });
        }

        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }
}
