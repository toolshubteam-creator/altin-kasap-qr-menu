using System.Text.Encodings.Web;
using System.Text.Unicode;
using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using AltinKasap.Web.Repositories;
using AltinKasap.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/altinkasap-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "AltinKasap.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

    builder.Services.AddMemoryCache();

    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IQrCodeRepository, QrCodeRepository>();
    builder.Services.AddScoped<IQrScanLogRepository, QrScanLogRepository>();
    builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
    builder.Services.AddScoped<IDailySpecialRepository, DailySpecialRepository>();
    builder.Services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
    builder.Services.AddScoped<IProductTagRepository, ProductTagRepository>();
    builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();

    builder.Services.AddScoped<IMenuService, MenuService>();
    builder.Services.AddScoped<IQrService, QrService>();
    builder.Services.AddScoped<IReportService, ReportService>();
    builder.Services.AddScoped<IImageService, ImageService>();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(UnicodeRanges.All));

    builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Menu}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "menu_short",
        pattern: "altin-kasap",
        defaults: new { controller = "Menu", action = "Index" });

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            await DbSeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
            Log.Information("Database seed completed successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database seed failed");
        }
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException
                            && ex.GetType().FullName != "Microsoft.Extensions.Hosting.HostAbortedException")
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
